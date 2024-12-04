using UnityEditor;
using UnityEngine;

namespace CodeBase
{
    public class GenerateUV : MonoBehaviour
    {
        void Start()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;

                // Generate UVs
                Vector2[] uvs = new Vector2[mesh.vertices.Length];
                for (int i = 0; i < uvs.Length; i++)
                {
                    Vector3 vertex = mesh.vertices[i];
                    uvs[i] = new Vector2(vertex.x, vertex.z); // Top-down UV mapping (XZ plane)
                }

                mesh.uv = uvs; // Assign the new UVs

#if UNITY_EDITOR
                // Save the modified mesh to a new asset
                SaveMesh(mesh, "Assets/SavedMesh.asset");
#endif
            }
        }

#if UNITY_EDITOR
        // Function to save the mesh as an asset
        void SaveMesh(Mesh mesh, string path)
        {
            Mesh newMesh = Instantiate(mesh); // Create a new mesh instance
            AssetDatabase.CreateAsset(newMesh, path); // Save it to the specified path
            AssetDatabase.SaveAssets();
            Debug.Log($"Mesh saved to {path}");
        }
#endif
    }
}