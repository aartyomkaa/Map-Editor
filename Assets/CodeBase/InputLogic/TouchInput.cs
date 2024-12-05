using System;
using System.Collections.Generic;
using CodeBase.Logic;
using CodeBase.Terrain;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.InputLogic
{
    public class TouchInput : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _rayDistance = 100f;
        
        private LayerMask _uiLayerMask = 5;

        private PointerEventData _pointerEventData;
        private List<RaycastResult> _results;

        private TerrainEditor _terrainEditor;
        private Ray _ray;

        public GameObject meshFinderPrefab;

        private GameObject _meshFinderInstance;
        private GridModifier _gridModifier;
        
        public event Action<Vector3, LayerMask> BrushMoved;
        
        private void Start()
        {
            _meshFinderInstance = Instantiate(meshFinderPrefab);
            _pointerEventData = new PointerEventData(EventSystem.current);
            _results = new List<RaycastResult>();
        }

        void Update()
        {
            HandleTouch();
        }

        public Vector3 GetBrushPosition() => 
            _meshFinderInstance.transform.position;

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
                        _meshFinderInstance.transform.position = hit.point;
                        
                        BrushMoved?.Invoke(hit.point, _groundMask);
                    }
                }
            }
        }

        private bool IsPointerOverUI(Vector2 screenPosition)
        {
            _results.Clear();
            _pointerEventData.position = screenPosition;
            EventSystem.current.RaycastAll(_pointerEventData, _results);

            foreach (RaycastResult result in _results)
            {
                if (result.gameObject.layer == _uiLayerMask)
                {
                    return true;
                }
            }

            return false;
        }
    }
}