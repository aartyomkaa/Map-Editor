using CodeBase.Constants;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.UI.Buttons
{
    public class LoadMapButton : BaseButton
    {
        [SerializeField] private TMP_Text _name;
        
        private string _filePath;
        private bool _isEditor;
        private int _json = 5;

        public void Construct(string buttonName, string jsonPath, bool isEditor)
        {
            string newName = buttonName.Substring(0, buttonName.Length - _json);
            
            _name.text = newName;
            _filePath = jsonPath;
            _isEditor = isEditor;
        }

        protected override void OnClick() => 
            LoadMap();

        private void LoadMap()
        {
            PlayerPrefs.SetString(Preferences.CurrentMap, _filePath);
            
            SceneManager.LoadScene(_isEditor ? "MapEditor" : "Game");
        }
    }
}