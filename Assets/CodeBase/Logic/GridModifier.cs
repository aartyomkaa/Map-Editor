using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Logic
{
    public class GridModifier
    {
        private float _area = 5f;
        private float _strength = 1f;
        private float _smoothness = 1f;
        private float _intensity = 1f;

        private List<Transform> _currentChunks = new List<Transform>();

        public void SetChunks(Vector3 hitPoint, LayerMask groundMask)
        {
            _currentChunks.Clear();
                        
            Collider[] hitColliders = Physics.OverlapSphere(hitPoint, _area, groundMask);
                        
            foreach (var collider in hitColliders)
            {
                _currentChunks.Add(collider.transform);
            }
        }

        public void RaiseTerrain(Vector3 meshFinderPosition)
        {
            if (_currentChunks.Count > 0)
            {
                foreach (var chunk in _currentChunks)
                {
                    ModifyMesh(chunk, meshFinderPosition, true);
                }
            }
        }

        public void LowerTerrain(Vector3 meshFinderPosition)
        {
            if (_currentChunks.Count > 0)
            {
                foreach (var chunk in _currentChunks)
                {
                    ModifyMesh(chunk, meshFinderPosition, false);
                }
            }
        }

        private void ModifyMesh(Transform chunk, Vector3 meshFinderPosition, bool isRaise)
        {
            MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
            MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();

            float strength = isRaise ? _strength : -_strength;

            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;
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
                        float heightFactor = Mathf.Pow(falloff, _smoothness) * _intensity;
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

        public void AdjustIntensity(float value) => 
            _intensity = value;

        public void AdjustStrength(float value) => 
            _strength = value;

        public void AdjustArea(float value) => 
            _area = value;
    }
}