using System.IO;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private Color tint;

    private readonly string iconName = "d_cs Script Icon";
    private readonly string iconsDirName = "Assets/ScriptsMarker/Icons";
    private readonly string fileName = "icon_{0}";

    [ContextMenu("Tint It")]
    void Start()
    {
        // TODO: ADD NULL CHECKS!
        Texture2D iconOrig = (Texture2D)EditorGUIUtility.IconContent(iconName).image;
        Texture2D iconCopy = CopyTexture(iconOrig);
        Texture2D tintedIcon = TintTexture(iconCopy, tint);

        string iconFileName = SaveTextureToDisk(tintedIcon, string.Format(fileName, ColorUtility.ToHtmlStringRGBA(tint)));

        SetCustomIconOnMonoScript(iconFileName);
    }

    private Texture2D CopyTexture(Texture2D icon)
    {
        var tex = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
        Graphics.CopyTexture(icon, tex);
        return tex;
    }

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

        return Path.GetFileName(filePath);
    }

    private void SetCustomIconOnMonoScript(string iconFileName)
    {
        MonoImporter monoImporter = (MonoImporter)AssetImporter.GetAtPath("Assets/SampleScript_1.cs"); //TODO: Get file path from Selection.

        string relativePath = Path.Combine(iconsDirName, iconFileName);
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

        monoImporter.SetIcon(icon);
        monoImporter.SaveAndReimport();
    }
}
