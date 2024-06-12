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


    [MenuItem("Assets/Script Colorizer/Tint It")]
    public static void ShowWindow()
    {
        ScriptColorizerWindow window = GetWindow<ScriptColorizerWindow>(true);
        window.maxSize = new Vector2(240, 350);

        // Setting icon to the window
        Texture icon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
        window.titleContent = new GUIContent("Script Colorizer", icon);

        // 1. Non-dockable
        // 2. Always on top
        // 3. Prohibits interaction with the Editor until this window is closed.
        window.ShowModalUtility();
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Preview Thumbnail
        iconGenerator ??= new IconGenerator();
        previewImage = new Image
        {
            sprite = iconGenerator.GetIconPreview(Color.white),
            viewDataKey = "lastGeneratedPreview" // Responsible for data persistence
        };
        root.Add(previewImage);

        // Color Picker
        colorField = new ColorField()
        {
            value = new Color(1, 1, 1, 1),
            showAlpha = true,
            showEyeDropper = true,
            hdr = true,
            viewDataKey = "lastSelectedColor"
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
        string iconFilePath = null;
        if (colorField.value != Color.white)
            iconFilePath = iconGenerator.SaveIcon(colorField.value);

        iconApplier ??= new IconApplier();
        iconApplier.ChangeIcon(iconFilePath);
    }

    private void RemoveColorizedIcon()
    {
        iconApplier ??= new IconApplier();
        iconApplier.ResetIcon();
    }
}
