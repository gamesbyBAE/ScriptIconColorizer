using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
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
                Debug.LogError("Default 'MonoScript' icon could not be loaded.");
        }

        public IconContext GenerateIconAndGetContext(Texture2D sourceTexture, Color tintColor)
        {
            IconType iconType = sourceTexture ? IconType.CUSTOM : IconType.DEFAULT;
            iconType = Utils.GetIconTypeFromColor(iconType, tintColor);

            bool needsTinting = Utils.NeedsTinting(iconType);

            Texture2D iconTexture = defaultIcon;
            string newTextureName = "default";

            if (sourceTexture)
            {
                newTextureName = sourceTexture.name;
                iconTexture = needsTinting ? CopyTexture(sourceTexture) : sourceTexture;
            }

            if (needsTinting)
                iconTexture = TintTexture(iconTexture, tintColor);

            IconContext iconContext = new(
                iconType,
                tintColor,
                iconTexture,
                newTextureName);

            Debug.LogFormat($"IconContext: {iconType}, {tintColor}, {newTextureName}, {iconTexture == null}");

            return iconContext;
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

            Debug.Log("Texture tinted successfully.");
            return tintedTex;
        }
    }
}