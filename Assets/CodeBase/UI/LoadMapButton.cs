using System;
using CodeBase.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class LoadMapButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _name;
        
        private string _filePath;
        private bool _isEditor;

        private void OnEnable() => 
            _button.onClick.AddListener(OnLoadMap);

        private void OnDisable() => 
            _button.onClick.RemoveListener(OnLoadMap);

        public void Construct(string buttonName, string jsonPath, bool isEditor)
        {
            _name.text = buttonName;
            _filePath = jsonPath;
            _isEditor = isEditor;
        }

        private void OnLoadMap()
        {
            PlayerPrefs.SetString(Preferences.CurrentMap, _filePath);
            
            SceneManager.LoadScene(_isEditor ? "MapEditor" : "Game");
        }
    }
}