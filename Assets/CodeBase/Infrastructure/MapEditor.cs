using System.Collections.Generic;
using CodeBase.Constants;
using CodeBase.Data;
using CodeBase.InputLogic;
using CodeBase.Logic;
using CodeBase.Logic.Chunks;
using CodeBase.UI.Buttons;
using CodeBase.UI.Editors;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CodeBase.Infrastructure
{
    public class MapEditor : MonoBehaviour
    {
        [SerializeField] private TouchInput _input;
        [SerializeField] private Chunk _chunkPrefab;
        [SerializeField] private Button _exit;
        [SerializeField] private GridEditor _gridEditor;
        [SerializeField] private TextureEditor _textureEditor;
        [SerializeField] private UnitPlacementEditor _unitPlacementEditor;
        [SerializeField] private List<EditorButton> _editorButtons;

        private List<EditorBase> _editors = new List<EditorBase>();
        private GameObject _chunksParent;
        private GridTerrain _gridTerrain;
        private SaveLoadService _saveLoadService;
        private string _selectedMap;

        private void OnEnable()
        {
            _exit.onClick.AddListener(OnSaveExit);

            foreach (EditorButton editorButton in _editorButtons)
            {
                editorButton.EditorTypeChanged += ChangeEditorType;
            }
        }

        private void Awake()
        {
            _chunksParent = new GameObject("Grid");
            _gridTerrain = new GridTerrain(_chunkPrefab, _chunksParent);

            _selectedMap = PlayerPrefs.GetString(Preferences.CurrentMap, string.Empty);

            _saveLoadService = new SaveLoadService();
            GridModifier gridModifier = new GridModifier();
            _gridEditor.SetGrid(_gridTerrain);
            
            _editors.Add(_gridEditor);
            _editors.Add(_textureEditor);
            _editors.Add(_unitPlacementEditor);
            
            foreach (EditorBase editor in _editors)
            {
                editor.Construct(gridModifier, _input);
            }
            
            ChangeEditorType(EditorType.GridEditor);
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

        private void OnDisable()
        {
            _exit.onClick.RemoveListener(OnSaveExit);
            
            foreach (EditorButton editorButton in _editorButtons)
            {
                editorButton.EditorTypeChanged -= ChangeEditorType;
            }
            
            foreach (EditorBase editor in _editors)
            {
                editor.Cleanup();
            }
        }

        private void ChangeEditorType(EditorType type)
        {
            foreach (EditorBase editor in _editors)
            {
                editor.gameObject.SetActive(false);
                
                if (editor.Type == type)
                {
                    editor.gameObject.SetActive(true);
                    editor.OnMainEditorSet();
                }
            }
            
            _input.SetCurrentEditor(type);
        }

        private void OnSaveExit()
        {
            _saveLoadService.SaveMap(_gridEditor.GetGrid(), _unitPlacementEditor.GetUnits());
            
            SceneManager.LoadScene("Menu");
        }
    }
}