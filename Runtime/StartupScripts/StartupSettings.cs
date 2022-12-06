using System.Collections.Generic;
using UnityEngine;

namespace UniKit
{
    public class StartupSettings : SingletonScriptableObject<StartupSettings>
    {
        [SerializeField]
        private ScriptCollection<StartupScript> startupScripts;
        public ScriptCollection<StartupScript> StartupScripts => startupScripts;
    }
}
