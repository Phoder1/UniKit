using System.Collections.Generic;
using UnityEngine;

namespace UniKit
{
    public class StartupSettings : SingletonScriptableObject<StartupSettings>
    {
        [SerializeField]
        private StartupScript[] startupScripts;
        [SerializeField]
        private ScriptCollection<StartupScript> startupScriptsCollection;
        public IReadOnlyList<StartupScript> StartupScripts => startupScripts;
    }
}
