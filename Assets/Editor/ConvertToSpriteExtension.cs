using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class ConvertToSpriteExtension
    {
        private const string SpritePath = "Assets/1_Graphics/Store/";

        public static Sprite ConvertToSprite(GameObject prefab)
        {
            if (prefab == null) return null;
            var texture2D = AssetPreview.GetAssetPreview(prefab);
            if (texture2D != null)
            {
                var sprite = CreateAndImportSprite(prefab, texture2D, out var spritePath);

                SetTextureImportSettings(spritePath);

                return sprite;
            }

            Debug.LogError("Error while creating the sprite automatically please try to generate it again");
            return null;
        }

        private static Sprite CreateAndImportSprite(GameObject prefab, Texture2D texture2D, out string spritePath)
        {
            var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
            sprite.name = prefab.name + "Sprite";
            spritePath = SpritePath + sprite.name + ".png";
            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(spritePath, bytes);
            AssetDatabase.ImportAsset(spritePath, ImportAssetOptions.ForceUpdate);
            return sprite;
        }

        private static void SetTextureImportSettings(string spritePath)
        {
            var textureImporter = AssetImporter.GetAtPath(spritePath) as TextureImporter;
            if (textureImporter != null)
            {
                textureImporter.isReadable = true;
                textureImporter.textureType = TextureImporterType.Sprite;
                textureImporter.spriteImportMode = SpriteImportMode.Single;
                textureImporter.alphaIsTransparency = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.wrapModeU = TextureWrapMode.Clamp;
                textureImporter.wrapModeV = TextureWrapMode.Clamp;
                textureImporter.SaveAndReimport();
            }
        }
    }
}