using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconColorizerWindow : EditorWindow
    {
        private ObjectField imagePickerField;
        private ColorField colorField;
        private Image previewImage;
        private IconGenerator iconGenerator;
        private IconApplier iconApplier;
        private readonly Vector2 defaultWindowSize = new(240, 350);
        private ScriptIconColorizerWindow window;

        public void ShowWindow()
        {
            window = GetWindow<ScriptIconColorizerWindow>(true);
            window.maxSize = defaultWindowSize;
            window.minSize = defaultWindowSize;

            // Setting icon to the window
            Texture icon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
            window.titleContent = new GUIContent("Script Icon Colorizer", icon);

            window.ShowUtility();
        }

        private void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.Add(PreviewImage());
            root.Add(ImagePicker());
            root.Add(ColorSelector());
            root.Add(ApplyButton());
            // root.RegisterCallback<GeometryChangedEvent>(FitWindowToContent);
        }

        private void OnDestroy()
        {
            iconGenerator = null;
            iconApplier = null;
        }

        private Image PreviewImage()
        {
            iconGenerator ??= new IconGenerator();
            previewImage = new Image
            {
                sprite = iconGenerator.GetIconPreview(null),
                scaleMode = ScaleMode.ScaleToFit,
                viewDataKey = "lastGeneratedPreview" // Responsible for data persistence
            };

            return previewImage;
        }

        private ObjectField ImagePicker()
        {
            imagePickerField = new ObjectField("Custom Icon:")
            {
                objectType = typeof(Texture2D),
                viewDataKey = "lastSelectedIcon"
            };
            imagePickerField.RegisterValueChangedCallback(OnImageSelectChange);

            return imagePickerField;
        }

        private ColorField ColorSelector()
        {
            colorField = new ColorField("Icon Tint: ")
            {
                value = new Color(1, 1, 1, 1),
                viewDataKey = "lastSelectedColor", // Responsible for data persistence
            };
            // colorField.style.paddingLeft = 16;
            // colorField.style.paddingRight = 16;
            colorField.RegisterValueChangedCallback(OnColorSelectChange);

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

        private void OnImageSelectChange(ChangeEvent<Object> evt)
        {
            colorField.SetValueWithoutNotify(Color.white);
            previewImage.sprite = iconGenerator.GetIconPreview((Texture2D)imagePickerField.value);
        }

        private void OnColorSelectChange(ChangeEvent<Color> evt)
        {
            previewImage.sprite = iconGenerator.GetIconPreview(evt.newValue);
        }

        private void ApplyColorizedIcon()
        {
            iconApplier ??= new IconApplier();
            iconApplier.ChangeIcon(iconGenerator.SaveIcon(colorField.value));
        }
    }
}