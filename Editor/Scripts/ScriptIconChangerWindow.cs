using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class ScriptIconChangerWindow : EditorWindow
    {
        [SerializeField] private StyleSheet customStyleSheet;

        private WindowPresenter windowPresenter;

        [MenuItem("Assets/Script Icon/Change Icon %&#c", false, 10000)]
        private static void ShowWindow()
        {
            var window = GetWindow<ScriptIconChangerWindow>(utility: true);
            window.ShowUtility();

            window.maxSize = window.minSize = new(250, 700);

            // Changing the window's name and icon.
            // Note: Icon visible only on a regular dockable window & not on a UtilityWindow.
            Texture windowIcon = EditorGUIUtility.IconContent("ClothInspector.PaintTool").image;
            window.titleContent = new GUIContent("Custom Script Icon", windowIcon);
        }

        /// <summary>
        /// Quick action to restore the icon of selected scripts to default without opening the window.
        /// </summary>
        [MenuItem("Assets/Script Icon/Restore Icon %&#r", false, 10001)]
        private static void ResetIcon()
        {
            new IconApplier().ResetIcon();
        }

        private void CreateGUI()
        {
            ApplyStyleSheet();
            windowPresenter = new WindowPresenter(rootVisualElement);
        }

        private void OnDestroy()
        {
            windowPresenter?.CleanUp();
            windowPresenter = null;
        }

        private void ApplyStyleSheet()
        {
            if (!customStyleSheet)
                Debug.LogWarning("[WARNING]: Custom Style Sheet not assigned!");
            else
                rootVisualElement.styleSheets.Add(customStyleSheet);
        }
    }
}