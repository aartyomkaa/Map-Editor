using CodeBase.Logic.Chunks;
using UnityEditor;
using UnityEngine;

namespace CodeBase.Logic
{
    public class MeshGenerator : MonoBehaviour
    {
        [SerializeField] private int _width;
        [SerializeField] private int _depth;
        [SerializeField] private int _cellSize;
        [SerializeField] private int _chunkSize;

        private void Start()
        {
            CreateChunk();
        }

        private void CreateChunk()
        {
            GameObject chunk = new GameObject("TerrainChunk");
            chunk.layer = LayerMask.NameToLayer("Ground");

            // Set the chunk's world position
            chunk.transform.position = new Vector3(_cellSize, 0, _cellSize);

            chunk.AddComponent<Chunk>();
            MeshFilter meshFilter = chunk.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = chunk.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));

            Mesh mesh = new Mesh();

            int chunkWidth = Mathf.Min(_chunkSize, _width);
            int chunkDepth = Mathf.Min(_chunkSize, _depth);

            Vector3[] vertices = new Vector3[(chunkWidth + 1) * (chunkDepth + 1)];
            int[] triangles = new int[chunkWidth * chunkDepth * 6];

            int vertIndex = 0;
            int triIndex = 0;
            
            for (int z = 0; z <= chunkDepth; z++)
            {
                for (int x = 0; x <= chunkWidth; x++)
                {
                    vertices[vertIndex] = new Vector3(x * _cellSize, 0, z * _cellSize);
                    vertIndex++;
                }
            }
            
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
            
            Vector2[] uvs = new Vector2[mesh.vertices.Length];
            for (int i = 0; i < uvs.Length; i++)
            {
                Vector3 vertex = mesh.vertices[i];
                uvs[i] = new Vector2(vertex.x, vertex.z);
            }

            mesh.uv = uvs;
            meshFilter.mesh = mesh;
            
#if UNITY_EDITOR
            SaveMesh(mesh, "Assets/GeneratedMesh.asset");
#endif
        }
        
#if UNITY_EDITOR
        void SaveMesh(Mesh mesh, string path)
        {
            Mesh newMesh = Instantiate(mesh);
            AssetDatabase.CreateAsset(newMesh, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"Mesh saved to {path}");
        }
#endif
    }
}

