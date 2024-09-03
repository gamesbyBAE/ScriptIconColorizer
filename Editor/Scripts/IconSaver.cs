using System.IO;
using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public class IconSaver
    {
        private readonly string iconsDirName = "Packages/com.basementexperiments.scripticoncolorizer/Icons";
        private readonly string iconDefaultName = "icon_default_{0}";
        private readonly string iconCustomName = "icon_{0}_{1}";

        public string SaveIcon(IconContext iconContext)
        {
            switch (iconContext.IconType)
            {
                case IconType.CUSTOM when iconContext.SourceTexture:
                    return AssetDatabase.GetAssetPath(iconContext.SourceTexture);

                case IconType.DEFAULT_TINTED:
                case IconType.CUSTOM_TINTED:
                    string iconName = GetIconName(iconContext);
                    return SaveTextureToDisk(iconContext.TintedTexture, iconName);

                default:
                    return null;
            }
        }

        private string GetIconName(IconContext iconContext)
        {
            string iconName = (iconContext.IconType == IconType.DEFAULT_TINTED) ? iconDefaultName : iconCustomName;

            switch (iconContext.IconType)
            {
                case IconType.DEFAULT_TINTED:
                    return string.Format(iconName, iconContext.SelectedColorHTMLString);

                case IconType.CUSTOM_TINTED:
                    string textureName = iconContext.SourceTexture ? iconContext.SourceTexture.name : "";
                    return string.Format(iconName, textureName, iconContext.SelectedColorHTMLString);
            }

            return null;
        }

        private string SaveTextureToDisk(Texture2D textureToSave, string fileName, string fileExtension = "png")
        {
            if (textureToSave == null || string.IsNullOrEmpty(fileName)) return null;

            // Creates directory if doesn't exist.
            Directory.CreateDirectory(iconsDirName);

            string filePath = Path.Combine(iconsDirName, $"{fileName}.{fileExtension}");

            if (!File.Exists(filePath))
            {
                File.WriteAllBytes(filePath, textureToSave.EncodeToPNG());
                AssetDatabase.Refresh();
            }

            return filePath;
        }
    }
}