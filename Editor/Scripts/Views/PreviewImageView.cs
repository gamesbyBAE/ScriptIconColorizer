using UnityEngine.UIElements;
using UnityEngine;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class PreviewImageView : BaseView
    {
        private Image previewImage;

        private VisualElement rootElement;
        public override VisualElement RootElement => rootElement;

        public PreviewImageView(string ussClassName) : base(ussClassName)
        {
            previewImage = new Image
            {
                name = "previewImageView",
                scaleMode = ScaleMode.ScaleToFit,
                pickingMode = PickingMode.Ignore
            };

            Label label = new()
            {
                name = "previewImageLabel",
                text = "Preview"
            };

            rootElement = new VisualElement() { name = ussClassName };
            rootElement.Add(previewImage);
            rootElement.Add(label);
        }

        public void UpdatePreviewTexture(Texture2D previewTexture)
        {
            if (previewImage == null) return;
            previewImage.image = previewTexture;
        }

        public void UpdatePreviewTint(Color color)
        {
            if (previewImage == null) return;
            previewImage.tintColor = color;
        }

        public override void Cleanup()
        {
            previewImage = null;

            rootElement?.Clear();
            rootElement = null;
        }
    }
}