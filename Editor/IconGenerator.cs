using System.IO;
using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public class IconGenerator
    {
        private Texture2D tintedIconTexture;

        private readonly Color32[] defaultIconPixels;
        private readonly int defaultIconWidth, defaultIconHeight;

        private readonly Rect previewSpriteRect;
        private readonly Vector2 previewSpritePivot;
        private readonly float previewSpritePixelPerUnit;

        private readonly string defaultIconName = "d_cs Script Icon";
        private readonly string iconsDirName = "Packages/com.basementexperiments.scriptcolorizer/Icons";
        private readonly string iconName = "icon_{0}";

        public IconGenerator()
        {
            Texture2D defaultIcon = CopyTexture((Texture2D)EditorGUIUtility.IconContent(defaultIconName).image);

            defaultIconWidth = defaultIcon.width;
            defaultIconHeight = defaultIcon.height;

            defaultIconPixels = new Color32[defaultIcon.width * defaultIcon.height];
            defaultIconPixels = defaultIcon.GetPixels32();

            previewSpriteRect = new Rect(0.0f, 0.0f, defaultIcon.width, defaultIcon.height);
            previewSpritePivot = new Vector2(0.5f, 0.5f);
            previewSpritePixelPerUnit = 100f;
        }

        public Sprite GetIconPreview(Color tintColor)
        {
            tintedIconTexture = TintTexture(tintColor);
            return Sprite.Create(tintedIconTexture, previewSpriteRect, previewSpritePivot, previewSpritePixelPerUnit);
        }

        public string SaveIcon(Color tintColor)
        {
            string iconName = string.Format(this.iconName, ColorUtility.ToHtmlStringRGBA(tintColor));
            return SaveTextureToDisk(tintedIconTexture, iconName);
        }

        private Texture2D CopyTexture(Texture2D icon)
        {
            var tex = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
            Graphics.CopyTexture(icon, tex);
            return tex;
        }

        private Texture2D TintTexture(Color tint)
        {
            // Tinting & storing the pixels.
            Color32[] tintedPixels = new Color32[defaultIconPixels.Length];
            for (int i = 0; i < defaultIconPixels.Length; i++)
                tintedPixels[i] = defaultIconPixels[i] * tint;

            // New texture using the tinted pixels.
            Texture2D tintedTex = new(defaultIconWidth, defaultIconHeight);
            tintedTex.SetPixels32(tintedPixels);
            tintedTex.Apply();

            return tintedTex;
        }

        private string SaveTextureToDisk(Texture2D textureToSave, string fileName, string fileExtension = "png")
        {
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