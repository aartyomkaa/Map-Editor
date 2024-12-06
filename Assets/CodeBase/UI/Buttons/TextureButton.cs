using System;
using UnityEngine;

namespace CodeBase.UI.Buttons
{
    public class TextureButton : BaseButton
    {
        [SerializeField] private Texture2D _texture;

        public event Action<Texture2D> TextureChanged; 
        
        protected override void OnClick()
        {
            TextureChanged?.Invoke(_texture);
        }
    }
}