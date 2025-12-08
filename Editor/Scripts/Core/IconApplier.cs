using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class IconApplier
    {
        public void ChangeIcon(string iconPath, List<Object> targetScripts)
        {
            if (string.IsNullOrEmpty(iconPath))
            {
                Debug.LogError("Icon Change Failed: Icon path is null!");
                return;
            }

            try
            {
                Texture2D iconToApply = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
                ApplyIcon(iconToApply, targetScripts);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error during scripted asset import: {e.Message}");
            }
        }

        public void ResetIcon()
        {
            ApplyIcon(null, Utils.GetSelectedScripts());
        }

        public void ResetIcon(List<Object> targetScripts)
        {
            ApplyIcon(null, targetScripts);
        }

        private void ApplyIcon(Texture2D iconToApply, List<Object> targetScripts)
        {
            if (targetScripts == null || targetScripts.Count == 0)
            {
                Debug.LogError("Icon Change Failed: No 'MonoScript' assets selected!");
                return;
            }

            // Batching reimports so that they are executed only at the end.
            try
            {
                AssetDatabase.StartAssetEditing();
                for (int i = 0; i < targetScripts.Count; i++)
                    if (targetScripts[i])
                        SetScriptIcon(targetScripts[i], iconToApply);
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

        private void SetScriptIcon(Object selectedScript, Texture2D icon)
        {
            string scriptPath = AssetDatabase.GetAssetPath(selectedScript);
            MonoImporter monoImporter = (MonoImporter)AssetImporter.GetAtPath(scriptPath);
            monoImporter.SetIcon(icon);
            monoImporter.SaveAndReimport();
        }
    }
}