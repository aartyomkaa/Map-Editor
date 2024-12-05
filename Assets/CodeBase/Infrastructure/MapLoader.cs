using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeBase.Constants;
using CodeBase.UI;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class MapLoader : MonoBehaviour
    {
        [SerializeField] private LoadMapButton _buttonPrefab;
        [SerializeField] private GameObject _content;

        private List<string> _filePaths;

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
                }
                else
                {
                    Debug.LogWarning("File not found: " + filePath);
                }
            }
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
    }
}