using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace BasementExperiments.ScriptIconColorizer
{
    public static class Utils
    {
        public static List<Object> GetSelectedScripts()
        {
            Object[] selectedScripts = Selection.GetFiltered(typeof(MonoScript), SelectionMode.TopLevel);
            if (selectedScripts == null || selectedScripts.Length == 0)
                return null;

            return new List<Object>(selectedScripts);
        }

        public static IconType GetIconTypeFromColor(IconType iconType, Color tintColor)
        {
            IconType newIconType;

            if (tintColor != Color.white)
            {
                newIconType = iconType == IconType.DEFAULT ? IconType.DEFAULT_TINTED
                            : iconType == IconType.CUSTOM ? IconType.CUSTOM_TINTED
                            : iconType;
            }
            else
            {
                newIconType = iconType == IconType.DEFAULT_TINTED ? IconType.DEFAULT
                            : iconType == IconType.CUSTOM_TINTED ? IconType.CUSTOM
                            : iconType;
            }

            return newIconType;
        }

        public static bool NeedsTinting(IconType iconType)
        {
            return iconType == IconType.DEFAULT_TINTED || iconType == IconType.CUSTOM_TINTED;
        }
    }
}