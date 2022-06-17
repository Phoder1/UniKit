using Sirenix.OdinInspector;
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
    [IncludeMyAttributes]
    [FoldoutGroup("Optional", GroupsOrder.Optional, Expanded = false)]
    public class OptionalsGroupAttribute : Attribute { }

    [IncludeMyAttributes]
    [FoldoutGroup("Events", GroupsOrder.Events, Expanded = false)]
    public class EventsGroupAttribute : Attribute { }

    [IncludeMyAttributes]
    [FoldoutGroup("Debug", GroupsOrder.Debug, Expanded = false)]
    public class DebugGroupAttribute : Attribute { }

}
