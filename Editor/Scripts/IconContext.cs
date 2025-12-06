using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public class IconContext
    {
        public IconType IconType { get; }
        public Texture2D SourceTexture { get; }
        public string SelectedColorHTMLString { get; }
        public Texture2D TintedTexture { get; }

        public IconContext(
            IconType iconType,
            Texture2D sourceTexture,
            string selectedColorHTMLString,
            Texture2D tintedTexture)
        {
            IconType = iconType;
            SourceTexture = sourceTexture;
            SelectedColorHTMLString = selectedColorHTMLString;
            TintedTexture = tintedTexture;
        }
    }

    public enum IconType { DEFAULT, DEFAULT_TINTED, CUSTOM, CUSTOM_TINTED }
}