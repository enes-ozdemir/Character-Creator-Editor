using _3_Scripts;
using UnityEditor;
using UnityEngine;
using System.IO;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class CharacterCreateSection
    {
        private static bool _isAutoIncrement;
        private static bool _isPrefabChanged;
        private static bool _autoCreateSprite;
        private static StoreItemContainer _storeItemContainer;
        private static StoreItem _tempSelectedProduct = new StoreItem();

        private const string PrefabPath = "Assets/2_Prefabs/";
        private const string FbxPath = "Assets/1_Graphics/Models/";

        public static void DrawGUI()
        {
            CreateSection();
        }

        #region CreateItemUISection

        private static void CreateSection()
        {
            GUILayout.Space(10);
            GUILayout.Label("Character Data", EditorStyles.boldLabel);

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawCharacterData();
                DrawPrefabSettings();
                DrawSpriteSettings();
                DrawIDSettings();

                if (GUILayout.Button("Create"))
                {
                    if (!IsFieldsAreValid()) return;
                    AddNewItemToContainer();
                }
            }
        }

        private static void AddNewItemToContainer()
        {
            _storeItemContainer.storeItemList.Add(_tempSelectedProduct);
            CreatePrefabForItem();
        }

        private static void CreatePrefabForItem()
        {
            var path = PrefabPath + _tempSelectedProduct.Name + ".prefab";
            var prefab = PrefabUtility.SaveAsPrefabAsset(_tempSelectedProduct.Prefab, path);

            prefab.name = _tempSelectedProduct.Name;
            AssetDatabase.SaveAssets();

            _tempSelectedProduct.Prefab = prefab;
        }

        private static bool IsFieldsAreValid()
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

        private static void DrawIDSettings()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _isAutoIncrement = EditorGUILayout.Toggle("Auto Increment ID", _isAutoIncrement);

                using (new EditorGUI.DisabledScope(_isAutoIncrement))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("ID", GUILayout.Width(50f));
                        var nextID = _storeItemContainer.storeItemList.Count;
                        _tempSelectedProduct.Id = EditorGUILayout.IntField(nextID);
                    }
                }
            }
        }

        private static void DrawSpriteSettings()
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
                        //Repaint();
                    }
                }

                if (_autoCreateSprite && _isPrefabChanged)
                {
                    _tempSelectedProduct.Icon =
                        ConvertToSpriteExtension.ConvertToSprite(_tempSelectedProduct.Prefab);
                }
            }
        }

        private static void DrawPrefabSettings()
        {
            using (new GUILayout.HorizontalScope())
            {
                var previousPrefab = _tempSelectedProduct.Prefab;
                _tempSelectedProduct.Prefab =
                    EditorGUILayout.ObjectField("Prefab", _tempSelectedProduct.Prefab, typeof(GameObject), false) as
                        GameObject;

                _isPrefabChanged = _tempSelectedProduct.Prefab != previousPrefab;

                if (GUILayout.Button("Select FBX"))
                {
                    string path = EditorUtility.OpenFilePanel("Select FBX", "", "fbx");
                }
            }
        }


        private static void DrawCharacterData()
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

        #endregion
        
        public static void SetStoreItemContainer(StoreItemContainer container)
        {
            _storeItemContainer = container;
        }
    }
}