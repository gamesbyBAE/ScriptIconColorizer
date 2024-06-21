using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconColorizerWindow : EditorWindow
    {
        private ColorField colorField;
        private Image previewImage;
        private IconGenerator iconGenerator;
        private IconApplier iconApplier;
        private readonly Vector2 defaultWindowSize = new(240, 350);
        ScriptIconColorizerWindow window;

        public void ShowWindow()
        {
            window = GetWindow<ScriptIconColorizerWindow>(true);
            window.maxSize = defaultWindowSize;
            window.minSize = defaultWindowSize;

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
            VisualElement root = rootVisualElement;
            root.Add(PreviewImage());
            root.Add(ColorSelector());
            root.Add(ApplyButton());
            root.RegisterCallback<GeometryChangedEvent>(FitWindowToContent);
        }

        private Image PreviewImage()
        {
            iconGenerator ??= new IconGenerator();
            previewImage = new Image
            {
                sprite = iconGenerator.GetIconPreview(Color.white),
                scaleMode = ScaleMode.ScaleToFit,
                viewDataKey = "lastGeneratedPreview" // Responsible for data persistence
            };

            return previewImage;
        }

        private ColorField ColorSelector()
        {
            colorField = new ColorField()
            {
                value = new Color(1, 1, 1, 1),
                showAlpha = true,
                showEyeDropper = true,
                hdr = true,
                viewDataKey = "lastSelectedColor", // Responsible for data persistence
            };
            colorField.style.paddingLeft = 16;
            colorField.style.paddingRight = 16;
            colorField.RegisterValueChangedCallback(UpdatePreview);

            return colorField;
        }

        private Button ApplyButton()
        {
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

            return applyButton;
        }


        // For Editor v2021.3.16f1, default Script icon is substantially smaller than
        // it is in Editor v2022.3.24f1, hence, resizing this Window.
        private void FitWindowToContent(GeometryChangedEvent evt)
        {
            rootVisualElement.UnregisterCallback<GeometryChangedEvent>(FitWindowToContent);

            float height = 0;
            foreach (var child in rootVisualElement.Children())
                height += child.resolvedStyle.height + child.resolvedStyle.marginTop + child.resolvedStyle.marginBottom;

            if (height < defaultWindowSize.y)
            {
                Vector2 newSize = new(defaultWindowSize.x, defaultWindowSize.y - (height + 50));
                window.maxSize = newSize;
                window.minSize = newSize;
            }
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