using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptColorizer
{
    public class IconApplier
    {
        public void ChangeIcon(string iconPath)
        {
            ApplyIcon(AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath));
        }

        public void ResetIcon()
        {
            ApplyIcon(null);
        }

        private void ApplyIcon(Texture2D iconToApply)
        {
            Object[] selectedScripts = Selection.GetFiltered(typeof(MonoScript), SelectionMode.TopLevel);
            if (selectedScripts == null || selectedScripts.Length == 0)
            {
                Debug.LogWarning("No SCRIPT selected!");
                return;
            }

            // All this so the reimports are done only at the end.
            try
            {
                AssetDatabase.StartAssetEditing();
                for (int i = 0; i < selectedScripts.Length; i++)
                    SetCustomIconOnMonoScript(selectedScripts[i], iconToApply);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error during scripted asset import: {e.Message}");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private void SetCustomIconOnMonoScript(Object selectedScript, Texture2D icon)
        {
            string scriptPath = AssetDatabase.GetAssetPath(selectedScript);
            MonoImporter monoImporter = (MonoImporter)AssetImporter.GetAtPath(scriptPath);
            monoImporter.SetIcon(icon);
            monoImporter.SaveAndReimport();
        }
    }
}