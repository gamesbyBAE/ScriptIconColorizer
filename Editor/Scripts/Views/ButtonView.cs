using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class ButtonView : BaseView
    {
        private Button button;
        public override VisualElement RootElement => button;

        // Event to notify when the button is clicked
        public event System.Action OnClick;

        public ButtonView(string buttonText, string ussClassName) : base(ussClassName)
        {
            button = new Button(HandleClick)
            {
                name = ussClassName,
                text = buttonText,
                focusable = false,
            };
        }

        private void HandleClick() => OnClick?.Invoke();

        public override void Cleanup()
        {
            if (button == null) return;

            button.clicked -= HandleClick;
            button = null;
        }
    }
}