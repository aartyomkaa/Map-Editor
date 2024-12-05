using System;
using CodeBase.InputLogic;
using CodeBase.Logic;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class Editor : MonoBehaviour
    {
        [SerializeField] private TouchInput _input;
        [SerializeField] private Button _raise;
        [SerializeField] private Button _lower;
        [SerializeField] private Slider _area;
        [SerializeField] private Slider _strenght;
        [SerializeField] private Slider _intensity;
        [SerializeField] private Slider _smoothness;
        [SerializeField] private Button _exit;
        
        private GridModifier _gridModifier;
        
        public event Action Exited;

        private void OnEnable()
        {
            _raise.onClick.AddListener(RaiseTerrain);
            _lower.onClick.AddListener(LowerTerrain);
            _exit.onClick.AddListener(OnExit);

            _area.onValueChanged.AddListener(SetModifierArea);
            _strenght.onValueChanged.AddListener(SetModifierStrength);
            _intensity.onValueChanged.AddListener(SetModifierIntensity);
            _smoothness.onValueChanged.AddListener(SetModifierSmoothness);

            _input.BrushMoved += SetChunks;
        }

        private void Start() => 
            _gridModifier = new GridModifier();

        private void OnDisable()
        {
            _raise.onClick.RemoveListener(RaiseTerrain);
            _lower.onClick.RemoveListener(LowerTerrain);
            _exit.onClick.RemoveListener(OnExit);
            
            _area.onValueChanged.RemoveListener(SetModifierArea);
            _strenght.onValueChanged.RemoveListener(SetModifierStrength);
            _intensity.onValueChanged.RemoveListener(SetModifierIntensity);
            _smoothness.onValueChanged.RemoveListener(SetModifierSmoothness);
            
            _input.BrushMoved -= SetChunks;
        }

        private void SetChunks(Vector3 hitPosition, LayerMask groundLayer) => 
            _gridModifier.SetChunks(hitPosition, groundLayer);

        private void SetModifierArea(float value) => 
            _gridModifier.AdjustArea(value);

        private void SetModifierStrength(float value) => 
            _gridModifier.AdjustStrength(value);

        private void SetModifierIntensity(float value) => 
            _gridModifier.AdjustIntensity(value);

        private void SetModifierSmoothness(float value) => 
            _gridModifier.AdjustSmoothness(value);

        private void RaiseTerrain() => 
            _gridModifier.RaiseTerrain(_input.GetBrushPosition());

        private void LowerTerrain() => 
            _gridModifier.LowerTerrain(_input.GetBrushPosition());
        
        private void OnExit() => 
            Exited?.Invoke();
    }
}