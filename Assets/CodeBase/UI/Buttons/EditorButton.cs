using System;
using CodeBase.Logic;
using UnityEngine;

namespace CodeBase.UI.Buttons
{
    public class EditorButton : BaseButton
    {
        [SerializeField] private EditorType _editorType;

        public event Action<EditorType> EditorTypeChanged; 
        
        protected override void OnClick() => 
            EditorTypeChanged?.Invoke(_editorType);
    }
}