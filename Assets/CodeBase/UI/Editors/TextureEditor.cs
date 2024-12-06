using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Editors
{
    public class TextureEditor : EditorBase
    {
        [SerializeField] private Slider _area;
        [SerializeField] private Slider _blendingFactor;
        
        protected override void OnInitialize()
        {
            _area.onValueChanged.AddListener(SetModifierArea);

            Input.BrushMovedTexture += Modify;
        }

        public override void Cleanup()
        {
            _area.onValueChanged.RemoveListener(SetModifierArea);
            
            Input.BrushMovedTexture -= Modify;
        }

        private void Modify(Vector3 hitPosition, LayerMask groundLayer)
        {
            GridModifier.SetChunks(hitPosition, groundLayer);
            GridModifier.ApplyTexture(_blendingFactor.value);
        }

        private void SetModifierArea(float value) => 
            GridModifier.AdjustArea(value);
    }
}