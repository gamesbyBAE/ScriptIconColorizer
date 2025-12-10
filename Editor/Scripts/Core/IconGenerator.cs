using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class IconGenerator
    {
        private readonly Texture2D defaultIcon;
        public Texture2D DefaultIcon => defaultIcon;

        private readonly string defaultIconName = "d_cs Script Icon";

        public IconGenerator()
        {
            defaultIcon = CopyTexture((Texture2D)EditorGUIUtility.IconContent(defaultIconName).image);
            if (!defaultIcon)
                Debug.LogError("[FAIL]: Default 'MonoScript' icon could not be loaded.");
        }

        public IconContext GenerateIconAndGetContext(Texture2D sourceTexture, Color tintColor)
        {
            IconType iconType = sourceTexture ? IconType.CUSTOM : IconType.DEFAULT;
            iconType = Utils.GetIconTypeFromColor(iconType, tintColor);

            bool needsTinting = Utils.NeedsTinting(iconType);

            Texture2D iconTexture = defaultIcon;
            string textureName = "default";

            if (sourceTexture)
            {
                iconTexture = needsTinting ? CopyTexture(sourceTexture) : sourceTexture;
                textureName = sourceTexture.name;
            }

            if (needsTinting)
            {
                iconTexture = TintTexture(iconTexture, tintColor);
            }

            IconContext iconContext = new(
                iconType,
                tintColor,
                iconTexture,
                textureName);

            return iconContext;
        }

        /// <summary>
        /// Clones the sauce Texture2D.
        /// Necessary to avoid modifying the original texture when applying tints, moreover,
        /// permissions to write to the original texture may not be available.
        /// </summary>
        /// <param name="textureToCopy">Texture2D to be copied.</param>
        /// <returns>Texture2D created by copying pixels.</returns>
        private Texture2D CopyTexture(Texture2D textureToCopy)
        {
            try
            {
                var tex = new Texture2D(
                    textureToCopy.width,
                    textureToCopy.height,
                    textureToCopy.format,
                    textureToCopy.mipmapCount > 1);

                Graphics.CopyTexture(textureToCopy, tex);

                return tex;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Multiplies each pixel of the texture by the tint color.
        /// </summary>
        /// <param name="textureToTint">Texture2D to be used as the sauce for modification.</param>
        /// <param name="tintColor">Color to be applied to each pixel.</param>
        /// <returns>A copy of source Texture2D that is tinted.</returns>
        private Texture2D TintTexture(Texture2D textureToTint, Color tintColor)
        {
            if (!textureToTint || tintColor == Color.white)
                return textureToTint;

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