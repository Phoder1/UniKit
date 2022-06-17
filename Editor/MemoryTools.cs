using UnityEditor;
using UnityEngine;

namespace Phoder1.Core
{
    public static class MemoryTools
    {
#if UNITY_EDITOR
        [MenuItem("Tools/Memory tools/Unload unused assets")]
        private static void UnloadUnusedAssets() => Resources.UnloadUnusedAssets();
#endif
    }
}
