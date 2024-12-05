using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using CodeBase.Constants;
using UnityEngine;
using CompressionLevel = UnityEngine.CompressionLevel;
using Object = UnityEngine.Object;

namespace CodeBase.Logic
{
    public class Grid
    {
        private readonly int _gridSize = 10;
        private readonly int _chunkSize = 100;

        private Dictionary<string, string> _savedTextures = new Dictionary<string, string>();
        private Dictionary<Vector2Int, Chunk> _chunks = new();
        private Chunk _chunkPrefab;
        private GameObject _parent;

        public Grid(Chunk chunkPrefab, GameObject parent)
        {
            _chunkPrefab = chunkPrefab;
            _parent = parent;
        }
        
        public void GenerateDefaultChunks()
        {
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    var localPosition = new Vector2Int(x, y);
                    _chunks[localPosition] = CreateDefaultChunk(localPosition);
                }
            }
        }

        public void LoadGrid(string jsonPath)
        {
            SerializedChunk[] savedChunks;
            
            if (File.Exists(jsonPath))
            {
                 savedChunks = LoadSerializedGrid(LoadCompressedJson(jsonPath));
            }
            else
            {
                throw new Exception("no such grid");
            }
            
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    var localPosition = new Vector2Int(x, y);
                    _chunks[localPosition] = LoadChunk(localPosition, savedChunks);
                }
            }
        }

        private SerializedChunk[] LoadSerializedGrid(string json)
        {
            if (string.IsNullOrEmpty(json) == false)
            {
                SerializationWrapper<SerializedChunk> wrapper = JsonUtility.FromJson<SerializationWrapper<SerializedChunk>>(json);
                
                return wrapper.Items;
            }

            throw new Exception("No such map");
        }

        private Chunk CreateDefaultChunk(Vector2Int localPosition)
        {
            Chunk instance = Object.Instantiate(_chunkPrefab, _parent.transform);
            instance.transform.localPosition = new Vector3(localPosition.x * _chunkSize, 0, localPosition.y * _chunkSize);

            return instance;
        }

        private Chunk LoadChunk(Vector2Int localPosition, SerializedChunk[] savedGrid)
        {
            Chunk instance = Object.Instantiate(_chunkPrefab, _parent.transform);
            instance.transform.localPosition = new Vector3(localPosition.x * _chunkSize, 0, localPosition.y * _chunkSize);

            SerializedChunk serializedChunk = savedGrid.First(x => x.LocalPosition == localPosition);
            Mesh mesh = instance.GetComponent<MeshFilter>().mesh;

            mesh.triangles = serializedChunk.Triangles;
            mesh.vertices = serializedChunk.Vertices;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            if (!string.IsNullOrEmpty(serializedChunk.TexturePath) && File.Exists(serializedChunk.TexturePath))
            {
                byte[] textureData = File.ReadAllBytes(serializedChunk.TexturePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(textureData);

                instance.GetComponent<MeshRenderer>().material.mainTexture = texture;
            }
            else
            {
                instance.GetComponent<MeshRenderer>().material.mainTexture = _chunkPrefab.GetComponent<MeshRenderer>().material.mainTexture;
            }

            return instance;
        }

        public void SaveGrid()
        {
            List<SerializedChunk> serializedChunks = new List<SerializedChunk>();
            foreach (var chunkPair in _chunks)
            {
                var chunk = chunkPair.Value;
                Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;

                var texture = chunk.GetComponent<MeshRenderer>().material.mainTexture as Texture2D;
                string texturePath = texture != null ? SaveTextureToDisk(texture, chunkPair.Key.ToString()) : null;

                SerializedChunk serializedChunk = new SerializedChunk
                {
                    LocalPosition = chunkPair.Key,
                    Vertices = mesh.vertices,
                    Triangles = mesh.triangles,
                    TexturePath = texturePath
                };

                serializedChunks.Add(serializedChunk);
            }

            string json = JsonUtility.ToJson(new SerializationWrapper<SerializedChunk>(serializedChunks.ToArray()));
            string fileName = "SavedMap_" + DateTime.Now.ToString("yyyy.MM.dd_HH-mm-ss") + ".json";
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            SaveCompressedJson(filePath, json);
            SaveFilePath(filePath);
        }

        private void SaveFilePath(string filePath)
        {
            string savedPaths = PlayerPrefs.GetString(Preferences.SavedPaths, string.Empty);
        
            if (string.IsNullOrEmpty(savedPaths))
            {
                savedPaths = filePath;
            }
            else
            {
                savedPaths += "," + filePath;
            }

            PlayerPrefs.SetString(Preferences.SavedPaths, savedPaths);
            PlayerPrefs.Save();
        }
        
        private string ComputeTextureHash(Texture2D texture)
        {
            byte[] textureData = texture.GetRawTextureData();
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(textureData);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
        
        private string SaveTextureToDisk(Texture2D texture, string textureName)
        {
            string textureHash = ComputeTextureHash(texture);

            // Check if the texture has already been saved
            if (_savedTextures.TryGetValue(textureHash, out string existingPath))
            {
                return existingPath; // Reuse the existing path
            }

            // Save the new texture
            string directory = Path.Combine(Application.persistentDataPath, "Textures");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filePath = Path.Combine(directory, $"{textureHash}.png");
            byte[] textureBytes = texture.EncodeToPNG();
            File.WriteAllBytes(filePath, textureBytes);

            // Store in the dictionary for reuse
            _savedTextures[textureHash] = filePath;

            return filePath;
        }
        
        private void SaveCompressedJson(string filePath, string json)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var gzipStream = new GZipStream(fileStream, System.IO.Compression.CompressionLevel.Optimal))
            {
                gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }
        
        private string LoadCompressedJson(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var gzipStream = new GZipStream(fileStream, CompressionMode.Decompress))
            using (var reader = new StreamReader(gzipStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}