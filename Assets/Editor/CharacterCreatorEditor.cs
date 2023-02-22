using _3_Scripts;
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