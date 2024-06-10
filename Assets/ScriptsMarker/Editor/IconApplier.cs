using UnityEditor;
using UnityEngine;

public class IconApplier
{
    public void ChangeIcon(string iconPath) => ApplyIcon(AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath));
    public void ResetIcon() => ApplyIcon(null);

    private void ApplyIcon(Texture2D iconToApply)
    {
        Object[] selectedScripts = Selection.GetFiltered(typeof(MonoScript), SelectionMode.TopLevel);

        if (selectedScripts == null || selectedScripts.Length == 0)
        {
            Debug.LogWarning("No SCRIPT selected!");
            return;
        }

        for (int i = 0; i < selectedScripts.Length; i++)
            SetCustomIconOnMonoScript(selectedScripts[i], iconToApply);
    }

    private void SetCustomIconOnMonoScript(Object selectedScript, Texture2D icon)
    {
        string scriptPath = AssetDatabase.GetAssetPath(selectedScript);
        MonoImporter monoImporter = (MonoImporter)AssetImporter.GetAtPath(scriptPath);
        monoImporter.SetIcon(icon);
        monoImporter.SaveAndReimport(); //TODO: Can we Reimport all at once later on or has to be right now?
        // TODO: SaveAndReimport itself uses AssetDatabase.ImportAsset()
    }
}
