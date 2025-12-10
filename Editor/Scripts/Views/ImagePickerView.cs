using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class ImagePickerView : BaseView
    {
        private ObjectField imagePickerField;

        // Properties
        public Texture2D SelectedTexture => imagePickerField?.value as Texture2D;
        public override VisualElement RootElement => imagePickerField;

        // Invoked Events
        public event System.Action<Texture2D> OnImageChanged;

        public ImagePickerView(string ussClassName) : base(ussClassName)
        {
            imagePickerField = new ObjectField()
            {
                name = ussClassName,
                objectType = typeof(Texture2D),
                tooltip = "Pick a custom icon image.",
                viewDataKey = "lastSelectedIcon" // Responsible for data persistence
            };

            imagePickerField.RegisterValueChangedCallback(HandlePickerValueChange);
        }

        private void HandlePickerValueChange(ChangeEvent<Object> evt)
        {
            RepaintWithoutNotify();
            OnImageChanged?.Invoke(evt.newValue as Texture2D);
        }

        /// <summary>
        /// Repaints the ObjectField with the user selected texture
        /// without triggering the value change event.
        /// </summary>
        private void RepaintWithoutNotify()
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

        public override void Cleanup()
        {
            if (imagePickerField == null) return;

            imagePickerField.UnregisterValueChangedCallback(HandlePickerValueChange);
            imagePickerField = null;
        }
    }
}