using System.IO;
using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public class IconSaver
    {
        private readonly string iconsDirName = "Packages/com.basementexperiments.scripticoncolorizer/Icons";
        private readonly string iconCustomName = "d3rp_<textureName>_<color>";

        public string SaveIcon(IconContext iconContext)
        {
            switch (iconContext.IconType)
            {
                case IconType.CUSTOM:
                    return GetVirginAssetPath(iconContext.IconTexture);

                case IconType.DEFAULT_TINTED:
                case IconType.CUSTOM_TINTED:
                    string iconName = GetIconName(iconContext.TextureName, iconContext.TintColor);
                    Debug.LogFormat($"Saving tinted icon as: {iconName}");
                    return SaveTextureToDisk(iconContext.IconTexture, iconName);

                default:
                    return null;
            }
        }

        private string GetVirginAssetPath(Texture2D texture)
        {
            if (!texture) return null;
            return AssetDatabase.GetAssetPath(texture);
        }

        private string GetIconName(string textureName, Color color)
        {
            if (string.IsNullOrEmpty(textureName))
                textureName = "default";

            string colorHTMLString = ColorUtility.ToHtmlStringRGBA(color);

            return iconCustomName
                .Replace("<textureName>", textureName)
                .Replace("<color>", colorHTMLString);
        }

        private string SaveTextureToDisk(
            Texture2D textureToSave,
            string fileName,
            string fileExtension = "png")
        {
            if (textureToSave == null || string.IsNullOrEmpty(fileName)) return null;

            // Creates directory if doesn't exist.
            Directory.CreateDirectory(iconsDirName);

            string filePath = Path.Combine(iconsDirName, $"{fileName}.{fileExtension}");
            Debug.LogFormat($"Saving icon to path: {filePath}");

            if (!File.Exists(filePath))
            {
                File.WriteAllBytes(filePath, textureToSave.EncodeToPNG());
                AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            }

            return filePath;
        }
    }
}