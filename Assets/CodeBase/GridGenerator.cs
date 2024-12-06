using CodeBase.Constants;
using CodeBase.Data;
using CodeBase.Logic;
using CodeBase.Logic.Chunks;
using UnityEngine;

namespace CodeBase
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private Chunk _chunkPrefab;

        private SaveLoadService _saveLoadService;
        private GridTerrain _gridTerrain;
        private GameObject _chunksParent;
        private string _selectedMap;

        private void Awake()
        {
            _chunksParent = new GameObject("Grid");
            _saveLoadService = new SaveLoadService();
            _gridTerrain = new GridTerrain(_chunkPrefab, _chunksParent);

            _selectedMap = PlayerPrefs.GetString(Preferences.CurrentMap, string.Empty);
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(_selectedMap))
            {
                _gridTerrain.GenerateDefaultChunks();
            }
            else
            {
                SerializedChunk[] chunks = _saveLoadService.LoadMap(_selectedMap);
                _gridTerrain.LoadGrid(chunks);
            }
        }
    }
}