#if UNITY_EDITOR

namespace Phoder1.Editor.Define
{
    using System;
    using System.Linq;
    using UnityEditor;

    /// <summary>
    /// Defines the ODIN_INSPECTOR symbol.
    /// </summary>
    internal static class EnsureOdinInspectorDefine
    {
        private static readonly string[] DEFINES = new string[] { "Phoder1Core" };

        [InitializeOnLoadMethod]
        private static void EnsureScriptingDefineSymbol()
        {
            var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;

            if (currentTarget == BuildTargetGroup.Unknown)
            {
                return;
            }

            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget).Trim();
            var defines = definesString.Split(';');

            bool changed = false;

            foreach (var define in DEFINES)
            {
                if (defines.Contains(define) == false)
                {
                    if (definesString.EndsWith(";", StringComparison.InvariantCulture) == false)
                    {
                        definesString += ";";
                    }

                    definesString += define;
                    changed = true;
                }
            }

            if (changed)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, definesString);
            }
        }
    }
}

#endif // UNITY_EDITOR