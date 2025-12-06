using UnityEditor;

namespace BasementExperiments.ScriptIconColorizer
{
    public class ScriptIconChanger : Editor
    {
        [MenuItem("Assets/Script Icon/Change Icon %&#c", false, 10000)]
        private static void ChangeIcon()
        {
            CreateInstance<ScriptIconChangerWindow>().ShowWindow();
        }


        [MenuItem("Assets/Script Icon/Reset %&#r", false, 10001)]
        private static void ResetIcon()
        {
            new IconApplier().ResetIcon();
        }
    }
}