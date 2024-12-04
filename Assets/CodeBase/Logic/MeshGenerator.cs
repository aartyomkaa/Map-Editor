using System.Collections.Generic;
using CodeBase.Terrain;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Logic
{
    public class MeshGenerator
    {
        private TerrainStaticData _terrainData;
        private List<Mesh> _terrainMeshes = new List<Mesh>();
        private GameObject _parent = new GameObject("TerrainParent");
        private bool _isSaved = false;


        public MeshGenerator(TerrainStaticData terrainData) => 
            _terrainData = terrainData;

        public List<Mesh> CreateMeshTerrain()
        {
            for (int z = 0; z < _terrainData.depth; z += _terrainData.chunkSize)
            {
                for (int x = 0; x < _terrainData.width; x += _terrainData.chunkSize)
                {
                    _terrainMeshes.Add(CreateChunk(x, z));
                }
            }

            return _terrainMeshes;
        }

        private Mesh CreateChunk(int offsetX, int offsetZ)
    {
        GameObject chunk = new GameObject("TerrainChunk");
    chunk.layer = LayerMask.NameToLayer("Ground");
    chunk.transform.SetParent(_parent.transform);
    
    // Set the chunk's world position
    chunk.transform.position = new Vector3(offsetX * _terrainData.cellSize, 0, offsetZ * _terrainData.cellSize);

    MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
    MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
    meshRenderer.material = new Material(Shader.Find("Standard"));

    Mesh mesh = new Mesh();

    int chunkWidth = Mathf.Min(_terrainData.chunkSize, _terrainData.width - offsetX);
    int chunkDepth = Mathf.Min(_terrainData.chunkSize, _terrainData.depth - offsetZ);

    Vector3[] vertices = new Vector3[(chunkWidth + 1) * (chunkDepth + 1)];
    int[] triangles = new int[chunkWidth * chunkDepth * 6];

    int vertIndex = 0;
    int triIndex = 0;

    // Generate vertices
    for (int z = 0; z <= chunkDepth; z++)
    {
        for (int x = 0; x <= chunkWidth; x++)
        {
            // Localize vertices relative to the chunk
            vertices[vertIndex] = new Vector3(x * _terrainData.cellSize, 0, z * _terrainData.cellSize);
            vertIndex++;
        }
    }

    // Generate triangles
    for (int z = 0; z < chunkDepth; z++)
    {
        for (int x = 0; x < chunkWidth; x++)
        {
            int topLeft = z * (chunkWidth + 1) + x;
            int topRight = topLeft + 1;
            int bottomLeft = topLeft + chunkWidth + 1;
            int bottomRight = bottomLeft + 1;

            triangles[triIndex++] = topLeft;
            triangles[triIndex++] = bottomLeft;
            triangles[triIndex++] = topRight;

            triangles[triIndex++] = topRight;
            triangles[triIndex++] = bottomLeft;
            triangles[triIndex++] = bottomRight;
        }
    }

    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.RecalculateNormals();
    meshFilter.mesh = mesh;

    return mesh;
}
    }
}

