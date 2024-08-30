// using System;
using System.Drawing.Printing;
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
        float bottomContentHeight;

        public void ShowWindow()
        {
            window = GetWindow<ScriptIconChangerWindow>(true);
            // window.maxSize = defaultWindowSize;
            // window.minSize = defaultWindowSize;

            // Changing the window icon.
            Texture icon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
            window.titleContent = new GUIContent("Script Icon Colorizer", icon);

            window.ShowUtility();
        }

        private void CreateGUI()
        {
            iconApplier ??= new IconApplier();
            iconGenerator ??= new IconGenerator();

            VisualElement root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;
            root.style.justifyContent = Justify.SpaceBetween;

            VisualElement imageArea = new();
            imageArea.style.alignItems = Align.Center;
            imageArea.style.backgroundColor = new StyleColor(Color.gray);
            imageArea.style.justifyContent = Justify.Center;
            // Set a fixed maximum size for the image area
            imageArea.style.maxHeight = 250;
            imageArea.style.maxWidth = 250;
            imageArea.style.height = Length.Percent(100);  // Let it use the remaining height
            imageArea.style.flexShrink = 0;  // Ensure the area does not shrink
            root.Add(imageArea);
            imageArea.Add(PreviewImage());

            VisualElement groupBox = new();
            // groupBox.style.position = Position.Relative;
            // groupBox.style.flexGrow = 1;
            groupBox.style.flexDirection = FlexDirection.Column;
            // groupBox.style.alignItems = Align.Center;
            groupBox.style.paddingBottom = 10;
            // groupBox.style.left = new StyleLength(new Length(25, LengthUnit.Percent));
            // groupBox.style.right = new StyleLength(new Length(5, LengthUnit.Percent));
            // groupBox.style.alignContent = Align.Center;
            // groupBox.style.justifyContent = Justify.Center;

            root.Add(groupBox);
            groupBox.Add(ImagePicker());
            groupBox.Add(ColorSelector());
            groupBox.Add(ApplyButton());
            bottomContentHeight = groupBox.resolvedStyle.height;

            root.UnregisterCallback<GeometryChangedEvent>(FitWindowToContent);
            root.RegisterCallback<GeometryChangedEvent>(FitWindowToContent);
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

            rootVisualElement.UnregisterCallback<GeometryChangedEvent>(FitWindowToContent);
        }

        private Image PreviewImage()
        {
            previewImage = new Image { scaleMode = ScaleMode.ScaleToFit };
            // previewImage.style.alignItems = Align.Center;
            // previewImage.style.backgroundColor = new StyleColor(Color.gray);
            // previewImage.style.justifyContent = Justify.Center;
            // // Set a fixed maximum size for the image area
            // previewImage.style.maxHeight = 250;
            // previewImage.style.maxWidth = 250;
            // previewImage.style.height = Length.Percent(100);  // Let it use the remaining height
            // previewImage.style.flexShrink = 0;  // Ensure the area does not shrink

            return previewImage;
        }

        private ObjectField ImagePicker()
        {
            imagePickerField = new ObjectField("Custom Icon:")
            {
                objectType = typeof(Texture2D),
                viewDataKey = "lastSelectedIcon" // Responsible for data persistence
            };
            imagePickerField.style.position = Position.Relative;
            imagePickerField.style.marginTop = 12;
            imagePickerField.style.flexGrow = 0;


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
            // colorField.style.position = Position.Absolute;
            // colorField.style.marginTop = 12;
            // colorField.style.bottom = 20;
            // colorField.style.right = 20;

            // colorField.style.paddingLeft = 16;
            // colorField.style.paddingRight = 16;
            colorField.RegisterValueChangedCallback(OnColorSelectChange);

            return colorField;
        }

        private Button ApplyButton()
        {
            Button applyButton = new(() => { ApplyColorizedIcon(); }) { text = "APPLY" };
            // applyButton.style.position = Position.Absolute;
            // applyButton.style.bottom = 10;
            // applyButton.style.right = 50;
            // applyButton.style.marginLeft = 55;
            // applyButton.style.marginRight = 55;
            // applyButton.style.marginBottom = 25;
            // applyButton.style.marginTop = 20;
            // applyButton.style.marginBottom = 20;
            // applyButton.style.borderBottomLeftRadius = 10;
            // applyButton.style.borderBottomRightRadius = 10;
            // applyButton.style.paddingBottom = 8;
            // applyButton.style.paddingTop = 8;
            // applyButton.style.fontSize = 15;

            return applyButton;
        }

        // For Editor v2021.3.16f1, default Script icon is substantially smaller than
        // it is in Editor v2022.3.24f1, hence, resizing this Window.
        private void FitWindowToContent(GeometryChangedEvent evt)
        {
            // return;
            // rootVisualElement.UnregisterCallback<GeometryChangedEvent>(FitWindowToContent);

            // Vector2 newSize1 = new(defaultWindowSize.x, rootVisualElement.contentRect.height);
            // window.minSize = newSize1;
            // Repaint();
            return;

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
            RepaintImagePickerWithoutNotify();

            if (evt.newValue != evt.previousValue)
                ResetColorPickerWithoutNotify();

            previewImage.sprite = iconGenerator.GetIconPreview(evt.newValue as Texture2D);

            // ResizeWindow(evt.newValue as Texture2D);
        }

        private void ResizeWindow(Texture2D texture)
        {
            if (!texture) return;
            rootVisualElement.schedule.Execute(() =>
            {
                // Calculate and set the window size based on image and bottom content size
                float windowWidth = Mathf.Max(texture.width, 200); // Set a minimum width if necessary
                float windowHeight = texture.height + bottomContentHeight;

                minSize = new Vector2(windowWidth, windowHeight);
                maxSize = minSize; // This will force the window to resize to the specified dimensions
            }).ExecuteLater(0); // Execute on the next frame after the layout pass
        }

        private void OnColorSelectChange(ChangeEvent<Color> evt)
        {
            previewImage.sprite = iconGenerator.GetIconPreview(evt.newValue);
        }

        private void ApplyColorizedIcon()
        {
            IconContext iconContext = new(iconGenerator.NewIconType,
                                          imagePickerField.value as Texture2D,
                                          ColorUtility.ToHtmlStringRGBA(colorField.value),
                                          iconGenerator.NewTintedTexture);

            string iconPath = new IconSaver().SaveIcon(iconContext);
            iconApplier.ChangeIcon(iconPath);
        }

        /// <summary>
        /// Repaints the ObjectField with the user selected texture
        /// without triggering the value change event.
        /// </summary>
        private void RepaintImagePickerWithoutNotify()
        {
            /*
                Necessary because domain reload visually shows 'None' selected
                but clicking the field or the dot on right shows it actually has
                a value assigned to it.

                Hence, refreshing/repainting the field to visually reflect the
                selection for better UX.
            */

            if (!imagePickerField.value) return;

            var storedValue = imagePickerField.value;
            imagePickerField.SetValueWithoutNotify(null); // Clearing to force the UI to refresh
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