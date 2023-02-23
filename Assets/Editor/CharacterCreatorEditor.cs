using System;
using _3_Scripts.SO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CharacterCreatorEditor : EditorWindow
    {
        private const string CreateCharacterTab = "Create Character";
        private const string EditCharacterTab = "Edit Character";
        private const string ScanProjectTab = "Scan Project";
        private string[] _tabs = {CreateCharacterTab, EditCharacterTab, ScanProjectTab};
        private int _tabSelected;
        private StoreItemContainer _storeItemContainer;

        public static Action<int> onTabChanged;

        [MenuItem("Tools/Character Creator")]
        public static void OpenWindow()
        {
            var window = GetWindow<CharacterCreatorEditor>();
            window.titleContent = new GUIContent("Character Creator Editor");
            window.Show();
        }

        private void OnEnable()
        {
            onTabChanged += OpenTab;
            _storeItemContainer = GetStoreItemContainer();
            
            InitializeTabs();
        }
        
        private void OnGUI()
        {
            GUILayout.Space(20);
            _tabSelected = GUILayout.Toolbar(_tabSelected, _tabs);

            CreateTab();
        }

        private void InitializeTabs()
        {
            InitCreateSection();
            InitStoreSection();
            InitSearchSection();
        }

        private void CreateTab()
        {
            switch (_tabs[_tabSelected])
            {
                case CreateCharacterTab:
                    CharacterCreateSection.DrawGUI();
                    break;
                case EditCharacterTab:
                    CharacterEditSection.DrawGUI();
                    break;
                case ScanProjectTab:
                    CharacterScanSection.DrawGUI();
                    break;
            }
        }

        private void OnDisable() => onTabChanged += OpenTab;

        private void InitSearchSection() => CharacterScanSection.SetStoreItemContainer(_storeItemContainer);

        private void InitStoreSection() => CharacterEditSection.SetStoreItemContainer(_storeItemContainer);

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

        private void OpenTab(int newTabIndex) => _tabSelected = newTabIndex;
    }
}