using UnityEditor;
using UnityEngine;

namespace UniKit.Core
{
    public static class MemoryTools
    {
#if UNITY_EDITOR
        [MenuItem("UniKit/Memory tools/Unload unused assets")]
        private static void UnloadUnusedAssets() => Resources.UnloadUnusedAssets();
#endif
    }
}
