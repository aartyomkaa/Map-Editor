using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using CodeBase.Assets;
using CodeBase.Constants;
using CodeBase.Data;
using CodeBase.Logic.Chunks;
using CodeBase.Units;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Logic
{
    public class SaveLoadService
    {
        public List<Unit> Units { get; private set; }
        
        private string _loadedMapPath;

        public void SaveMap(Dictionary<Vector2Int, Chunk> chunks, List<Unit> units)
        {
            List<SerializedChunk> serializedChunks = new List<SerializedChunk>();

            foreach (var chunkPair in chunks)
            {
                SaveChunk(chunkPair, serializedChunks);
            }

            List<SerializedUnit> serializedUnits = units.Select(unit => new SerializedUnit
            {
                Type = unit.Type,
                Position = unit.transform.position,
            }).ToList();

            string jsonGrid = JsonUtility.ToJson(new SerializationWrapper<SerializedChunk>(serializedChunks.ToArray()));
            string jsonUnits = JsonUtility.ToJson(new SerializationWrapper<SerializedUnit>(serializedUnits.ToArray()));

            SavedMap savedMap = new SavedMap();

            savedMap.Grid = jsonGrid;
            savedMap.Units = jsonUnits;

            string combinedJson = JsonUtility.ToJson(savedMap);
            
            string filePath = string.IsNullOrEmpty(_loadedMapPath) 
                ? Path.Combine(Application.persistentDataPath, "SavedMap_" + DateTime.Now.ToString("yyyy.MM.dd_HH-mm-ss") + ".json") 
                : _loadedMapPath;

            SaveCompressedJson(filePath, combinedJson);

            if (string.IsNullOrEmpty(_loadedMapPath))
            {
                SaveFilePath(filePath);
            }
        }

        public SerializedChunk[] LoadMap(string filePath)
        {
            if (File.Exists(filePath) == false)
                throw new Exception("No saved map and units file found.");

            _loadedMapPath = filePath;
            
            string combinedJson = LoadCompressedJson(filePath);
            
            SavedMap savedMap = JsonUtility.FromJson<SavedMap>(combinedJson);
            
            SerializedChunk[] chunks = JsonUtility.FromJson<SerializationWrapper<SerializedChunk>>(savedMap.Grid).Items;
            List<SerializedUnit> serializedUnits = JsonUtility.FromJson<SerializationWrapper<SerializedUnit>>(savedMap.Units).Items.ToList();

            List<Unit> units = new List<Unit>();
            
            foreach (var serializedUnit in serializedUnits)
            {
                string prefabPath = AssetsProvider.GetPath(serializedUnit.Type);
                GameObject prefab = Resources.Load<GameObject>(prefabPath);

                if (prefab == null)
                    throw new Exception($"Prefab not found at path: {prefabPath}");

                GameObject instance = Object.Instantiate(prefab);
                instance.transform.position = serializedUnit.Position;

                units.Add(instance.GetComponent<Unit>());
            }

            Units = units;
            return chunks;
        }

        private void SaveChunk(KeyValuePair<Vector2Int, Chunk> chunkPair, List<SerializedChunk> serializedChunks)
        {
            var chunk = chunkPair.Value;
            Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;
            
            float[] colorsR = mesh.colors.Select(color => color.r).ToArray();
            
            SerializedChunk serializedChunk = new SerializedChunk
            {
                LocalPosition = chunkPair.Key,
                Vertices = mesh.vertices,
                Triangles = mesh.triangles,
                ColorsR = colorsR
            };

            serializedChunks.Add(serializedChunk);
        }

        private void SaveFilePath(string filePath)
        {
            string savedPaths = PlayerPrefs.GetString(Preferences.SavedPaths, string.Empty);

            List<string> paths = string.IsNullOrEmpty(savedPaths)
                ? new List<string>()
                : savedPaths.Split(',').ToList();

            if (paths.Contains(filePath) == false)
            {
                paths.Add(filePath);
            }

            string newSavedPaths = string.Join(",", paths);
            PlayerPrefs.SetString(Preferences.SavedPaths, newSavedPaths);
            PlayerPrefs.Save();
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