using System.IO;
using UnityEditor;
using UnityEngine;

public class IconGenerator
{
    private readonly string iconName = "d_cs Script Icon";
    private readonly string iconsDirName = "Assets/ScriptsMarker/Icons";
    private readonly string fileName = "icon_{0}";
    private readonly Texture2D iconDefault;
    private Texture2D iconTinted;
    private readonly Rect previewSpriteRect;
    private readonly Vector2 previewSpritePivot;
    private readonly float previewSpritePixelPerUnit;

    public IconGenerator()
    {
        iconDefault = CopyTexture((Texture2D)EditorGUIUtility.IconContent(iconName).image);
        previewSpriteRect = new Rect(0.0f, 0.0f, iconDefault.width, iconDefault.height);
        previewSpritePivot = new Vector2(0.5f, 0.5f);
        previewSpritePixelPerUnit = 100f;
    }

    public Sprite GetIconPreview(Color tintColor)
    {
        iconTinted = TintTexture(iconDefault, tintColor);
        return Sprite.Create(iconTinted, previewSpriteRect, previewSpritePivot, previewSpritePixelPerUnit);
    }

    public string SaveIcon(Color tintColor)
    {
        return SaveTextureToDisk(iconTinted, string.Format(fileName, ColorUtility.ToHtmlStringRGBA(tintColor)));
    }

    private Texture2D CopyTexture(Texture2D icon)
    {
        var tex = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
        Graphics.CopyTexture(icon, tex);
        return tex;
    }

    // TODO: Optimise generating of 'pixels' array.
    // TODO: Cache using Texture2D.GetPixels and loop everytime?
    private Texture2D TintTexture(Texture2D texture2D, Color tint)
    {
        int width = texture2D.width;
        int height = texture2D.height;

        int pixelIndex = 0;
        Color32[] pixels = new Color32[width * height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixels[pixelIndex] = texture2D.GetPixel(j, i) * tint;
                pixelIndex++;
            }
        }

        Texture2D tintedTex = new(width, height);
        tintedTex.SetPixels32(pixels);
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
