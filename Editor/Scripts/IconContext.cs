using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public enum IconType { DEFAULT, DEFAULT_TINTED, CUSTOM, CUSTOM_TINTED }

    public class IconContext
    {
        public IconType IconType { get; }
        public Color TintColor { get; }
        public Texture2D IconTexture { get; }
        public string TextureName { get; }

        public IconContext(
            IconType iconType,
            Color tintColor,
            Texture2D iconTexture,
            string textureName)
        {
            IconType = iconType;
            TintColor = tintColor;
            IconTexture = iconTexture;
            TextureName = textureName;
        }
    }
}