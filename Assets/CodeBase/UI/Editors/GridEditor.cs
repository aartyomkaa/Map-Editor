using System.Collections.Generic;
using CodeBase.Logic;
using CodeBase.Logic.Chunks;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Editors
{
    public class GridEditor : EditorBase
    {
        [SerializeField] private Button _raise;
        [SerializeField] private Button _lower;
        [SerializeField] private Slider _area;
        [SerializeField] private Slider _strenght;
        [SerializeField] private Slider _smoothness;

        private GridTerrain _gridTerrain;
        
        protected override void OnInitialize()
        {
            _raise.onClick.AddListener(RaiseTerrain);
            _lower.onClick.AddListener(LowerTerrain);

            _area.onValueChanged.AddListener(SetModifierArea);
            _strenght.onValueChanged.AddListener(SetModifierStrength);
            _smoothness.onValueChanged.AddListener(SetModifierSmoothness);

            Input.BrushMovedGrid += SetChunks;
        }

        public Dictionary<Vector2Int, Chunk> GetGrid() => 
            _gridTerrain.GetGrid();

        public override void Cleanup()
        {
            _raise.onClick.RemoveListener(RaiseTerrain);
            _lower.onClick.RemoveListener(LowerTerrain);
            
            _area.onValueChanged.RemoveListener(SetModifierArea);
            _strenght.onValueChanged.RemoveListener(SetModifierStrength);
            _smoothness.onValueChanged.RemoveListener(SetModifierSmoothness);
            
            Input.BrushMovedGrid -= SetChunks;
        }

        public void SetGrid(GridTerrain grid) => 
            _gridTerrain = grid;

        private void SetChunks(Vector3 hitPosition, LayerMask groundLayer) => 
            GridModifier.SetChunks(hitPosition, groundLayer);

        private void SetModifierArea(float value) => 
            GridModifier.AdjustArea(value);

        private void SetModifierStrength(float value) => 
            GridModifier.AdjustStrength(value);

        private void SetModifierSmoothness(float value) => 
            GridModifier.AdjustSmoothness(value);

        private void RaiseTerrain() => 
            GridModifier.RaiseTerrain();

        private void LowerTerrain() => 
            GridModifier.LowerTerrain();
    }
}