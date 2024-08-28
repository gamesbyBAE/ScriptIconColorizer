using UnityEditor;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconChanger : Editor
    {

        [MenuItem("Assets/Script Colorizer/Change Icon %&#c", false, 10000)]
        private static void ChangeIcon() => CreateInstance<ScriptIconChangerWindow>().ShowWindow();


        [MenuItem("Assets/Script Colorizer/Reset %&#r", false, 10001)]
        private static void ResetIcon() => new IconApplier().ResetIcon();
    }
}