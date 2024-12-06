using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase.Constants;
using CodeBase.UI.Buttons;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CodeBase.Infrastructure
{
    public class MapLoader : MonoBehaviour
    {
        [SerializeField] private LoadMapButton _buttonPrefab;
        [SerializeField] private GameObject _content;
        [SerializeField] private Button _deleteSaves;
        [SerializeField] private Button _newMap;

        private List<LoadMapButton> _maps = new List<LoadMapButton>();
        private List<string> _filePaths;

        private void OnEnable()
        {
            _deleteSaves.onClick.AddListener(DeleteAllSavedData);
            _newMap.onClick.AddListener(OpenMapEditor);
        }
        
        private void OnDisable()
        {
            _deleteSaves.onClick.AddListener(DeleteAllSavedData);
            _newMap.onClick.RemoveListener(OpenMapEditor);
        }

        public void Init(bool isEditor)
        {
            _filePaths = LoadFilePaths();
            
            foreach (string filePath in _filePaths)
            {
                if (File.Exists(filePath))
                {
                    string fileName = Path.GetFileName(filePath);

                    LoadMapButton instance = Instantiate(_buttonPrefab, _content.transform);

                    instance.Construct(fileName, filePath, isEditor);
                    _maps.Add(instance);
                }
            }
        }

        private void DeleteAllSavedData()
        {
            string[] jsonFiles = Directory.GetFiles(
                Application.persistentDataPath,
                "SavedMap_*.json",
                SearchOption.TopDirectoryOnly);
            
            foreach (var file in jsonFiles)
            {
                File.Delete(file);
            }

            foreach (LoadMapButton button in _maps)
            {
                Destroy(button.gameObject);
            }

            PlayerPrefs.DeleteKey(Preferences.SavedPaths);
            PlayerPrefs.Save();
        }

        private List<string> LoadFilePaths()
        {
            string savedPaths = PlayerPrefs.GetString(Preferences.SavedPaths, string.Empty);
            List<string> filePaths = new List<string>();

            if (string.IsNullOrEmpty(savedPaths) == false)
            {
                filePaths.AddRange(savedPaths.Split(','));
            }
            
            filePaths = filePaths.Distinct().ToList();

            return filePaths;
        }

        private void OpenMapEditor()
        {
            PlayerPrefs.SetString(Preferences.CurrentMap, string.Empty);
            SceneManager.LoadScene("MapEditor");
        }
    }
}