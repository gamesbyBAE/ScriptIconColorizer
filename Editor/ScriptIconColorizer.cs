using UnityEditor;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconColorizer : Editor
    {

        [MenuItem("Assets/Script Colorizer/Tint It! %&#c", false, 10000)]
        private static void ColorizeScripts() => CreateInstance<ScriptIconColorizerWindow>().ShowWindow();


        [MenuItem("Assets/Script Colorizer/Reset %&#r", false, 10001)]
        private static void RemoveColorization() => new IconApplier().ResetIcon();
    }
}