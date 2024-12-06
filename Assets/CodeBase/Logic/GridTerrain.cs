using System.Collections.Generic;
using System.Linq;
using CodeBase.Data;
using CodeBase.Logic.Chunks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Logic
{
    public class GridTerrain
    {
        private readonly int _gridSize = 10;
        private readonly int _chunkSize = 100;

        private Dictionary<Vector2Int, Chunk> _chunks = new();
        private SaveLoadService _saveLoadService;
        private Chunk _chunkPrefab;
        private GameObject _parent;

        public GridTerrain(Chunk chunkPrefab, GameObject parent)
        {
            _saveLoadService = new SaveLoadService();
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

        public void LoadGrid(SerializedChunk[] savedChunks)
        {
            for (int y = 0; y < _gridSize; y++)
            {
                for (int x = 0; x < _gridSize; x++)
                {
                    var localPosition = new Vector2Int(x, y);
                    _chunks[localPosition] = CreateSavedChunk(localPosition, savedChunks);
                }
            }
        }

        public Dictionary<Vector2Int, Chunk> GetGrid() =>
            _chunks;
        
        private Chunk CreateDefaultChunk(Vector2Int localPosition)
        {
            Chunk instance = Object.Instantiate(_chunkPrefab, _parent.transform);
            instance.transform.localPosition = new Vector3(localPosition.x * _chunkSize, 0, localPosition.y * _chunkSize);
    
            Mesh mesh = instance.GetComponent<MeshFilter>().mesh;
            Color[] blackColors = new Color[mesh.vertexCount];

            for (int i = 0; i < blackColors.Length; i++)
            {
                blackColors[i] = Color.black;
            }

            mesh.colors = blackColors;

            return instance;
        }

        private Chunk CreateSavedChunk(Vector2Int localPosition, SerializedChunk[] savedGrid)
        {
            Chunk instance = Object.Instantiate(_chunkPrefab, _parent.transform);
            instance.transform.localPosition = new Vector3(localPosition.x * _chunkSize, 0, localPosition.y * _chunkSize);

            SerializedChunk serializedChunk = savedGrid.First(x => x.LocalPosition == localPosition);
            Mesh mesh = instance.GetComponent<MeshFilter>().mesh;
            MeshCollider meshCollider = instance.GetComponent<MeshCollider>();

            mesh.triangles = serializedChunk.Triangles;
            mesh.vertices = serializedChunk.Vertices;
            
            Color[] currentColors = mesh.colors;

            for (int i = 0; i < currentColors.Length; i++)
            {
                float newR = serializedChunk.ColorsR[i];

                currentColors[i].r = newR;
                currentColors[i].g = 0;
                currentColors[i].b = 0;
            }

            mesh.SetColors(currentColors);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
            
            return instance;
        }
    }
}