using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptColorizer
{
    public class ScriptsColorizerWindow : EditorWindow
    {
        private ColorField colorField;
        private Image previewImage;
        private IconGenerator iconGenerator;
        private IconApplier iconApplier;

        public void ShowWindow()
        {
            ScriptsColorizerWindow window = GetWindow<ScriptsColorizerWindow>(true);
            window.maxSize = new Vector2(240, 350);

            // Setting icon to the window
            Texture icon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
            window.titleContent = new GUIContent("Script Colorizer", icon);

            // 1. Non-dockable
            // 2. Always on top
            // 3. Prohibits interaction with the Editor until this window is closed.
            window.ShowModalUtility();
        }

        private void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // PREVIEW THUMBNAIL
            iconGenerator ??= new IconGenerator();
            previewImage = new Image
            {
                sprite = iconGenerator.GetIconPreview(Color.white),
                viewDataKey = "lastGeneratedPreview" // Responsible for data persistence
            };
            root.Add(previewImage);

            // COLOR PICKER
            colorField = new ColorField()
            {
                value = new Color(1, 1, 1, 1),
                showAlpha = true,
                showEyeDropper = true,
                hdr = true,
                viewDataKey = "lastSelectedColor",
            };
            colorField.style.paddingLeft = 16;
            colorField.style.paddingRight = 16;
            colorField.RegisterValueChangedCallback(UpdatePreview);
            root.Add(colorField);

            // APPLY BUTTON
            Button applyButton = new(() => { ApplyColorizedIcon(); }) { text = "APPLY" };
            applyButton.style.marginLeft = 55;
            applyButton.style.marginRight = 55;
            applyButton.style.marginBottom = 25;
            applyButton.style.marginTop = 15;
            applyButton.style.borderBottomLeftRadius = 10;
            applyButton.style.borderBottomRightRadius = 10;
            applyButton.style.paddingBottom = 8;
            applyButton.style.paddingTop = 8;
            applyButton.style.fontSize = 15;
            root.Add(applyButton);
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
    }
}