using _3_Scripts;
using UnityEditor;
using UnityEngine;
using System.IO;
using _3_Scripts.SO;
using UnityEditor.Animations;
using Object = UnityEngine.Object;

namespace Editor
{
    public static class CharacterCreateSection
    {
        private static StoreItemContainer _storeItemContainer;
        private static StoreItem _tempSelectedProduct = new StoreItem();
        private static bool _isAutoIncrement;
        private static bool _isPrefabChanged;
        private static bool _autoCreateSprite;
        private static bool _isOptimizeForMobile = true;


        private const string PrefabPath = "Assets/2_Prefabs/";
        private const string FbxPath = "Assets/1_Graphics/Models/";

        #region AdditionalConfigSection

        private static bool _showConfig;
        private static PrefabConfigSettings _prefabConfigSettings;

        #endregion

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
                DrawConfigurationSettings();

                _isOptimizeForMobile = EditorGUILayout.Toggle("Optimize for Mobile", _isOptimizeForMobile);

                if (GUILayout.Button("Create"))
                {
                    if (!IsFieldsAreValid()) return;
                    AddNewItemToContainer();
                }
            }
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
                    var fbx = ImportFBXFromPath(path);

                    CreatePrefabForCharacter(_tempSelectedProduct, fbx);
                }
            }
        }


        private static void DrawCharacterData()
        {
            using (new GUILayout.VerticalScope())
            {
                _tempSelectedProduct.Name = EditorGUILayout.TextField("Name", _tempSelectedProduct.Name);
                _tempSelectedProduct.Price = EditorGUILayout.IntField("Price", _tempSelectedProduct.Price);
            }
        }

        private static void DrawConfigurationSettings()
        {
            _showConfig = EditorGUILayout.Foldout(_showConfig, "Additional Configuration Settings");

            if (_showConfig)
            {
                using (new GUILayout.VerticalScope())
                {
                    _prefabConfigSettings.animator =
                        EditorGUILayout.ObjectField("Animator", _prefabConfigSettings.animator,
                            typeof(AnimatorController), false) as AnimatorController;
                    _prefabConfigSettings.colliderRadius =
                        EditorGUILayout.FloatField("Collider Radius", _prefabConfigSettings.colliderRadius);
                    _prefabConfigSettings.colliderHeight =
                        EditorGUILayout.FloatField("Collider Height", _prefabConfigSettings.colliderHeight);
                }
            }
        }

        #endregion

        private static GameObject ImportFBXFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Path can't be empty");
                return null;
            }

            string destinationPath = FbxPath + Path.GetFileName(path);

            if (File.Exists(destinationPath))
            {
                Debug.LogWarning($"File {destinationPath} already imported");
                return AssetDatabase.LoadAssetAtPath<GameObject>(destinationPath);
            }

            FileUtil.CopyFileOrDirectory(path, destinationPath);
            AssetDatabase.Refresh();

            var modelImporter = AssetImporter.GetAtPath(destinationPath) as ModelImporter;
            SetDefaultModelSettings(modelImporter);

            var fbxGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(destinationPath);

            return fbxGameObject;
        }

        private static void AddNewItemToContainer()
        {
            _storeItemContainer.storeItemList.Add(_tempSelectedProduct);
        }

        private static void CreatePrefabForCharacter(StoreItem storeItem, Object model)
        {
            CheckIfPrefabAlreadyExists(storeItem);

            var prefab = (GameObject) PrefabUtility.InstantiatePrefab(model);

            AddCollider(prefab);
            AddAnimator(prefab);

            PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            storeItem.Prefab = PrefabUtility.SaveAsPrefabAsset(prefab, $"{PrefabPath}{prefab.name}.prefab");
        }

        private static void CheckIfPrefabAlreadyExists(StoreItem storeItem)
        {
            if (storeItem.Prefab)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(storeItem.Prefab));
                storeItem.Prefab = null;
            }
        }

        private static void AddAnimator(GameObject prefab)
        {
            var animator = prefab.AddComponent<Animator>();
            if (_prefabConfigSettings.animator != null)
            {
                animator.runtimeAnimatorController = _prefabConfigSettings.animator;
            }

            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        private static void AddCollider(GameObject prefab)
        {
            prefab.AddComponent<CapsuleCollider>();
        }

        private static void SetDefaultModelSettings(ModelImporter importer)
        {
            importer.animationType = ModelImporterAnimationType.Human;
            importer.importAnimation = false;
            importer.importConstraints = false;

            importer.SaveAndReimport();
        }


        public static void SetStoreItemContainer(StoreItemContainer container)
        {
            _storeItemContainer = container;
        }

        public static void SetPrefabConfigSettings(PrefabConfigSettings prefabConfigSettings)
        {
            _prefabConfigSettings = prefabConfigSettings;
        }
    }
}