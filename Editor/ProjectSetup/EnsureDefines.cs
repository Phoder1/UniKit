#if UNITY_EDITOR

namespace Phoder1.Editor.Define
{
    using System;
    using System.Linq;
    using UnityEditor;

    /// <summary>
    /// Automatically adds a define to Unity with the package's name
    /// </summary>
    internal static class EnsureDefines
    {
        private static readonly string define
            = typeof(EnsureDefines)
            .Assembly
            .FindContainingPackage()
            .displayName
            .RemoveSpecialCharacters()
            .SpaceToUnderScore()
            .RemoveAllWhitespaces();

        [InitializeOnLoadMethod]
        private static void EnsureScriptingDefineSymbol()
        {
            using (DefinesStream stream = new DefinesStream())
            {
                stream.AddDefine(define);
            }
        }
    }
}

#endif // UNITY_EDITOR