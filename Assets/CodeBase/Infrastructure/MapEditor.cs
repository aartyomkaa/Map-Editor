using CodeBase.Constants;
using CodeBase.Logic;
using CodeBase.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Grid = CodeBase.Logic.Grid;

namespace CodeBase.Infrastructure
{
    public class MapEditor : MonoBehaviour
    {
        [SerializeField] private Chunk _chunkPrefab;
        [SerializeField] private Editor _editorView;

        private GameObject _chunksParent;
        private Grid _grid;
        private string _selectedMap;

        private void OnEnable() => 
            _editorView.Exited += OnExit;

        private void Awake()
        {
            _chunksParent = new GameObject("Grid");
            _grid = new Grid(_chunkPrefab, _chunksParent);

            _selectedMap = PlayerPrefs.GetString(Preferences.CurrentMap, string.Empty);
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(_selectedMap))
                _grid.GenerateDefaultChunks();
            else
                _grid.LoadGrid(_selectedMap);
        }

        private void OnDisable() => 
            _editorView.Exited -= OnExit;

        private void OnExit()
        {
            _grid.SaveGrid();
            SceneManager.LoadScene("Menu");
        }
    }
}