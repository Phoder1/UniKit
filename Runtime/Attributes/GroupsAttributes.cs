#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;

namespace Phoder1
{
    public static class GroupsOrder
    {
        public const int Optional = 10;
        public const int Refrences = 998;
        public const int Events = 999;
        public const int Debug = 9999;
    }
#if ODIN_INSPECTOR
    [IncludeMyAttributes]
    [FoldoutGroup("Optional", GroupsOrder.Optional, Expanded = false)]
#endif
    public class OptionalsGroupAttribute : Attribute { }

#if ODIN_INSPECTOR
    [IncludeMyAttributes]
    [FoldoutGroup("Events", GroupsOrder.Events, Expanded = false)]
#endif
    public class EventsGroupAttribute : Attribute { }

#if ODIN_INSPECTOR
    [IncludeMyAttributes]
    [FoldoutGroup("Debug", GroupsOrder.Debug, Expanded = false)]
#endif
    public class DebugGroupAttribute : Attribute { }

}
