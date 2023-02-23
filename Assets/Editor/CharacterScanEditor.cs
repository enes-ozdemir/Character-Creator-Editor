using System.Collections.Generic;
using _3_Scripts.SO;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

namespace Editor
{
    public class CharacterScanEditor : EditorWindow
    {
        private const string PrefabName = "Char";
        private static StoreItemContainer _storeItemContainer;
        private static List<GameObject> unUsedPrefabs = new List<GameObject>();

        public static void DrawGUI()
        {
            CreateSection();
        }

        private static void CreateSection()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Label("Scan the Project", EditorStyles.boldLabel);

                if (GUILayout.Button("Scan"))
                {
                    ScanProject();
                }

                DisplayUnUsedPrefabs();
            }
        }

        private static void DisplayUnUsedPrefabs()
        {
            if (unUsedPrefabs.Count <= 0) return;
            EditorGUILayout.LabelField("Unused prefabs:", EditorStyles.boldLabel);

            for (int i = 0; i < unUsedPrefabs.Count; i++)
            {
                var prefab = unUsedPrefabs[i];

                using (new GUILayout.HorizontalScope())
                {
                    var unUsedPrefab = EditorGUILayout.ObjectField("Unused prefab: " + i, prefab,
                            typeof(GameObject), false) as
                        GameObject;

                    if (GUILayout.Button("Create"))
                    {
                       CharacterCreatorEditor.onTabChanged.Invoke(0);
                       CharacterCreateSection.SetPrefab(unUsedPrefab);
                    }
                }
            }
        }

        private static void ScanProject()
        {
            unUsedPrefabs.Clear();
            var charPrefabList = GetPrefabStartsWith(PrefabName);
            unUsedPrefabs = GetUnUsedPrefabs(charPrefabList);
        }

        private static List<GameObject> GetUnUsedPrefabs(List<GameObject> charPrefabList)
        {
            for (int i = charPrefabList.Count - 1; i >= 0; i--)
            {
                foreach (var storeItem in _storeItemContainer.storeItemList)
                {
                    if (charPrefabList[i].name == storeItem.Prefab.name)
                    {
                        Debug.Log($"Exists {charPrefabList[i].name}");
                        charPrefabList.RemoveAt(i);
                        break;
                    }
                }
            }

            Debug.Log($"There are {charPrefabList.Count} unused prefabs");
            return charPrefabList;
        }


        private static List<GameObject> GetPrefabStartsWith(string prefix)
        {
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            var usedPrefabList = new List<GameObject>();


            foreach (string guid in prefabGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (assetPath.StartsWith("Assets") && assetPath.EndsWith(".prefab"))
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    if (prefab.name.StartsWith(prefix))
                    {
                        Debug.Log("Found prefab: " + prefab.name + " at path: " + assetPath);
                        usedPrefabList.Add(prefab);
                    }
                }
            }

            return usedPrefabList;
        }


        public static void SetStoreItemContainer(StoreItemContainer container)
        {
            _storeItemContainer = container;
        }
    }
}