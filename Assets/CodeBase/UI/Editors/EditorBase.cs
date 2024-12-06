using CodeBase.InputLogic;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.UI.Editors
{
    public abstract class EditorBase : MonoBehaviour
    {
        [SerializeField] private EditorType _editorType;
        
        private TouchInput _input;
        private GridModifier _gridModifier;
        private SaveLoadService _saveLoadService;

        protected GridModifier GridModifier => _gridModifier;
        protected TouchInput Input => _input;

        public EditorType Type => _editorType;
        
        public void Construct(GridModifier gridModifier, TouchInput input)
        {
            _gridModifier = gridModifier;
            _input = input;
            
            OnInitialize();
        }

        public virtual void Cleanup(){}

        public virtual void OnMainEditorSet(){}

        public virtual void SaveChanges(){}
        
        protected virtual void OnInitialize(){}
    }
}