using _3_Scripts;
using _3_Scripts.SO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CharacterCreatorEditor : EditorWindow
    {
        private string[] _tabs = {"Create Item", "Edit Item", "Scan Project", "Edit Prefab Config"};
        private int _tabSelected = 0;
        private static SerializedObject _so;
        private static SerializedProperty _propList;
        private StoreItemContainer _storeItemContainer;

        [MenuItem("Tools/Character Creator")]
        public static void OpenWindow()
        {
            var window = GetWindow<CharacterCreatorEditor>();
            window.titleContent = new GUIContent("Character Creator");
            window.Show();
        }

        private void OnEnable()
        {
            _storeItemContainer = GetStoreItemContainer();

            InitCreateSection();
            InitStoreSection();
        }

        private void InitStoreSection()
        {
            StoreItemEditor.SetStoreItemContainer(_storeItemContainer);

            _so = new SerializedObject(_storeItemContainer);
            _propList = _so.FindProperty("storeItemList");
            StoreItemEditor.SetSerializedObjects(_so, _propList);
        }

        private void InitCreateSection()
        {
            CharacterCreateSection.SetStoreItemContainer(_storeItemContainer);
            var prefabConfigSettings = GetPrefabConfigSettings();
            CharacterCreateSection.SetPrefabConfigSettings(prefabConfigSettings);
            CharacterCreateSection.SetTempConfigSettings();
        }

        private PrefabConfigSettings GetPrefabConfigSettings()
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:" + nameof(PrefabConfigSettings));
            string path = AssetDatabase.GUIDToAssetPath(prefabGuids[0]);
            var prefabConfigSettings = AssetDatabase.LoadAssetAtPath<PrefabConfigSettings>(path);
            return prefabConfigSettings;
        }

        private StoreItemContainer GetStoreItemContainer()
        {
            string[] storeItemGuids = AssetDatabase.FindAssets("t:" + nameof(StoreItemContainer));
            string path = AssetDatabase.GUIDToAssetPath(storeItemGuids[0]);
            var storeItemContainer = AssetDatabase.LoadAssetAtPath<StoreItemContainer>(path);
            return storeItemContainer;
        }

        private void OnGUI()
        {
            GUILayout.Space(20);
            _tabSelected = GUILayout.Toolbar(_tabSelected, _tabs);

            switch (_tabs[_tabSelected])
            {
                case "Create Item":
                    CharacterCreateSection.DrawGUI();
                    break;
                case "Edit Item":
                    StoreItemEditor.DrawGUI();
                    break;
                case "Scan Project":

                    break;
                case "Edit Prefab Config":

                    break;
            }
        }
    }
}