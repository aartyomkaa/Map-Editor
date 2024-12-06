using System.Collections.Generic;
using CodeBase.Constants;
using CodeBase.UI.Buttons;
using CodeBase.Units;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Editors
{
    public class UnitPlacementEditor : EditorBase
    {
        [SerializeField] private Button _place;
        [SerializeField] private List<UnitButton> _unitButtons;
        [SerializeField] private Unit _currentUnit;

        private Unit _unitView;
        private List<Unit> _placedUnits;

        protected override void OnInitialize()
        {
            _placedUnits = new List<Unit>();
            
            _place.onClick.AddListener(OnPlaceUnit);
            Input.BrushMovedUnit += OnBrushMovedTexture;

            foreach (UnitButton unitButton in _unitButtons)
            {
                unitButton.UnitChanged += OnChangeUnit;
            }

            InitView();
        }

        public override void Cleanup()
        {
            _place.onClick.RemoveListener(OnPlaceUnit);
            Input.BrushMovedUnit -= OnBrushMovedTexture;

            foreach (UnitButton unitButton in _unitButtons)
            {
                unitButton.UnitChanged -= OnChangeUnit;
            }
        }

        public void SetUnits(List<Unit> units) => 
            _placedUnits = units;
        
        public List<Unit> GetUnits() => 
            _placedUnits;

        private void OnBrushMovedTexture(Vector3 brushPosition, LayerMask groundMask)
        {
            _unitView.transform.position = brushPosition;
        }

        private void OnPlaceUnit()
        {
            Unit instance = Instantiate(_currentUnit, _unitView.transform.position, Quaternion.identity);
            _placedUnits.Add(instance);
        }

        private void OnChangeUnit(Unit unit)
        {
            _currentUnit = unit;
            
            InitView();
        }

        private void InitView()
        {
            if (_unitView != null)
                Destroy(_unitView.gameObject);
            
            _unitView = Instantiate(_currentUnit);
            _unitView.gameObject.SetActive(true);
        }

        public override void OnMainEditorSet() => 
            _unitView.gameObject.SetActive(true);
    }
}