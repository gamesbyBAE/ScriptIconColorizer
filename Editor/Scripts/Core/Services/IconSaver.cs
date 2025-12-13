using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEditor.PackageManager;
using System;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class IconSaver
    {
        private readonly string iconsDirName;
        private readonly string iconFileName = "d3rp_<textureName>_<color>";
        private readonly string iconsDirectory = "CustomScriptIcons";

        public IconSaver()
        {
            // Assembly editorAssembly = Assembly.GetExecutingAssembly();
            // var packageInfo = PackageInfo.FindForAssembly(editorAssembly);
            // if (packageInfo != null)
            // {
            //     iconsDirName = Path.Combine("Packages", packageInfo.name, iconsDirectory);
            //     return;
            // }

            iconsDirName = Path.Combine("Assets", iconsDirectory);
        }
        public string SaveIcon(IconContext iconContext)
        {
            switch (iconContext.IconType)
            {
                case IconType.CUSTOM:
                    return GetUserAssetPath(iconContext.IconTexture);

                case IconType.DEFAULT_TINTED:
                case IconType.CUSTOM_TINTED:
                    {
                        string iconName = GetIconName(
                            iconContext.TextureName,
                            iconContext.TintColor);

                        return SaveTextureToDisk(iconContext.IconTexture, iconName);
                    }

                default:
                    return null;
            }
        }

        private string GetUserAssetPath(Texture2D texture)
        {
            if (!texture) return null;
            return UnityEditor.AssetDatabase.GetAssetPath(texture);
        }

        private string GetIconName(string textureName, Color color)
        {
            if (string.IsNullOrEmpty(textureName))
                textureName = "unknown";

            string colorHTMLString = ColorUtility.ToHtmlStringRGBA(color);

            return iconFileName
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

            if (!File.Exists(filePath))
            {
                try
                {
                    File.WriteAllBytes(filePath, textureToSave.EncodeToPNG());
                    UnityEditor.AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[FAIL]: Could not save icon to disk.\n{e}");
                    return null;
                }
            }

            return filePath;
        }
    }
}