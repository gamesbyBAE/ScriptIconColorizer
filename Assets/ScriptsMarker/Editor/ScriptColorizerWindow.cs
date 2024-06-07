using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ScriptColorizerWindow : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    private Image previewImage;

    [MenuItem("Window/Script Colorizer")]
    public static void ShowWindow()
    {
        ScriptColorizerWindow wnd = GetWindow<ScriptColorizerWindow>();
        Texture icon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
        wnd.titleContent = new GUIContent("Script Colorizer", icon);
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Color Picker
        ColorField colorField = new()
        {
            value = new Color(0.5f, 0.5f, 0.5f, 1.0f),
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

        // Preview Thumbnail
        root.Add(previewImage);
    }

    private void UpdatePreview(ChangeEvent<Color> evt)
    {
        Debug.Log(evt.newValue);
        // TODO: Call method that returns Sprite & assign to previewImage.sprite
    }

    private void ApplyColorizedIcon()
    {
        Debug.Log("Apply Clicked!");
    }

    private void RemoveColorizedIcon()
    {
        Debug.Log("Reset Clicked!");
    }
}
