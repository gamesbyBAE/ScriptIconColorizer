using UnityEngine;

namespace BasementExperiments.ScriptIconColorizer
{
    public class IconContext
    {
        public IconType IconType { get; }
        public Texture2D SelectedTexture { get; }
        public string SelectedColorHTMLString { get; }
        public Texture2D TintedTexture { get; }

        public IconContext(IconType iconType, Texture2D selectedTexture, string selectedColorHTMLString, Texture2D tintedTexture)
        {
            IconType = iconType;
            SelectedTexture = selectedTexture;
            SelectedColorHTMLString = selectedColorHTMLString;
            TintedTexture = tintedTexture;
        }
    }

    public enum IconType { DEFAULT, DEFAULT_TINTED, CUSTOM, CUSTOM_TINTED }
}