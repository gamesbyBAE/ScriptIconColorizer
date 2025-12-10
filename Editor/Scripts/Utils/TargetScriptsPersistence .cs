using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BasementExperiments.ScriptIconCustomiser
{
    /// <summary>
    /// Handles persistence of target script references using their GUIDs stored in EditorPrefs.
    /// Only way to persist target script references between domain reloads.
    /// </summary>
    public class TargetScriptsPersistence
    {
        private readonly string editorPrefsKey = "TargetScriptGuids";

        /// <summary>
        /// Converts List of Objects to List of GUIDs and saves them to EditorPrefs.
        /// </summary>
        public void SaveGuidsToPrefs(List<Object> scripts)
        {
            List<string> guids = ConvertScriptsToGuids(scripts);
            string json = JsonUtility.ToJson(new GuidWrapper { guids = guids }, false);
            EditorPrefs.SetString(editorPrefsKey, json);
        }

        /// <summary>
        /// Loads GUIDs from EditorPrefs and converts them back to List of Objects
        /// </summary>
        public List<Object> LoadScriptsFromPrefs()
        {
            string json = EditorPrefs.GetString(editorPrefsKey, "");
            if (string.IsNullOrEmpty(json))
                return new List<Object>();

            try
            {
                GuidWrapper wrapper = JsonUtility.FromJson<GuidWrapper>(json);
                return ConvertGuidsToScripts(wrapper.guids);
            }
            catch
            {
                Debug.LogWarning("[WARNING]: Failed to load saved scripts from EditorPrefs.");
                return new List<Object>();
            }
        }

        /// <summary>
        /// Converts List of Objects to List of GUIDs
        /// </summary>
        private List<string> ConvertScriptsToGuids(List<Object> scripts)
        {
            List<string> guids = new();
            if (scripts == null || scripts.Count == 0)
                return guids;

            foreach (var script in scripts)
            {
                if (script == null) continue;

                string path = AssetDatabase.GetAssetPath(script);
                string guid = AssetDatabase.AssetPathToGUID(path);
                if (!string.IsNullOrEmpty(guid))
                    guids.Add(guid);
            }

            return guids;
        }

        /// <summary>
        /// Converts List of GUIDs to List of Objects
        /// </summary>
        private List<Object> ConvertGuidsToScripts(List<string> guids)
        {
            List<Object> scripts = new();
            if (guids == null || guids.Count == 0)
                return scripts;

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Object script = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (script != null)
                    scripts.Add(script);
            }

            return scripts;
        }

        /// <summary>
        /// Clears all saved GUIDs from EditorPrefs
        /// </summary>
        public void DeleteSavedGuids()
        {
            EditorPrefs.DeleteKey(editorPrefsKey);
        }

        /// <summary>
        /// Helper class for JSON serialization of GUIDs list
        /// </summary>
        [System.Serializable]
        private class GuidWrapper
        {
            public List<string> guids = new();
        }
    }
}