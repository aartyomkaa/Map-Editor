using System;
using System.Collections.Generic;
using CodeBase.Terrain;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBase.Logic
{
    public class TouchInput : MonoBehaviour
    {
        [SerializeField] private TerrainStaticData _data;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private Button _button;
        
        private LayerMask _ui = 5;
        private PointerEventData _pointerEventData;
        private List<RaycastResult> _results;

        private TerrainEditor _terrainEditor;
        private Ray _ray;
        private float _rayDistance = 100f;

        public GameObject meshFinderPrefab; // Prefab for the visual marker
        public float raiseAmount = 1f;      // Amount to raise the terrain
        public float effectRadius = 5f;     // Radius of effect for raising vertices

        private GameObject _meshFinderInstance;
        private List<Transform> _currentChunks = new List<Transform>();

        private void Start()
        {
            _button.onClick.AddListener(RaiseTerrain);
            
            _pointerEventData = new PointerEventData(EventSystem.current);
            _results = new List<RaycastResult>();
        }

        void Update()
        {
            HandleTouch();
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(RaiseTerrain);
        }

        private void HandleTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                
                if (IsPointerOverUI(touch.position))
                    return;
                
                if (touch.phase == TouchPhase.Stationary)
                {
                    _ray = Camera.main.ScreenPointToRay(touch.position);
                    
                    if (Physics.Raycast(_ray, out RaycastHit hit, _rayDistance, _groundMask))
                    {
                        if (_meshFinderInstance == null)
                        {
                            _meshFinderInstance = Instantiate(meshFinderPrefab);
                        }
                        
                        _meshFinderInstance.transform.position = hit.point;
                        
                        _currentChunks.Clear();
                        
                        Collider[] hitColliders = Physics.OverlapSphere(hit.point, effectRadius, _groundMask);
                        
                        foreach (var collider in hitColliders)
                        {
                            _currentChunks.Add(collider.transform);
                        }
                    }
                }
            }
        }

        public void RaiseTerrain()
        {
            if (_meshFinderInstance != null && _currentChunks.Count > 0)
            {
                foreach (var chunk in _currentChunks)
                {
                    ModifyMesh(chunk);
                }
            }
        }

        private void ModifyMesh(Transform chunk)
        {
            MeshFilter meshFilter = chunk.GetComponent<MeshFilter>();
            MeshCollider meshCollider = chunk.GetComponent<MeshCollider>();
            Debug.Log(meshCollider);

            if (meshFilter != null)
            {
                Mesh mesh = meshFilter.mesh;
                Vector3[] vertices = mesh.vertices;

                // Get the position of the mesh finder in the local space of the chunk
                Vector3 localFinderPosition = chunk.InverseTransformPoint(_meshFinderInstance.transform.position);
                
                // Iterate through each vertex to check if it's within the effect radius
                for (int i = 0; i < vertices.Length; i++)
                {
                    // Calculate the distance in the XZ plane (ignoring Y axis)
                    float distance = Vector2.Distance(new Vector2(vertices[i].x, vertices[i].z), new Vector2(localFinderPosition.x, localFinderPosition.z));

                    // Check if the vertex is within the effect radius (on the XZ plane)
                    if (distance <= effectRadius)
                    {
                        // Apply vertical displacement only in Y direction
                        float heightFactor = Mathf.Clamp01(1 - (distance / effectRadius)); // Smooth factor based on distance
                        vertices[i].y += raiseAmount * heightFactor;
                    }
                }

                // Update the mesh with modified vertices
                mesh.vertices = vertices;

                // Recalculate normals and bounds to update lighting and mesh boundaries
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                meshCollider.sharedMesh = mesh;
            }
        }

        private Vector3 FindClosestVertex(Vector3 worldPosition, Vector3[] vertices)
        {
            Vector3 closestVertex = Vector3.zero;
            float closestDistance = Mathf.Infinity;

            // Iterate through the vertices and find the closest one
            foreach (var vertex in vertices)
            {
                float distance = Vector3.Distance(worldPosition, vertex);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestVertex = vertex;
                }
            }

            return closestVertex;
        }

        public bool IsPointerOverUI(Vector2 screenPosition)
        {
            _results.Clear();
            _pointerEventData.position = screenPosition;
            EventSystem.current.RaycastAll(_pointerEventData, _results);

            foreach (RaycastResult result in _results)
            {
                if (result.gameObject.layer == _ui)
                {
                    return true;
                }
            }

            return false;
        }
    }
}