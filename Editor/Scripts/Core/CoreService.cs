using System.Collections.Generic;
using UnityEngine;

namespace BasementExperiments.ScriptIconCustomiser
{
    public class CoreService
    {
        private IconGenerator iconGenerator;
        private IconSaver iconSaver;
        private IconApplier iconApplier;
        private TargetScriptsPersistence targetsPersistence;

        public Texture2D DefaultIconTexture => iconGenerator?.DefaultIcon;
        public TargetScriptsPersistence TargetsPersistence => targetsPersistence;

        public CoreService()
        {
            iconGenerator ??= new IconGenerator();
            iconSaver ??= new IconSaver();
            iconApplier ??= new IconApplier();
            targetsPersistence ??= new TargetScriptsPersistence();
        }

        public void ApplyIcon(Texture2D iconTexture, Color tintColor, List<Object> targetScripts)
        {
            if (targetScripts == null || targetScripts.Count == 0)
                return;

            targetsPersistence?.SaveGuidsToPrefs(targetScripts);

            IconContext iconContext = iconGenerator?.GenerateIconAndGetContext(iconTexture, tintColor);

            // Skip if trying to apply the default icon (no changes).
            if (iconContext != null
                && iconContext.IconType == IconType.DEFAULT
                && iconContext.TintColor == Color.white)
            {
                return;
            }

            string iconPath = iconSaver?.SaveIcon(iconContext);

            iconApplier?.ChangeIcon(iconPath, targetScripts);
        }

        public void ResetIcon(List<Object> targetScripts)
        {
            if (targetScripts == null || targetScripts.Count == 0)
                return;

            targetsPersistence?.SaveGuidsToPrefs(targetScripts);

            iconApplier?.ResetIcon(targetScripts);
        }

        public void CleanUp()
        {
            iconGenerator = null;
            iconSaver = null;
            iconApplier = null;

            targetsPersistence?.DeleteSavedGuids();
            targetsPersistence = null;
        }
    }
}