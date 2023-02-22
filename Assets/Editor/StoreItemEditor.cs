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
        public static StoreItemContainer _storeItemContainer;
        private static List<StoreItem> _storeItemList;
        private static int _selectedProductIndex = 0;

        private static StoreItem _tempSelectedProduct = null;

        //private string _folderPath = "Assets/4_SO";
        private static bool _isEditing = false;

        private static SerializedObject _so;
        private static SerializedProperty _propList;


        public static void DrawGUI()
        {
            _so.Update();
            EditorGUILayout.PropertyField(_propList);
            _so.ApplyModifiedProperties();
            DisplayEditArea();
        }

        public static void SetStoreItemContainer(StoreItemContainer container)
        {
            _storeItemContainer = container;
            _storeItemList = _storeItemContainer.storeItemList;
        }

        public static void SetSerializedObjects(SerializedObject so, SerializedProperty _serializedProperty)
        {
            _so = so;
            _propList = _serializedProperty;
        }

        private static void DisplayEditArea()
        {
            var productNames = new List<string>();
            foreach (var product in _storeItemList) productNames.Add(product.Name);

            GUILayout.Space(10);
            GUILayout.Label("Edit item area", EditorStyles.boldLabel);
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
                            _storeItemContainer.storeItemList[_selectedProductIndex] = _tempSelectedProduct;
                            EditorUtility.SetDirty(_storeItemContainer);
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