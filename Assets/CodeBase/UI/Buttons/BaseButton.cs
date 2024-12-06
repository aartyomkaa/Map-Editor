using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI.Buttons
{
    public abstract class BaseButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void OnEnable() => 
            _button.onClick.AddListener(OnClick);

        private void OnDisable() => 
            _button.onClick.RemoveListener(OnClick);

        protected abstract void OnClick();
    }
}