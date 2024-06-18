using UnityEditor;

namespace BasementExperiments.ScriptColorizer
{
    public class ScriptsColorizer : Editor
    {

        [MenuItem("Assets/Script Colorizer/Tint It!", false, 1000)]
        private static void ColorizeScripts() => CreateInstance<ScriptsColorizerWindow>().ShowWindow();


        [MenuItem("Assets/Script Colorizer/Reset", false, 1001)]
        private static void RemoveColorization() => new IconApplier().ResetIcon();
    }
}