using System.IO;
using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public class IconGenerator
    {
        private IconType iconType;
        private int customTextureInstanceID;
        private Texture2D textureToTint;
        private Texture2D tintedIconTexture;

        private readonly Texture2D defaultIcon;
        private readonly Vector2 previewSpritePivot;
        private readonly float previewSpritePixelPerUnit;
        private readonly string defaultIconName = "d_cs Script Icon";

        //TODO: Dynamically handle iconsDirName;
        // private readonly string iconsDirName = "Packages/com.basementexperiments.scripticoncolorizer/Icons";
        private readonly string iconsDirName = "Assets/ScriptColorizer/Icons";
        private readonly string iconDefaultName = "icon_default_{0}";
        private readonly string iconCustomName = "icon_{0}_{1}";

        public IconGenerator()
        {
            defaultIcon = CopyTexture((Texture2D)EditorGUIUtility.IconContent(defaultIconName).image);
            previewSpritePivot = new Vector2(0.5f, 0.5f);
            previewSpritePixelPerUnit = 100f;
        }

        public Sprite GetIconPreview(Texture2D iconSprite)
        {
            if (iconSprite)
            {
                iconType = IconType.CUSTOM;
                customTextureInstanceID = iconSprite.GetInstanceID();
            }
            else
            {
                iconType = IconType.DEFAULT;
                customTextureInstanceID = int.MinValue;
            }

            textureToTint = (iconType > IconType.DEFAULT_TINTED) ? CopyTexture(iconSprite) : defaultIcon;
            Rect textureRect = new(0.0f, 0.0f, textureToTint.width, textureToTint.height);
            return Sprite.Create(textureToTint, textureRect, previewSpritePivot, previewSpritePixelPerUnit);
        }

        public Sprite GetIconPreview(Color tintColor)
        {
            iconType = GetIconTypeBasedOnColor(iconType, tintColor);
            tintedIconTexture = TintTexture(textureToTint, tintColor);
            Rect textureRect = new(0.0f, 0.0f, textureToTint.width, textureToTint.height);
            return Sprite.Create(tintedIconTexture, textureRect, previewSpritePivot, previewSpritePixelPerUnit);
        }

        public string SaveIcon(Color tintColor)
        {
            if (iconType == IconType.CUSTOM)
            {
                return AssetDatabase.GetAssetPath(customTextureInstanceID);
            }
            else if (iconType == IconType.DEFAULT_TINTED || iconType == IconType.CUSTOM_TINTED)
            {
                string iconName = (iconType == IconType.DEFAULT_TINTED) ? iconDefaultName : iconCustomName;

                if (iconType == IconType.DEFAULT_TINTED)
                    iconName = string.Format(iconName, ColorUtility.ToHtmlStringRGBA(tintColor));
                else
                    iconName = string.Format(iconName, customTextureInstanceID, ColorUtility.ToHtmlStringRGBA(tintColor));
                //TODO: Find way to name custom tinted icons as per the name of the texture and not instance id.

                return SaveTextureToDisk(tintedIconTexture, iconName);
            }

            return null;
        }

        private IconType GetIconTypeBasedOnColor(IconType iconType, Color tintColor)
        {
            IconType newIconType;

            if (tintColor != Color.white)
            {
                newIconType = iconType == IconType.DEFAULT ? IconType.DEFAULT_TINTED
                            : iconType == IconType.CUSTOM ? IconType.CUSTOM_TINTED
                            : iconType;
            }
            else
            {
                newIconType = iconType == IconType.DEFAULT_TINTED ? IconType.DEFAULT
                            : iconType == IconType.CUSTOM_TINTED ? IconType.CUSTOM
                            : iconType;
            }

            return newIconType;
        }

        /// <summary>
        /// Copies pixel data from one texture to another.
        /// </summary>
        /// <param name="textureToCopy">Texture2D to be copied.</param>
        /// <returns>Texture2D created by copying pixels.</returns>
        private Texture2D CopyTexture(Texture2D textureToCopy)
        {
            var tex = new Texture2D(textureToCopy.width, textureToCopy.height, textureToCopy.format, textureToCopy.mipmapCount > 1);
            Graphics.CopyTexture(textureToCopy, tex);
            return tex;
        }

        private Texture2D TintTexture(Texture2D textureToTint, Color tintColor)
        {
            if (!textureToTint || tintColor == Color.white) return textureToTint;

            // Tinting & Storing the pixels.
            Color32[] tintedPixels = textureToTint.GetPixels32();
            for (int i = 0; i < tintedPixels.Length; i++)
                tintedPixels[i] *= tintColor;

            // New texture using the tinted pixels.
            Texture2D tintedTex = new(textureToTint.width, textureToTint.height);
            tintedTex.SetPixels32(tintedPixels);
            tintedTex.Apply();

            return tintedTex;
        }

        private string SaveTextureToDisk(Texture2D textureToSave, string fileName, string fileExtension = "png")
        {
            if (textureToSave == null) return null;

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

    public enum IconType
    {
        DEFAULT,
        DEFAULT_TINTED,
        CUSTOM,
        CUSTOM_TINTED
    }
}