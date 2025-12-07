using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconColorizer
{
    public abstract class BaseView
    {
        public abstract VisualElement RootElement { get; }
        protected BaseView(string ussClassName) { }
        public abstract void Cleanup();
    }
}