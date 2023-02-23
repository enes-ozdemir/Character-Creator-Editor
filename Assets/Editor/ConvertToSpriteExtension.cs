using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ConvertToSpriteExtension
    {
        public static Sprite ConvertToSprite(GameObject prefab)
        {
            var texture2D = AssetPreview.GetAssetPreview(prefab);
            if (texture2D != null)
                return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            
            Debug.LogError("Error while creating the sprite automatically please select the asset again");
            return null;
        }
    }
}