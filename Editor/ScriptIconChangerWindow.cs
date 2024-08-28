using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconChangerWindow : EditorWindow
    {
        private ObjectField imagePickerField;
        private ColorField colorField;
        private Image previewImage;
        private IconGenerator iconGenerator;
        private IconApplier iconApplier;
        private readonly Vector2 defaultWindowSize = new(300, 350);
        private ScriptIconChangerWindow window;

        private Texture2D lastSelectedIcon;

        public void ShowWindow()
        {
            window = GetWindow<ScriptIconChangerWindow>(true);
            window.maxSize = defaultWindowSize;
            window.minSize = defaultWindowSize;

            // Changing the window icon.
            Texture icon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
            window.titleContent = new GUIContent("Script Icon Colorizer", icon);

            // window.ShowUtility();
            window.Show();
        }

        private void CreateGUI()
        {
            iconApplier ??= new IconApplier();
            iconGenerator ??= new IconGenerator();

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

            previewImage = null;

            imagePickerField?.UnregisterValueChangedCallback(OnImageSelectChange);
            imagePickerField = null;

            colorField?.UnregisterValueChangedCallback(OnColorSelectChange);
            colorField = null;
        }

        private Image PreviewImage()
        {
            previewImage = new Image { scaleMode = ScaleMode.ScaleToFit };
            return previewImage;
        }

        private ObjectField ImagePicker()
        {
            imagePickerField = new ObjectField("Custom Icon:")
            {
                objectType = typeof(Texture2D),
                viewDataKey = "lastSelectedIcon" // Responsible for data persistence
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

        //TODO: Handle None/null selected to display default icon.
        private void OnImageSelectChange(ChangeEvent<Object> evt)
        {
            if (!evt.newValue) return;

            if (evt.newValue != evt.previousValue)
                ResetColorPickerWithoutNotify();

            Texture2D selectedTexture = evt.newValue ? (Texture2D)evt.newValue : null;
            lastSelectedIcon = selectedTexture;
            previewImage.sprite = iconGenerator.GetIconPreview(selectedTexture);

            RepaintImagePickerUI();
        }

        private void OnColorSelectChange(ChangeEvent<Color> evt)
        {
            previewImage.sprite = iconGenerator.GetIconPreview(evt.newValue);
        }

        private void ApplyColorizedIcon()
        {
            iconApplier.ChangeIcon(iconGenerator.SaveIcon(colorField.value));
        }

        /// <summary>
        /// Repaints the ObjectField with the user selected texture
        /// without triggering the value change event.
        /// </summary>
        private void RepaintImagePickerUI()
        {
            /*
                Necessary because domain reload visually shows 'None' selected
                but clicking the field or the dot on right shows it actually has
                a value assigned to it.

                Hence, refreshing/repainting the field to visually reflect the
                selection for better UX.
            */

            if (!imagePickerField.value) return;

            Object storedValue = imagePickerField.value;
            imagePickerField.value = null; // Clearing to force the UI to refresh
            imagePickerField.SetValueWithoutNotify(storedValue);
        }

        /// <summary>
        /// Resets the ColorField to show White color without
        /// triggering the Value Change Event.
        /// </summary>
        private void ResetColorPickerWithoutNotify()
        {
            colorField.SetValueWithoutNotify(Color.white);
        }
    }
}