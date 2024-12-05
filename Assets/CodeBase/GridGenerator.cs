using CodeBase.Constants;
using CodeBase.Logic;
using UnityEngine;
using Grid = CodeBase.Logic.Grid;

namespace CodeBase
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private Chunk _chunkPrefab;

        private Grid _grid;
        private GameObject _chunksParent;
        private string _selectedMap;

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
    }
}