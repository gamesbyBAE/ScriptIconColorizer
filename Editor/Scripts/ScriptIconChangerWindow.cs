using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconChangerWindow : EditorWindow
    {
        [SerializeField] private StyleSheet customStyleSheet;

        private PreviewImageView previewImageView;
        private ImagePickerView imagePickerView;
        private ColorPickerView colorPickerView;
        private TargetsListView targetsListView;
        private ButtonView applyButtonView;
        private ButtonView resetButtonView;

        private IconGenerator iconGenerator;
        private IconApplier iconApplier;
        private IconSaver iconSaver;

        private readonly Vector2 windowSize = new(250, 700);

        // Names must match with the one mentioned in the
        // USS to automatically apply custom styles.
        #region  USS Names
        private readonly string previewAreaName = "previewArea";
        private readonly string interactableAreaName = "interactableArea";
        private readonly string imagePickerName = "imagePicker";
        private readonly string colorSelectorName = "colorSelector";
        private readonly string scriptListAreaName = "scriptListArea";
        private readonly string applyButtonName = "applyButton";
        private readonly string resetButtonName = "resetButton";
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

            VisualElement previewArea = SetupPreviewArea();
            VisualElement interactableArea = SetupInteractableArea();

            rootVisualElement.Add(previewArea);
            rootVisualElement.Add(interactableArea);
        }

        private void OnDestroy()
        {
            iconGenerator = null;
            iconSaver = null;
            iconApplier = null;

            previewImageView?.Cleanup();
            previewImageView = null;

            if (imagePickerView != null)
            {
                imagePickerView.OnImageChanged -= ChangePreviewImage;
                imagePickerView.Cleanup();
                imagePickerView = null;
            }

            if (colorPickerView != null)
            {
                colorPickerView.OnColorChanged -= ChangePreviewImageTint;
                colorPickerView?.Cleanup();
                colorPickerView = null;
            }

            targetsListView?.Cleanup();
            targetsListView = null;

            if (applyButtonView != null)
            {
                applyButtonView.OnClick -= ApplyNewIcon;
                applyButtonView?.Cleanup();
                applyButtonView = null;
            }

            if (resetButtonView != null)
            {
                resetButtonView.OnClick -= ResetIcon;
                resetButtonView?.Cleanup();
                resetButtonView = null;
            }
        }

        private void ApplyStyleSheet()
        {
            if (!customStyleSheet)
                Debug.LogWarning("Custom Style Sheet not assigned!");
            else
                rootVisualElement.styleSheets.Add(customStyleSheet);
        }

        private VisualElement SetupPreviewArea()
        {
            previewImageView ??= new PreviewImageView(ussClassName: previewAreaName);
            return previewImageView.RootElement;
        }

        private VisualElement SetupInteractableArea()
        {
            imagePickerView ??= new ImagePickerView(ussClassName: imagePickerName);
            colorPickerView ??= new ColorPickerView(ussClassName: colorSelectorName);
            targetsListView ??= new TargetsListView(ussClassName: scriptListAreaName);
            applyButtonView ??= new ButtonView("Apply", ussClassName: applyButtonName);
            resetButtonView ??= new ButtonView("Reset", ussClassName: resetButtonName);

            imagePickerView.OnImageChanged += ChangePreviewImage;
            colorPickerView.OnColorChanged += ChangePreviewImageTint;
            applyButtonView.OnClick += ApplyNewIcon;
            resetButtonView.OnClick += ResetIcon;

            // Initialising Preview;
            ChangePreviewImage(imagePickerView.SelectedTexture);

            VisualElement controlsArea = new() { name = interactableAreaName };
            controlsArea.Add(imagePickerView.RootElement);
            controlsArea.Add(colorPickerView.RootElement);
            controlsArea.Add(targetsListView.RootElement);
            controlsArea.Add(applyButtonView.RootElement);
            controlsArea.Add(resetButtonView.RootElement);
            return controlsArea;
        }

        private void ChangePreviewImage(Texture2D newTexture)
        {
            if (!newTexture)
                newTexture = iconGenerator?.DefaultIcon;

            if (previewImageView != null)
            {
                previewImageView.UpdatePreviewTexture(newTexture);
                previewImageView.UpdatePreviewTint(Color.white);
            }

            colorPickerView?.ResetPickerWithoutNotify(Color.white);
        }

        private void ChangePreviewImageTint(Color newColor)
        {
            previewImageView?.UpdatePreviewTint(newColor);
        }

        private void ApplyNewIcon()
        {
            IconContext iconContext = iconGenerator.GenerateIconAndGetContext(
                imagePickerView?.SelectedTexture,
                colorPickerView?.SelectedColor ?? Color.white);

            iconSaver ??= new IconSaver();
            string iconPath = iconSaver.SaveIcon(iconContext);
            Debug.LogFormat($"Icon saved at path: {iconPath}");

            iconApplier ??= new IconApplier();
            iconApplier.ChangeIcon(iconPath, targetsListView.TargetScripts);
        }

        private void ResetIcon()
        {
            iconApplier ??= new IconApplier();
            iconApplier.ResetIcon(targetsListView.TargetScripts);
        }
    }
}