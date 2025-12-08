using UnityEditor;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class ScriptIconCustomiser : Editor
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