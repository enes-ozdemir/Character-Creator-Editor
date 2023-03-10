using UnityEditor;
using UnityEngine;
using System.IO;
using _3_Scripts.SO;
using UnityEditor.Animations;

namespace Editor
{
    public class CharacterCreateSection : EditorWindow
    {
        private static StoreItemContainer _storeItemContainer;
        private static StoreItem _tempSelectedProduct = new StoreItem();
        private static bool _isFbxSelected;
        private static bool _isOptimizeForMobileEnabled = true;
        private static string _selectedFbxPath;

        private const string PrefabPath = "Assets/2_Prefabs/";
        private const string FbxPath = "Assets/1_Graphics/Models/";

        #region AdditionalConfigSection

        private static bool _showConfig;
        private static PrefabConfigSettings _prefabConfigSettings;
        private static AnimatorController _tempAnimator;
        private static float _tempColliderHeight = 1.1f;
        private static float _tempColliderRadius = 0.2f;

        #endregion

        #region CreateItemUISection

        public static void DrawGUI() => CreateSection();

        private static void CreateSection()
        {
            GUILayout.Space(10);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Create Character Section", EditorStyles.boldLabel);

                if (GUILayout.Button("Clear")) RestoreEditor();
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                DrawCharacterData();
                DrawPrefabSettings();

                DrawSpriteSettings();
                DrawConfigurationSettings();


                if (GUILayout.Button("Create"))
                {
                    if (!IsFieldsAreValid()) return;
                    CreatePrefab();
                    _isFbxSelected = false;
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

            if (_tempSelectedProduct.Prefab == null && !_isFbxSelected)
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

        private static void DrawSpriteSettings()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    _tempSelectedProduct.Icon =
                        EditorGUILayout.ObjectField("Icon", _tempSelectedProduct.Icon,
                            typeof(Sprite), false) as Sprite;
                }

                if (_isFbxSelected) return;

                EditorGUILayout.HelpBox("You can Auto-create icon selecting a model or fbx from the project ",
                    MessageType.Info);

                if (GUILayout.Button("Create Icon Automatically"))
                {
                    if (_tempSelectedProduct.Prefab != null)
                    {
                        var sprite = ConvertToSpriteExtension.ConvertToSprite(_tempSelectedProduct.Prefab);
                        if (sprite != null) Debug.Log("Sprite automatically generated");
                    }
                    else
                    {
                        Debug.LogWarning("Please select prefab/model first ");
                    }
                }
            }
        }

        private static void DrawPrefabSettings()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.HelpBox("You can select from the project or from your computer", MessageType.Info);
                using (new GUILayout.HorizontalScope())
                {
                    using (new EditorGUI.DisabledScope(_isFbxSelected))
                    {
                        _tempSelectedProduct.Prefab = EditorGUILayout.ObjectField("Select Prefab/FBX",
                            _tempSelectedProduct.Prefab, typeof(GameObject), false) as GameObject;
                    }

                    if (GUILayout.Button("Select FBX"))
                    {
                        string path = EditorUtility.OpenFilePanel("Select FBX", "", "fbx");
                        _selectedFbxPath = path;
                        _isFbxSelected = true;
                    }
                }

                if (_isFbxSelected)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        EditorGUILayout.LabelField("Selected FBX", _selectedFbxPath);
                        _tempSelectedProduct.Prefab = null;
                    }
                }

                _isOptimizeForMobileEnabled =
                    EditorGUILayout.Toggle("Optimize For Mobile", _isOptimizeForMobileEnabled);
            }
        }

        private static void DrawCharacterData()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            using (new GUILayout.VerticalScope())
            {
                _tempSelectedProduct.Name = EditorGUILayout.TextField("Name", _tempSelectedProduct.Name);
                _tempSelectedProduct.Price = EditorGUILayout.IntField("Price", _tempSelectedProduct.Price);
                _tempSelectedProduct.Id = _storeItemContainer.storeItemList.Count;
            }
        }

        private static void DrawConfigurationSettings()
        {
            _showConfig = EditorGUILayout.Foldout(_showConfig, "Additional Configuration Settings");

            if (!_showConfig) return;

            using (new GUILayout.VerticalScope())
            {
                _tempAnimator = EditorGUILayout.ObjectField("Animator", _tempAnimator,
                    typeof(AnimatorController), false) as AnimatorController;
                _tempColliderRadius =
                    EditorGUILayout.FloatField("Collider Radius", _tempColliderRadius);
                _tempColliderHeight =
                    EditorGUILayout.FloatField("Collider Height", _tempColliderHeight);
            }
        }

        #endregion

        private static void RestoreEditor()
        {
            _isFbxSelected = false;
            _isOptimizeForMobileEnabled = true;
            _tempSelectedProduct = new StoreItem();

            SetTempConfigSettings();
        }

        public static void SetTempConfigSettings()
        {
            _tempAnimator = _prefabConfigSettings.animator;
            _tempColliderHeight = _prefabConfigSettings.colliderHeight;
            _tempColliderRadius = _prefabConfigSettings.colliderRadius;
        }

        private static void CreatePrefab()
        {
            var path = _isFbxSelected ? _selectedFbxPath : AssetDatabase.GetAssetPath(_tempSelectedProduct.Prefab);
            var prefab = ImportObjectFromPath(path, out var isFbxFile);
            if (prefab == null) return;

            if (isFbxFile) CreatePrefabForCharacter(_tempSelectedProduct, prefab);

            AddNewItemToContainer();

            Debug.Log($"Prefab created at {path}");
            EditorUtility.DisplayDialog("Character Creation", "Character Creation successful", "OK");
            RestoreEditor();
        }

        private static void CreatePrefabForCharacter(StoreItem storeItem, GameObject model)
        {
            CheckIfPrefabAlreadyExists(storeItem);
            var prefab = PrefabUtility.InstantiatePrefab(model) as GameObject;

            AddCollider(prefab);
            AddAnimator(prefab);

            PrefabUtility.UnpackPrefabInstance(prefab, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
            storeItem.Prefab =
                PrefabUtility.SaveAsPrefabAsset(prefab, $"{PrefabPath}Char{_tempSelectedProduct.Id}.prefab");

            DestroyImmediate(prefab);
        }

        private static void CheckIfPrefabAlreadyExists(StoreItem storeItem)
        {
            if (storeItem.Prefab == null) return;

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(storeItem.Prefab));
            storeItem.Prefab = null;
        }

        public static void SetPrefabConfigSettings(PrefabConfigSettings prefabConfigSettings) =>
            _prefabConfigSettings = prefabConfigSettings;

        private static GameObject ImportObjectFromPath(string path, out bool isFbx)
        {
            isFbx = false;

            if (IsPathNull(path)) return null;

            var fileName = Path.GetFileName(path);
            string destinationPath = FbxPath + fileName;

            if (IsFileAlreadyExists(destinationPath)) return null;

            FileUtil.CopyFileOrDirectory(path, destinationPath);
            AssetDatabase.Refresh();

            if (fileName.Contains(".FBX"))
            {
                var modelImporter = AssetImporter.GetAtPath(destinationPath) as ModelImporter;
                SetDefaultModelSettings(modelImporter);
                isFbx = true;
            }

            var fbxGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(destinationPath);
            return fbxGameObject;
        }

        private static bool IsFileAlreadyExists(string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                Debug.LogWarning($"File already exists at {destinationPath} ");
                return true;
            }

            return false;
        }

        private static bool IsPathNull(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Path can't be empty");
                return true;
            }

            return false;
        }

        private static void AddNewItemToContainer()
        {
            Debug.Log("New Item added" + _tempSelectedProduct.Icon.name);

            _storeItemContainer.storeItemList.Add(_tempSelectedProduct);
        }

        public static void SetStoreItemContainer(StoreItemContainer container) => _storeItemContainer = container;

        private static void AddAnimator(GameObject prefab)
        {
            var animator = prefab.GetComponent<Animator>();
            if (_prefabConfigSettings.animator != null)
            {
                animator.runtimeAnimatorController = _prefabConfigSettings.animator;
            }

            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        }

        private static void AddCollider(GameObject prefab)
        {
            var collider = prefab.AddComponent<CapsuleCollider>();
            collider.radius = _tempColliderRadius;
            collider.height = _tempColliderHeight;
        }

        private static void SetDefaultModelSettings(ModelImporter importer)
        {
            importer.animationType = ModelImporterAnimationType.Human;
            importer.importAnimation = false;
            importer.importConstraints = false;

            if (_isOptimizeForMobileEnabled)
            {
                SetMobileOptimizations(importer);
                Debug.Log("Prefab optimized for mobile");
            }

            importer.SaveAndReimport();
        }

        private static void SetMobileOptimizations(ModelImporter importer)
        {
            // Disable unnecessary import settings
            importer.importVisibility = false;
            importer.importCameras = false;
            importer.importLights = false;
            importer.importBlendShapes = false;

            // Set compression options
            importer.animationCompression = ModelImporterAnimationCompression.Optimal;
            importer.meshCompression = ModelImporterMeshCompression.Low;
            importer.optimizeMeshPolygons = true;
            importer.optimizeMeshVertices = true;
            importer.importTangents = ModelImporterTangents.CalculateMikk;

            importer.importNormals = ModelImporterNormals.Import;
            importer.indexFormat = ModelImporterIndexFormat.UInt16;
            importer.normalCalculationMode = ModelImporterNormalCalculationMode.Unweighted_Legacy;
        }

        public static void OpenEditorWithPrefab(GameObject prefab)
        {
            RestoreEditor();
            _tempSelectedProduct.Prefab = prefab;
        }
    }
}