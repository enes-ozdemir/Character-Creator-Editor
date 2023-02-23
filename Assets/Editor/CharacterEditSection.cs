using System.Collections.Generic;
using System.Linq;
using _3_Scripts.SO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class CharacterEditSection : EditorWindow
    {
        private static StoreItemContainer _storeItemContainer;
        private static List<StoreItem> _storeItemList;
        private static StoreItem _tempSelectedItem;
        private static int _selectedItemIndex;
        private static bool _isEditModeOn;

        public static void DrawGUI() => CreateSection();

        public static void SetStoreItemContainer(StoreItemContainer container)
        {
            _storeItemContainer = container;
            _storeItemList = _storeItemContainer.storeItemList;
        }

        private static void CreateSection()
        {
            GUILayout.Space(10);

            var productNames = _storeItemList.Select(p => p.Name).ToList();
            GUILayout.Label("Edit Character Section", EditorStyles.boldLabel);
            
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                _selectedItemIndex =
                    EditorGUILayout.Popup("Select an item", _selectedItemIndex, productNames.ToArray());
                
                if (_storeItemList.Count == 0) return;

                var selectedProduct = _storeItemList[_selectedItemIndex];

                if (!_isEditModeOn && selectedProduct != _tempSelectedItem)
                    _tempSelectedItem = GetCurrentItem(selectedProduct);

                SetFields();
                SetButtons(selectedProduct);
            }
        }

        private static void SetFields()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.BeginDisabledGroup(!_isEditModeOn);
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Name", GUILayout.Width(50f));
                    _tempSelectedItem.Name = EditorGUILayout.TextField(_tempSelectedItem.Name);
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Price", GUILayout.Width(50f));
                    _tempSelectedItem.Price = EditorGUILayout.IntField(_tempSelectedItem.Price);
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Icon", GUILayout.Width(50f));
                    _tempSelectedItem.Icon =
                        EditorGUILayout.ObjectField(_tempSelectedItem.Icon, typeof(Sprite), false) as Sprite;
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Prefab", GUILayout.Width(50f));
                    _tempSelectedItem.Prefab =
                        EditorGUILayout.ObjectField(_tempSelectedItem.Prefab, typeof(GameObject), false) as
                            GameObject;
                }

                EditorGUI.EndDisabledGroup();
            }
        }

        private static void SetButtons(StoreItem selectedProduct)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (!_isEditModeOn)
                {
                    if (GUILayout.Button("Edit"))
                    {
                        _isEditModeOn = true;
                    }
                }
                else
                {
                    if (GUILayout.Button("Save"))
                    {
                        _isEditModeOn = false;
                        _storeItemContainer.storeItemList[_selectedItemIndex] = _tempSelectedItem;
                        EditorUtility.SetDirty(_storeItemContainer);
                        AssetDatabase.SaveAssets();
                        _tempSelectedItem = null;
                        Debug.Log("Item edited");
                    }
                }

                if (GUILayout.Button("Remove"))
                {
                    _storeItemList.Remove(selectedProduct);
                    _selectedItemIndex--;
                    Debug.Log("Item removed from store");

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