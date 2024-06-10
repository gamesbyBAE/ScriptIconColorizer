using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ScriptColorizerWindow : EditorWindow
{
    private ColorField colorField;
    private Image previewImage;
    private IconGenerator iconGenerator;
    private IconApplier iconApplier;


    [MenuItem("Color Me/Script Colorizer")]
    public static void ShowWindow()
    {
        EditorWindow window = CreateInstance<ScriptColorizerWindow>();
        Texture icon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
        window.titleContent = new GUIContent("Script Colorizer", icon);
        window.Show();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Preview Thumbnail
        iconGenerator ??= new IconGenerator();
        previewImage = new Image
        {
            sprite = iconGenerator.GetIconPreview(Color.white)
        };
        root.Add(previewImage);

        // Color Picker
        colorField = new ColorField()
        {
            value = new Color(1, 1, 1, 1),
            showAlpha = true,
            showEyeDropper = true,
            hdr = true,
        };
        colorField.RegisterValueChangedCallback(UpdatePreview);
        root.Add(colorField);

        // Apply Button
        Button applyButton = new(() => { ApplyColorizedIcon(); }) { text = "Apply" };
        root.Add(applyButton);

        // Remove Color Button
        Button resetButton = new(() => { RemoveColorizedIcon(); }) { text = "Reset" };
        root.Add(resetButton);


    }

    private void UpdatePreview(ChangeEvent<Color> evt)
    {
        previewImage.sprite = iconGenerator.GetIconPreview(evt.newValue);
    }

    private void ApplyColorizedIcon()
    {
        string iconFilePath = iconGenerator.SaveIcon(colorField.value);
        iconApplier ??= new IconApplier();
        iconApplier.ChangeIcon(iconFilePath);
    }

    private void RemoveColorizedIcon()
    {
        iconApplier ??= new IconApplier();
        iconApplier.ResetIcon();
    }
}
