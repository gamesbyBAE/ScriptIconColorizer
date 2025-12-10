using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class ColorPickerView : BaseView
    {
        private ColorField colorPickerField;

        // Properties
        public Color SelectedColor => colorPickerField?.value ?? Color.white;
        public override VisualElement RootElement => colorPickerField;

        // Invoked Events
        public event System.Action<Color> OnColorChanged;

        public ColorPickerView(string ussClassName) : base(ussClassName)
        {
            colorPickerField = new ColorField()
            {
                name = ussClassName,
                value = Color.white,
                showAlpha = true,
                tooltip = "Pick a tint color.",
                viewDataKey = "lastSelectedColor" // Responsible for data persistence
            };

            colorPickerField.RegisterValueChangedCallback(HandlePickerValueChange);
        }

        private void HandlePickerValueChange(ChangeEvent<Color> evt)
        {
            OnColorChanged?.Invoke(evt.newValue);
        }

        public void ResetPickerWithoutNotify(Color color)
        {
            colorPickerField?.SetValueWithoutNotify(color);
        }

        public override void Cleanup()
        {
            colorPickerField?.UnregisterValueChangedCallback(HandlePickerValueChange);
            colorPickerField = null;
        }
    }
}