using UnityEngine;

namespace Phoder1.Core.Attributes
{
    public class InlineAttribute : PropertyAttribute
    {
        public readonly bool UseOriginalLabel;

        public InlineAttribute(bool useOriginalLabel = false)
        {
            UseOriginalLabel = useOriginalLabel;
        }
    }
}
