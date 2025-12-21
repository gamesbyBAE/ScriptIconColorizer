using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class TargetsListView : BaseView
    {
        private ListView targetScriptsListView;
        public List<Object> TargetScripts { get; private set; }

        private VisualElement rootElement;
        public override VisualElement RootElement => rootElement;

        public TargetsListView(string ussClassName, TargetScriptsPersistence targetsPersistence) : base(ussClassName)
        {
            TargetScripts = InitialiseList(targetsPersistence);

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
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight,
                horizontalScrollingEnabled = false,
                viewDataKey = "targetScriptsList" // Responsible for data persistence
            };

            rootElement = new VisualElement() { name = ussClassName };
            rootElement.Add(targetScriptsListView);

            RegisterDragAndDropHandlers();
        }

        private List<Object> InitialiseList(TargetScriptsPersistence targetsPersistence)
        {
            if (targetsPersistence != null)
            {
                List<Object> savedScripts = targetsPersistence.LoadScriptsFromPrefs();
                if (savedScripts.Count > 0)
                {
                    return savedScripts;
                }
            }

            List<Object> selectedScripts = Utils.GetSelectedScripts();
            return selectedScripts ?? new List<Object>();
        }

        private void RegisterDragAndDropHandlers()
        {
            if (rootElement == null) return;

            rootElement.RegisterCallback<DragLeaveEvent>(OnDragLeave);
            rootElement.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            rootElement.RegisterCallback<DragPerformEvent>(OnDrop);
        }

        private void UnregisterDragAndDropHandlers()
        {
            if (rootElement == null) return;

            rootElement.UnregisterCallback<DragLeaveEvent>(OnDragLeave);
            rootElement.UnregisterCallback<DragUpdatedEvent>(OnDragUpdated);
            rootElement.UnregisterCallback<DragPerformEvent>(OnDrop);
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
            scriptField.SetValueWithoutNotify(
                (TargetScripts != null && index < TargetScripts.Count)
                ? TargetScripts[index]
                : null);
            scriptField.RegisterValueChangedCallback(OnScriptFieldChanged);
        }

        private void OnScriptFieldChanged(ChangeEvent<Object> evt)
        {
            if (evt.target is not ObjectField scriptField
                || scriptField.userData is not int index
                || TargetScripts == null)
            {
                return;
            }

            if (index >= 0 && index < TargetScripts.Count)
                TargetScripts[index] = evt.newValue;
        }

        private void OnDragUpdated(DragUpdatedEvent evt)
        {
            if (DragAndDrop.objectReferences.Length > 0
                && DragAndDrop.objectReferences.Any(obj => obj is MonoScript))
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
                Debug.LogWarning("[WARNING]: No 'MonoScript' assets were dropped.");
                return;
            }

            TargetScripts?.AddRange(droppedMonoScripts);
            targetScriptsListView?.Rebuild();
            DragAndDrop.AcceptDrag();
        }

        public override void Cleanup()
        {
            TargetScripts?.Clear();
            TargetScripts = null;

            UnregisterDragAndDropHandlers();

            targetScriptsListView = null;
            rootElement = null;
        }
    }
}
