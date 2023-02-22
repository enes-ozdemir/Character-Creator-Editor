using System;
using _3_Scripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CharacterCreatorEditor : EditorWindow
    {
        private StoreItem _tempSelectedProduct = new StoreItem();
        private bool _isAutoIncrement;
        private bool isPrefabChanged;
        private bool _autoCreateSprite;
        public StoreItemContainer storeItemContainer;

        [MenuItem("Tools/Character Creator")]
        public static void OpenWindow()
        {
            var window = GetWindow<CharacterCreatorEditor>();
            window.titleContent = new GUIContent("Character Creator");
            window.Show();
        }

        private void OnEnable()
        {
            GetStoreItemContainer();
        }

        private void GetStoreItemContainer()
        {
            string[] storeItemGuids = AssetDatabase.FindAssets("t:" + nameof(StoreItemContainer));
            string path = AssetDatabase.GUIDToAssetPath(storeItemGuids[0]);
            storeItemContainer = AssetDatabase.LoadAssetAtPath<StoreItemContainer>(path);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Character Data", EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawCharacterData();
                DrawPrefabSettings();
                DrawSpriteSettings();
                DrawIDSettings();

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Create"))
                    {
                        if(!IsFieldsAreValid()) return;
                        CreateNewItem();
                    }
                }

                
            }
        }

        private void CreateNewItem()
        {
        }

        private bool IsFieldsAreValid()
        {
            if (_tempSelectedProduct.Name == null || _tempSelectedProduct.Name.Trim() == "")
            {
                EditorUtility.DisplayDialog("Error", "Name cannot be empty", "OK");
                Debug.LogError("Name cannot be empty");
                return false;
            }

            if (_tempSelectedProduct.Prefab == null)
            {
                EditorUtility.DisplayDialog("Error", "Prefab cannot be null", "OK");
                Debug.LogError("Prefab cannot be null");
                return false;
            }

            if (_tempSelectedProduct.Icon == null)
            {
                EditorUtility.DisplayDialog("Error", "Icon cannot be null", "OK");
                Debug.LogError("Icon cannot be null");
                return false;
            }

            return true;
        }

        private void DrawIDSettings()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _isAutoIncrement = EditorGUILayout.Toggle("Auto Increment ID", _isAutoIncrement);

                using (new EditorGUI.DisabledScope(_isAutoIncrement))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("ID", GUILayout.Width(50f));
                        var nextID = storeItemContainer.storeItemList.Count;
                        _tempSelectedProduct.Id = EditorGUILayout.IntField(nextID);
                    }
                }
            }
        }

        private void DrawSpriteSettings()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _autoCreateSprite = EditorGUILayout.Toggle("Automatically Create Sprite", _autoCreateSprite);

                using (new EditorGUI.DisabledScope(_autoCreateSprite))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        _tempSelectedProduct.Icon =
                            EditorGUILayout.ObjectField("Icon", _tempSelectedProduct.Icon, typeof(Sprite), false) as
                                Sprite;
                        Repaint();
                    }
                }

                if (_autoCreateSprite && isPrefabChanged)
                {
                    _tempSelectedProduct.Icon =
                        ConvertToSpriteExtension.ConvertToSprite(_tempSelectedProduct.Prefab);
                }
            }
        }

        private void DrawPrefabSettings()
        {
            using (new GUILayout.HorizontalScope())
            {
                var previousPrefab = _tempSelectedProduct.Prefab; // Store previous value of prefab
                _tempSelectedProduct.Prefab =
                    EditorGUILayout.ObjectField("Prefab", _tempSelectedProduct.Prefab, typeof(GameObject), false) as
                        GameObject;

                // Check if prefab has changed
                isPrefabChanged = _tempSelectedProduct.Prefab != previousPrefab;
            }
        }

        private void DrawCharacterData()
        {
            using (new GUILayout.HorizontalScope())
            {
                _tempSelectedProduct.Name = EditorGUILayout.TextField("Name", _tempSelectedProduct.Name);
            }

            using (new GUILayout.HorizontalScope())
            {
                _tempSelectedProduct.Price = EditorGUILayout.IntField("Price", _tempSelectedProduct.Price);
            }
        }
    }
}