using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Logic
{
    public class GridModifier
    {
        private float _area = 5f;
        private float _strength = 1f;
        private float _smoothness = 1f;

        private Vector3 _brushPosition;
        private List<Transform> _currentChunks = new List<Transform>();

        public void SetChunks(Vector3 hitPoint, LayerMask groundMask)
        {
            _currentChunks.Clear();
            _brushPosition = hitPoint;
                        
            Collider[] hitColliders = Physics.OverlapSphere(hitPoint, _area, groundMask);
                        
            foreach (var collider in hitColliders)
            {
                _currentChunks.Add(collider.transform);
            }
        }

        public void RaiseTerrain()
        {
            if (_currentChunks.Count > 0)
            {
                foreach (var chunk in _currentChunks)
                {
                    ModifyMesh(chunk, _brushPosition, true);
                }
            }
        }

        public void LowerTerrain()
        {
            if (_currentChunks.Count > 0)
            {
                foreach (var chunk in _currentChunks)
                {
                    ModifyMesh(chunk, _brushPosition, false);
                }
            }
        }

        public void ApplyTexture(float blendingFactor)
        {
            if (_currentChunks.Count > 0)
            {
                foreach (var chunk in _currentChunks)
                {
                    ModifyMeshTexture(blendingFactor, chunk);
                }
            }
        }
        
        private void ModifyMeshTexture(float blendingFactor,Transform chunk)
        {
            Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;

            EnsureColorsArrayMatchesVertices(mesh);

            Vector3[] vertices = mesh.vertices;
            Color[] colors = mesh.colors;
            Color brushColor = new Color(blendingFactor, 0, 0);
            
            Vector3 localBrushPosition = chunk.InverseTransformPoint(_brushPosition);
           
            for (int i = 0; i < vertices.Length; i++)
            {
                float distance = Vector3.Distance(
                    new Vector3(vertices[i].x, vertices[i].y, vertices[i].z),
                    new Vector3(localBrushPosition.x, localBrushPosition.y, localBrushPosition.z));

                if (distance <= _area)
                {
                    colors[i] = brushColor;
                }
            }

            mesh.SetColors(colors);
        }
        
        private void EnsureColorsArrayMatchesVertices(Mesh mesh)
        {
            Vector3[] vertices = mesh.vertices;
            Color[] colors = mesh.colors;

            if (colors == null || colors.Length != vertices.Length)
            {
                colors = new Color[vertices.Length];
                
                for (int i = 0; i < colors.Length; i++)
                {
                    colors[i] = Color.black;
                }

                mesh.colors = colors;
            }
        }
        
        private void ModifyMesh(Transform chunk, Vector3 meshFinderPosition, bool isRaise)
        {
            MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
            MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();
            Mesh mesh = meshFilter.mesh;

            float strength = isRaise ? _strength : -_strength;
            
            if (meshFilter != null)
            {
                Vector3[] vertices = mesh.vertices;

                Vector3 localFinderPosition = chunk.InverseTransformPoint(meshFinderPosition);
                
                for (int i = 0; i < vertices.Length; i++)
                {
                    float distance = Vector2.Distance(
                        new Vector2(vertices[i].x, vertices[i].z),
                        new Vector2(localFinderPosition.x, localFinderPosition.z));

                    if (distance <= _area)
                    {
                        float falloff = Mathf.Clamp01(1 - (distance / _area));
                        float heightFactor = Mathf.Pow(falloff, _smoothness);
                        vertices[i].y += strength * heightFactor;
                    }
                }

                mesh.vertices = vertices;

                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                meshCollider.sharedMesh = mesh;
            }
        }

        public void AdjustSmoothness(float value) => 
            _smoothness = value;

        public void AdjustStrength(float value) => 
            _strength = value;

        public void AdjustArea(float value) => 
            _area = value;
    }
}