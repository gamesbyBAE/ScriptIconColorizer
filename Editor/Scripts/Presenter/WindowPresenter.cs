using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class WindowPresenter
    {
        private CoreService coreService;

        private readonly VisualElement rootElement;

        private List<BaseView> allViews;
        private PreviewImageView previewImageView;
        private ImagePickerView imagePickerView;
        private ColorPickerView colorPickerView;
        private TargetsListView targetsListView;
        private ButtonView applyButtonView;
        private ButtonView resetButtonView;

        public WindowPresenter(VisualElement root)
        {
            rootElement = root;
            coreService = new CoreService();

            CreateViews();
            SubscribeToViewEvents();
            RegisterViews(new BaseView[]
                {
                    previewImageView,
                    imagePickerView,
                    colorPickerView,
                    targetsListView,
                    applyButtonView,
                    resetButtonView
                });

            BuildLayout();

            // Initialsing Preview Image
            ChangePreviewImage(imagePickerView?.SelectedTexture);
        }

        public void CleanUp()
        {
            coreService?.CleanUp();
            coreService = null;

            UnsubscribeFromViewEvents();
            CleanupViews();

            rootElement?.Clear();
        }

        private void CreateViews()
        {
            previewImageView ??= new PreviewImageView(UssNames.PreviewArea);
            imagePickerView ??= new ImagePickerView(UssNames.ImagePicker);
            colorPickerView ??= new ColorPickerView(UssNames.ColorSelector);
            targetsListView ??= new TargetsListView(UssNames.ScriptListArea, coreService.TargetsPersistence);
            applyButtonView ??= new ButtonView("APPLY", UssNames.ApplyButton);
            resetButtonView ??= new ButtonView("RESTORE", UssNames.ResetButton);
        }

        private void SubscribeToViewEvents()
        {
            if (imagePickerView != null)
                imagePickerView.OnImageChanged += ChangePreviewImage;

            if (colorPickerView != null)
                colorPickerView.OnColorChanged += ChangePreviewImageTint;

            if (applyButtonView != null)
                applyButtonView.OnClick += ApplyNewIcon;

            if (resetButtonView != null)
                resetButtonView.OnClick += ResetIcon;
        }

        private void UnsubscribeFromViewEvents()
        {
            if (imagePickerView != null)
                imagePickerView.OnImageChanged -= ChangePreviewImage;

            if (colorPickerView != null)
                colorPickerView.OnColorChanged -= ChangePreviewImageTint;

            if (applyButtonView != null)
                applyButtonView.OnClick -= ApplyNewIcon;

            if (resetButtonView != null)
                resetButtonView.OnClick -= ResetIcon;
        }

        private void RegisterViews(params BaseView[] views)
        {
            allViews ??= new List<BaseView>();
            foreach (var v in views)
                if (v != null && !allViews.Contains(v))
                    allViews.Add(v);
        }

        private void CleanupViews()
        {
            if (allViews == null || allViews.Count == 0)
                return;

            for (int i = 0; i < allViews.Count; i++)
                allViews[i]?.Cleanup();

            allViews.Clear();
            allViews = null;
        }


        // Building Layout
        private void BuildLayout()
        {
            rootElement.Add(BuildPreviewArea());
            rootElement.Add(BuildInteractiveArea());
            rootElement.Add(BuildFooter());
        }

        private VisualElement BuildPreviewArea()
        {
            return previewImageView?.RootElement;
        }

        private VisualElement BuildInteractiveArea()
        {
            // Root container for all interactive controls
            VisualElement controlsArea = new() { name = UssNames.InteractableArea };

            // Horizontal row for Image & Color pickers
            VisualElement pickersRow = new() { name = UssNames.PickersRow };
            pickersRow.Add(imagePickerView?.RootElement);
            pickersRow.Add(colorPickerView?.RootElement);
            controlsArea.Add(pickersRow);

            // Target Scripts List
            controlsArea.Add(targetsListView?.RootElement);

            // Horizontal row for action buttons
            VisualElement actionRow = new() { name = UssNames.ActionButtonsRow };
            actionRow.Add(resetButtonView?.RootElement);
            actionRow.Add(applyButtonView?.RootElement);
            controlsArea.Add(actionRow);

            return controlsArea;
        }

        private VisualElement BuildFooter()
        {
            Label footerLabel = new()
            {
                name = UssNames.Footer,
                text = "› Basement Experiments ‹"
            };

            return footerLabel;
        }


        // Event Handlers
        private void ChangePreviewImage(Texture2D newTexture)
        {
            if (!newTexture)
                newTexture = coreService?.DefaultIconTexture;

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
            coreService?.ApplyIcon(
                imagePickerView?.SelectedTexture,
                colorPickerView?.SelectedColor ?? Color.white,
                targetsListView?.TargetScripts);
        }

        private void ResetIcon()
        {
            coreService?.ResetIcon(targetsListView?.TargetScripts);
        }
    }
}