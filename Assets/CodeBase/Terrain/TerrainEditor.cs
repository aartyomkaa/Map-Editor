using UnityEngine;

namespace CodeBase.Terrain
{
    public class TerrainEditor
    {
        private readonly TerrainStaticData _terrainData;
        
        public Texture2D _terrainTexture;

        private Transform _terrain;
        private Mesh _mesh;

        public TerrainEditor(TerrainStaticData data) => 
            _terrainData = data;

        // Modify height based on touch input
        public void ModifyTerrain(GameObject chunk, Vector3 hitPoint, float modificationAmount)
        {
            
            Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;
            
            Vector3[] vertices = mesh.vertices;  // Get mesh vertices

            // Convert the hit point to local space
            Vector3 localHitPoint = chunk.transform.InverseTransformPoint(hitPoint);

            // Calculate which vertex to modify based on touch position
            int x = Mathf.FloorToInt(localHitPoint.x);
            int z = Mathf.FloorToInt(localHitPoint.z);

            // Ensure x and z are within the mesh bounds
            if (x >= 0 && z >= 0 && x < mesh.vertexCount && z < mesh.vertexCount)
            {
                int vertexIndex = z * (mesh.vertexCount / (z + 1)) + x;  // Get the index of the vertex to modify
                vertices[vertexIndex].y += modificationAmount;  // Modify the height (y value)

                mesh.vertices = vertices;
                mesh.RecalculateNormals();  // Recalculate normals after modification
            }
        }

        // Modify texture based on touch position
        public void ModifyTexture(Vector3 touchPosition, Texture2D newTexture)
        {
            Vector2[] uv = _mesh.uv;
        
            // Convert touch position to terrain space
            Vector3 terrainPosition = _terrain.InverseTransformPoint(touchPosition);
            int x = Mathf.FloorToInt(terrainPosition.x / _terrainData.cellSize);
            int z = Mathf.FloorToInt(terrainPosition.z / _terrainData.cellSize);

            if (x >= 0 && z >= 0 && x < _terrainData.width && z < _terrainData.depth)
            {
                int vertexIndex = z * (_terrainData.width + 1) + x;
                uv[vertexIndex] = new Vector2((float)x / _terrainData.width, (float)z / _terrainData.depth);  // Modify UV based on touch

                _mesh.uv = uv;
            }
        }
    }
}