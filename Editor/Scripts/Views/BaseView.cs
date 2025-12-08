using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
{
    public abstract class BaseView
    {
        public abstract VisualElement RootElement { get; }
        protected BaseView(string ussClassName) { }
        public abstract void Cleanup();
    }
}