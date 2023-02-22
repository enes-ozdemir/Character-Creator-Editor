using System;
using System.Collections.Generic;
using _3_Scripts;
using _3_Scripts.SO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class StoreItemEditor : EditorWindow
    {
        public StoreItemContainer storeItemContainer;
        private List<StoreItem> _storeItemList;
        private int _selectedProductIndex = 0;
        private StoreItem _tempSelectedProduct = null;
        private string _folderPath = "Assets/4_SO";
        private bool _isEditing = false;

        private SerializedObject so;
        private SerializedProperty _propList;

        private void OnEnable()
        {
            string[] storeItemGuids = AssetDatabase.FindAssets("t:" + nameof(StoreItemContainer));
            string path = AssetDatabase.GUIDToAssetPath(storeItemGuids[0]);
            storeItemContainer = AssetDatabase.LoadAssetAtPath<StoreItemContainer>(path);
            _storeItemList = storeItemContainer.storeItemList;
            so = new SerializedObject(storeItemContainer);

            _propList = so.FindProperty("storeItemList");
            


            //TODO add check there can't be more than one StoreItemContainer or make it dynamic
        }

        [MenuItem("Tools/Store Item Editor")]
        public static void OpenWindow()
        {
            var window = GetWindow<StoreItemEditor>();
            window.titleContent = new GUIContent("Store Item Editor");
            window.Show();
        }

        private void OnGUI()
        {
            so.Update();
            EditorGUILayout.PropertyField(_propList);
            so.ApplyModifiedProperties();
            DisplayEditArea();
        }

        private void DisplayEditArea()
        {
            var productNames = new List<string>();
            foreach (var product in _storeItemList) productNames.Add(product.Name);

            GUILayout.Space( 10 );
            GUILayout.Label( "Edit item area", EditorStyles.boldLabel );
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _selectedProductIndex =
                    EditorGUILayout.Popup("Select an item", _selectedProductIndex, productNames.ToArray());

                var selectedProduct = _storeItemList[_selectedProductIndex];

                _tempSelectedProduct = GetCurrentItem(selectedProduct);

                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUI.BeginDisabledGroup(!_isEditing);
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Name", GUILayout.Width(50f));
                        _tempSelectedProduct.Name = EditorGUILayout.TextField(_tempSelectedProduct.Name);
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Price", GUILayout.Width(50f));
                        _tempSelectedProduct.Price = EditorGUILayout.IntField(_tempSelectedProduct.Price);
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Icon", GUILayout.Width(50f));
                        _tempSelectedProduct.Icon =
                            EditorGUILayout.ObjectField(_tempSelectedProduct.Icon, typeof(Sprite), false) as Sprite;
                    }

                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Prefab", GUILayout.Width(50f));
                        _tempSelectedProduct.Prefab =
                            EditorGUILayout.ObjectField(_tempSelectedProduct.Prefab, typeof(GameObject), false) as
                                GameObject;
                    }

                    EditorGUI.EndDisabledGroup();
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (!_isEditing)
                    {
                        if (GUILayout.Button("Edit"))
                        {
                            _isEditing = true;
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Save"))
                        {
                            _isEditing = false;
                            storeItemContainer.storeItemList[_selectedProductIndex] = _tempSelectedProduct;
                            EditorUtility.SetDirty(storeItemContainer);
                            AssetDatabase.SaveAssets();
                            _tempSelectedProduct = null;
                        }
                    }

                    if (GUILayout.Button("Remove"))
                    {
                        _storeItemList.Remove(selectedProduct);
                        _tempSelectedProduct = null;
                        //Todo add delete related files
                    }
                }
            }
        }

        private static StoreItem GetCurrentItem(StoreItem selectedProduct)
        {
            return new StoreItem
            {
                Name = selectedProduct.Name,
                Price = selectedProduct.Price,
                Icon = selectedProduct.Icon,
                Prefab = selectedProduct.Prefab,
                Id = selectedProduct.Id
            };
        }
    }
}