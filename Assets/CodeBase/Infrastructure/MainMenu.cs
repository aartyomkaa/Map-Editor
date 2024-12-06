using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.Infrastructure
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button _startGame;
        [SerializeField] private Button _mapEditor;
        [SerializeField] private MapLoader _loader;

        private void OnEnable()
        {
            _mapEditor.onClick.AddListener(OpenMapLoaderEditorMode);
            _startGame.onClick.AddListener(OpenMapLoaderGameMode);
        }

        private void OnDisable()
        {
            _mapEditor.onClick.RemoveListener(OpenMapLoaderEditorMode);
            _startGame.onClick.RemoveListener(OpenMapLoaderGameMode);
        }

        private void OpenMapLoaderGameMode()
        {
            CloseButtons();
            
            _loader.gameObject.SetActive(true);
            _loader.Init(false);
        }

        private void OpenMapLoaderEditorMode()
        {
            CloseButtons();
            
            _loader.gameObject.SetActive(true);
            _loader.Init(true);
        }

        private void CloseButtons()
        {
            _startGame.gameObject.SetActive(false);
            _mapEditor.gameObject.SetActive(false);
        }
    }
}