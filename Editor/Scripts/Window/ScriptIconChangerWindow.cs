using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
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
        private TargetScriptsPersistence targetsPersistence;

        private List<BaseView> allViews;

        private readonly Vector2 windowSize = new(250, 700);

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
            targetsPersistence ??= new TargetScriptsPersistence();
            allViews ??= new List<BaseView>();

            ApplyStyleSheet();

            CreateViews();

            // TODO: One single layout method that further calls sub-layout methods.
            VisualElement previewArea = SetupPreviewArea();
            VisualElement interactableArea = BuildLayout();

            rootVisualElement.Add(previewArea);
            rootVisualElement.Add(interactableArea);

            // Branding Footer
            Label footerLabel = new()
            {
                name = UssNames.Footer,
                text = "› Basement Experiments ‹"
            };
            rootVisualElement.Add(footerLabel);


            // Initialsing Preview Image
            ChangePreviewImage(imagePickerView.SelectedTexture);

            RegisterViews(new BaseView[]
                {
                    previewImageView,
                    imagePickerView,
                    colorPickerView,
                    targetsListView,
                    applyButtonView,
                    resetButtonView
                });
        }

        private void OnDestroy()
        {
            iconGenerator = null;
            iconSaver = null;
            iconApplier = null;

            targetsPersistence?.DeleteSavedGuids();
            targetsPersistence = null;

            // Unsubscribing
            if (imagePickerView != null)
                imagePickerView.OnImageChanged -= ChangePreviewImage;

            if (colorPickerView != null)
                colorPickerView.OnColorChanged -= ChangePreviewImageTint;

            if (applyButtonView != null)
                applyButtonView.OnClick -= ApplyNewIcon;

            if (resetButtonView != null)
                resetButtonView.OnClick -= ResetIcon;

            // Calling CleanUp() of all the views.
            if (allViews == null)
                return;

            for (int i = 0; i < allViews.Count; i++)
                allViews[i]?.Cleanup();

            allViews.Clear();
            allViews = null;
        }

        private void ApplyStyleSheet()
        {
            if (!customStyleSheet)
                Debug.LogWarning("[WARNING]: Custom Style Sheet not assigned!");
            else
                rootVisualElement.styleSheets.Add(customStyleSheet);
        }

        private void RegisterViews(params BaseView[] views)
        {
            allViews ??= new List<BaseView>();
            foreach (var v in views)
                if (v != null && !allViews.Contains(v))
                    allViews.Add(v);
        }

        private VisualElement SetupPreviewArea()
        {
            previewImageView ??= new PreviewImageView(ussClassName: UssNames.PreviewArea);
            return previewImageView.RootElement;
        }

        private void CreateViews()
        {
            imagePickerView ??= new ImagePickerView(ussClassName: UssNames.ImagePicker);
            colorPickerView ??= new ColorPickerView(ussClassName: UssNames.ColorSelector);
            targetsListView ??= new TargetsListView(ussClassName: UssNames.ScriptListArea, targetsPersistence);
            applyButtonView ??= new ButtonView("APPLY", ussClassName: UssNames.ApplyButton);
            resetButtonView ??= new ButtonView("RESTORE", ussClassName: UssNames.ResetButton);

            imagePickerView.OnImageChanged += ChangePreviewImage;
            colorPickerView.OnColorChanged += ChangePreviewImageTint;
            applyButtonView.OnClick += ApplyNewIcon;
            resetButtonView.OnClick += ResetIcon;
        }

        private VisualElement BuildLayout()
        {
            VisualElement controlsArea = new() { name = UssNames.InteractableArea };

            // Create a horizontal row for image & color pickers
            VisualElement pickerRow = new() { name = "pickerRow" };
            pickerRow.Add(imagePickerView.RootElement);
            pickerRow.Add(colorPickerView.RootElement);
            controlsArea.Add(pickerRow);

            controlsArea.Add(targetsListView.RootElement);

            // Create a horizontal row for action buttons
            VisualElement actionRow = new() { name = "actionButtonsRow" };
            actionRow.Add(resetButtonView.RootElement);
            actionRow.Add(applyButtonView.RootElement);
            controlsArea.Add(actionRow);

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
            targetsPersistence.SaveGuidsToPrefs(targetsListView.TargetScripts);

            IconContext iconContext = iconGenerator.GenerateIconAndGetContext(
                imagePickerView?.SelectedTexture,
                colorPickerView?.SelectedColor ?? Color.white);

            // Skip if trying to apply the default icon (no changes).
            if (iconContext.IconType == IconType.DEFAULT
                && iconContext.TintColor == Color.white)
            {
                return;
            }

            iconSaver ??= new IconSaver();
            string iconPath = iconSaver.SaveIcon(iconContext);

            iconApplier ??= new IconApplier();
            iconApplier.ChangeIcon(iconPath, targetsListView.TargetScripts);
        }

        private void ResetIcon()
        {
            targetsPersistence.SaveGuidsToPrefs(targetsListView.TargetScripts);

            iconApplier ??= new IconApplier();
            iconApplier.ResetIcon(targetsListView.TargetScripts);
        }
    }
}