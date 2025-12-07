using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconColorizer
{
    public class TargetsListView : BaseView
    {
        private ListView targetScriptsListView;
        public List<Object> TargetScripts { get; private set; }

        private readonly VisualElement rootElement;
        public override VisualElement RootElement => rootElement;

        public TargetsListView(string ussClassName) : base(ussClassName)
        {
            var selectedScripts = Utils.GetSelectedScripts();
            if (selectedScripts != null)
                TargetScripts = new List<Object>(selectedScripts);
            else
                TargetScripts = new List<Object>();


            targetScriptsListView = new ListView(
                itemsSource: TargetScripts,
                itemHeight: 40,
                makeItem: MakeScriptFieldItem,
                bindItem: BindScriptFieldItem)
            {
                name = "targetsListView",
                showFoldoutHeader = true,
                headerTitle = "Target Scripts",
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                showAddRemoveFooter = true,
                showBorder = true,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                selectionType = SelectionType.Multiple,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                horizontalScrollingEnabled = false,
            };

            rootElement = new VisualElement() { name = ussClassName };
            rootElement.Add(targetScriptsListView);

            RegisterDragAndDropHandlers();
        }

        private void RegisterDragAndDropHandlers()
        {
            if (targetScriptsListView == null) return;
            targetScriptsListView.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            targetScriptsListView.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            targetScriptsListView.RegisterCallback<DragPerformEvent>(OnDrop);
        }

        private void UnregisterDragAndDropHandlers()
        {
            if (targetScriptsListView == null) return;
            targetScriptsListView.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            targetScriptsListView.UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            targetScriptsListView.UnregisterCallback<DragPerformEvent>(OnDrop);
        }

        private VisualElement MakeScriptFieldItem()
        {
            ObjectField scriptField = new()
            {
                objectType = typeof(MonoScript),
                style =
                {
                    flexGrow = 1,
                    flexShrink = 1,
                    flexBasis = 0,
                }
            };

            return scriptField;
        }

        private void BindScriptFieldItem(VisualElement element, int index)
        {
            if (element is not ObjectField scriptField) return;

            scriptField.userData = index;
            scriptField.UnregisterValueChangedCallback(OnScriptFieldChanged);
            scriptField.SetValueWithoutNotify(index < TargetScripts.Count ? TargetScripts[index] : null);
            scriptField.RegisterValueChangedCallback(OnScriptFieldChanged);
        }

        private void OnScriptFieldChanged(ChangeEvent<Object> evt)
        {
            if (evt.target is ObjectField scriptField && scriptField.userData is int index)
                if (index >= 0 && index < TargetScripts.Count)
                    TargetScripts[index] = evt.newValue;
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (DragAndDrop.objectReferences.Length > 0 &&
                DragAndDrop.objectReferences.Any(obj => obj is MonoScript))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                return;
            }

            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        }

        private void OnDragLeave(DragLeaveEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        }

        private void OnDrop(DragPerformEvent evt)
        {
            var droppedMonoScripts = DragAndDrop.objectReferences.OfType<MonoScript>().ToList();
            if (droppedMonoScripts == null || droppedMonoScripts.Count <= 0)
            {
                Debug.LogWarning("No 'MonoScript' assets were dropped.");
                return;
            }

            TargetScripts.AddRange(droppedMonoScripts);
            targetScriptsListView.Rebuild();
            DragAndDrop.AcceptDrag();
        }

        public override void Cleanup()
        {
            TargetScripts?.Clear();
            TargetScripts = null;

            UnregisterDragAndDropHandlers();

            targetScriptsListView = null;
        }
    }
}
