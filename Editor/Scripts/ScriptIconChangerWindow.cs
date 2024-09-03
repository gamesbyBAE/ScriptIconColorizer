using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconChangerWindow : EditorWindow
    {
        [SerializeField] private StyleSheet customStyleSheet;

        private ObjectField imagePickerField;
        private ColorField colorSelectorField;
        private Image previewImage;

        private IconGenerator iconGenerator;
        private IconApplier iconApplier;
        private IconSaver iconSaver;

        private readonly Vector2 windowSize = new(250, 400);

        // Names must match with the one mentioned in the
        // USS to automatically apply custom styles.
        #region  USS Names
        private readonly string previewAreaName = "previewArea";
        private readonly string interactableAreaName = "interactableArea";
        private readonly string imagePickerName = "imagePicker";
        private readonly string colorSelectorName = "colorSelector";
        private readonly string applyButtonName = "applyButton";
        #endregion

        public void ShowWindow()
        {
            ShowUtility();

            maxSize = minSize = windowSize;

            // Changing the window's name and icon.
            // Note: Icon visible only on a regular dockable window & not on a UtilityWindow.
            Texture windowIcon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
            titleContent = new GUIContent("Custom Script Icon", windowIcon);
        }

        private void CreateGUI()
        {
            iconGenerator ??= new IconGenerator();
            ApplyStyleSheet();
            CreatePreviewArea();
            CreateInteractableArea();
        }

        private void OnDestroy()
        {
            iconGenerator = null;
            iconSaver = null;
            iconApplier = null;

            previewImage = null;

            imagePickerField?.UnregisterValueChangedCallback(ChangePreviewImage);
            imagePickerField = null;

            colorSelectorField?.UnregisterValueChangedCallback(ChangePreviewImageTint);
            colorSelectorField = null;

            Button applyButton = rootVisualElement.Query<Button>(applyButtonName);
            if (applyButton != null) applyButton.clicked -= ApplyNewIcon;
        }

        private void ApplyStyleSheet()
        {
            if (!customStyleSheet)
                Debug.Log("Custom Style Sheet not assigned!");
            else
                rootVisualElement.styleSheets.Add(customStyleSheet);
        }

        private void CreatePreviewArea()
        {
            VisualElement previewArea = new() { name = previewAreaName };
            previewArea.Add(PreviewImage());

            rootVisualElement.Add(previewArea);
        }

        private void CreateInteractableArea()
        {
            VisualElement controlsArea = new() { name = interactableAreaName };
            controlsArea.Add(ImagePicker());
            controlsArea.Add(ColorSelector());
            controlsArea.Add(ApplyButton());

            rootVisualElement.Add(controlsArea);
        }

        private Image PreviewImage()
        {
            previewImage = new Image { scaleMode = ScaleMode.ScaleToFit };
            return previewImage;
        }

        private ObjectField ImagePicker()
        {
            imagePickerField = new ObjectField("Custom Icon")
            {
                name = imagePickerName,
                objectType = typeof(Texture2D),
                viewDataKey = "lastSelectedIcon" // Responsible for data persistence
            };

            imagePickerField.RegisterValueChangedCallback(ChangePreviewImage);

            return imagePickerField;
        }

        private ColorField ColorSelector()
        {
            colorSelectorField = new ColorField("Icon Tint")
            {
                name = colorSelectorName,
                value = new Color(1, 1, 1, 1),
                viewDataKey = "lastSelectedColor", // Responsible for data persistence
            };

            colorSelectorField.RegisterValueChangedCallback(ChangePreviewImageTint);

            return colorSelectorField;
        }

        private Button ApplyButton()
        {
            Button applyButton = new(() => { ApplyNewIcon(); })
            {
                name = applyButtonName,
                text = "APPLY"
            };

            return applyButton;
        }

        private void ChangePreviewImage(ChangeEvent<Object> evt)
        {
            RepaintImagePickerWithoutNotify();

            if (evt.newValue != evt.previousValue)
                ResetColorPickerWithoutNotify();

            previewImage.sprite = iconGenerator.GetIconPreview(evt.newValue as Texture2D);
        }

        private void ChangePreviewImageTint(ChangeEvent<Color> evt)
        {
            previewImage.sprite = iconGenerator.GetIconPreview(evt.newValue);
        }

        private void ApplyNewIcon()
        {
            IconContext iconContext = new(iconGenerator.NewIconType,
                                          imagePickerField.value as Texture2D,
                                          ColorUtility.ToHtmlStringRGBA(colorSelectorField.value),
                                          iconGenerator.NewTintedTexture
                                          );

            iconSaver ??= new IconSaver();
            string iconPath = iconSaver.SaveIcon(iconContext);

            iconApplier ??= new IconApplier();
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
                but clicking the field or the dot on the right shows it actually
                has a value assigned to it.

                Hence, refreshing/repainting the field to visually reflect the
                selection for better UX.
            */

            if (!imagePickerField.value) return;

            Object storedValue = imagePickerField.value;
            imagePickerField.SetValueWithoutNotify(null); // Clearing to force the UI to refresh
            imagePickerField.SetValueWithoutNotify(storedValue);
        }

        /// <summary>
        /// Resets the ColorField to show White color without
        /// triggering the Value Change Event.
        /// </summary>
        private void ResetColorPickerWithoutNotify()
        {
            colorSelectorField.SetValueWithoutNotify(Color.white);
        }
    }
}