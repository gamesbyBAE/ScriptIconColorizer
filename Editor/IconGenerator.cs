using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public class IconGenerator
    {
        public IconType NewIconType { get; private set; }
        private Texture2D textureToModify;
        public Texture2D NewTintedTexture { get; private set; }

        private readonly Texture2D defaultIcon;
        private readonly Vector2 previewSpritePivot;
        private readonly float previewSpritePixelPerUnit;
        private readonly string defaultIconName = "d_cs Script Icon";

        public IconGenerator()
        {
            defaultIcon = CopyTexture((Texture2D)EditorGUIUtility.IconContent(defaultIconName).image);
            previewSpritePivot = new Vector2(0.5f, 0.5f);
            previewSpritePixelPerUnit = 100f;
        }

        public Sprite GetIconPreview(Texture2D selectedTexture)
        {
            NewIconType = selectedTexture ? IconType.CUSTOM : IconType.DEFAULT;

            textureToModify = (NewIconType > IconType.DEFAULT_TINTED) ? CopyTexture(selectedTexture) : defaultIcon;
            return CreateNewSprite(textureToModify);
        }

        public Sprite GetIconPreview(Color tintColor)
        {
            NewIconType = GetIconTypeBasedOnColor(NewIconType, tintColor);

            NewTintedTexture = TintTexture(textureToModify, tintColor);
            return CreateNewSprite(NewTintedTexture);
        }

        /// <summary>
        /// Copies pixel data from one texture to another.
        /// </summary>
        /// <param name="textureToCopy">Texture2D to be copied.</param>
        /// <returns>Texture2D created by copying pixels.</returns>
        private Texture2D CopyTexture(Texture2D textureToCopy)
        {
            try
            {
                var tex = new Texture2D(textureToCopy.width, textureToCopy.height, textureToCopy.format, textureToCopy.mipmapCount > 1);
                Graphics.CopyTexture(textureToCopy, tex);
                return tex;
            }
            catch (System.Exception e)
            {
                throw e;
            }
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
        /// Generates sprite from a given Texture2D.
        /// </summary>
        /// <param name="sourceTexture">Texture to be used to create new sprite.</param>
        /// <returns>Newly created sprite.</returns>
        private Sprite CreateNewSprite(Texture2D sourceTexture)
        {
            Rect textureRect = new(0.0f, 0.0f, sourceTexture.width, sourceTexture.height);
            return Sprite.Create(sourceTexture, textureRect, previewSpritePivot, previewSpritePixelPerUnit);
        }

        private Texture2D TintTexture(Texture2D textureToTint, Color tintColor)
        {
            if (!textureToTint || tintColor == Color.white) return textureToTint;

            Color32[] tintedPixels = textureToTint.GetPixels32();
            for (int i = 0; i < tintedPixels.Length; i++)
                tintedPixels[i] *= tintColor;

            // New texture using the tinted pixels.
            Texture2D tintedTex = new(textureToTint.width, textureToTint.height);
            tintedTex.SetPixels32(tintedPixels);
            tintedTex.Apply();

            return tintedTex;
        }
    }
}