using _3_Scripts;
using _3_Scripts.SO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CharacterCreatorEditor : EditorWindow
    {
        private string[] _tabs = {"Create Item", "Edit Item", "Scan Project"};
        private int _tabSelected = 0;
        
        [MenuItem("Tools/Character Creator")]
        public static void OpenWindow()
        {
            var window = GetWindow<CharacterCreatorEditor>();
            window.titleContent = new GUIContent("Character Creator");
            window.Show();
        }

        private void OnEnable()
        {
            var container = GetStoreItemContainer();
            CharacterCreateSection.SetStoreItemContainer(container);
            var prefabConfigSettings = GetPrefabConfigSettings();
            CharacterCreateSection.SetPrefabConfigSettings(prefabConfigSettings);

        }

        private PrefabConfigSettings GetPrefabConfigSettings()
        {
            string[] storeItemGuids = AssetDatabase.FindAssets("t:" + nameof(PrefabConfigSettings));
            string path = AssetDatabase.GUIDToAssetPath(storeItemGuids[0]);
            var storeItemContainer = AssetDatabase.LoadAssetAtPath<PrefabConfigSettings>(path);
            return storeItemContainer;
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

                    break;
                case "Scan Project":

                    break;
            }
        }
    }
}