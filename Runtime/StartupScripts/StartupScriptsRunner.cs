using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace UniKit
{
    [Preserve]
    public static class StartupScriptsRunner
    {
        private static StartupSettings Settings => StartupSettings.Data;
        private static IReadOnlyList<StartupScript> StartupScripts => Settings?.StartupScripts;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration() => RunLoadType(RuntimeInitializeLoadType.SubsystemRegistration);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void AfterAssembliesLoaded() => RunLoadType(RuntimeInitializeLoadType.AfterAssembliesLoaded);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void BeforeSplashScreen() => RunLoadType(RuntimeInitializeLoadType.BeforeSplashScreen);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void BeforeSceneLoad() => RunLoadType(RuntimeInitializeLoadType.BeforeSceneLoad);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AfterSceneLoad() => RunLoadType(RuntimeInitializeLoadType.AfterSceneLoad);

        private static void RunLoadType(RuntimeInitializeLoadType loadType) => StartupScripts?.WithLoadType(loadType).Run();
        public static IEnumerable<StartupScript> WithLoadType(this IEnumerable<StartupScript> startupScripts, RuntimeInitializeLoadType loadType)
            => startupScripts?.Where(ss => ss.LoadType == loadType);

        public static void Run(this IEnumerable<StartupScript> startupScripts)
        {
            if (startupScripts == null)
                return;

            foreach (var startupScript in startupScripts)
                startupScript.Run();
        }
    }
}
