using System;
using System.Collections.Generic;
using CodeBase.Logic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CodeBase.InputLogic
{
    public class TouchInput : MonoBehaviour
    {
        [SerializeField] private Texture _texture;
        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private float _rayDistance = 100f;
        [SerializeField] public GameObject meshFinderPrefab;

        private LayerMask _uiLayerMask = 5;

        private PointerEventData _pointerEventData;
        private List<RaycastResult> _results;
        private Ray _ray;
        
        private GameObject _brushInstance;
        private GridModifier _gridModifier;
        
        private float _touchTime = 0f;
        private float _maxTouchDuration = 0.3f;
        private bool _isTouching = false;
        private EditorType _currentEditor;
        private float _lastBrushTime = 0;
        private float _brushCooldown = 0.2f;

        public event Action<Vector3, LayerMask> BrushMovedGrid;
        public event Action<Vector3, LayerMask> BrushMovedUnit;
        public event Action<Vector3, LayerMask> BrushMovedTexture;
        
        private void Start()
        {
            _brushInstance = Instantiate(meshFinderPrefab);
            _pointerEventData = new PointerEventData(EventSystem.current);
            _results = new List<RaycastResult>();
        }

        private void Update()
        {
            switch (_currentEditor)
            {
                case EditorType.GridEditor:
                    HandleGridModifierTouch();
                    break;

                case EditorType.TextureEditor:
                    HandleTextureModifierTouch();
                    break;
                
                case EditorType.PlacementEditor:
                    HandleUnitPlacementTouch();
                    break;
            }
        }

        public void SetCurrentEditor(EditorType editor) => 
            _currentEditor = editor;

        private void HandleUnitPlacementTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (IsPointerOverUI(touch.position))
                    return;

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Moved)
                {
                    _ray = Camera.main.ScreenPointToRay(touch.position);

                    if (Physics.Raycast(_ray, out RaycastHit hit, _rayDistance, _groundMask))
                    {
                        _brushInstance.transform.position = hit.point;
                        BrushMovedUnit?.Invoke(hit.point, _groundMask);
                    }
                }
            }
        }

        private void HandleGridModifierTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (IsPointerOverUI(touch.position))
                    return;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _isTouching = true;
                        _touchTime = 0f;
                        break;

                    case TouchPhase.Stationary:
                        if (_isTouching)
                        {
                            _touchTime += Time.deltaTime;
                        }

                        if (_touchTime >= _maxTouchDuration)
                        {
                            _ray = Camera.main.ScreenPointToRay(touch.position);

                            if (Physics.Raycast(_ray, out RaycastHit hit, _rayDistance, _groundMask))
                            {
                                _brushInstance.transform.position = hit.point;
                                BrushMovedGrid?.Invoke(hit.point, _groundMask);
                            }

                            _isTouching = false;
                        }

                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        _isTouching = false;
                        _touchTime = 0f;
                        break;
                }
            }
        }
        
        private void HandleTextureModifierTouch()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (IsPointerOverUI(touch.position))
                    return;

                if (touch.phase == TouchPhase.Moved)
                {
                    _ray = Camera.main.ScreenPointToRay(touch.position);

                    if (Physics.Raycast(_ray, out RaycastHit hit, _rayDistance, _groundMask))
                    {
                        _brushInstance.transform.position = hit.point;
                        BrushMovedTexture?.Invoke(hit.point, _groundMask);
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