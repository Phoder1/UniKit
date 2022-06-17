using Sirenix.OdinInspector;

namespace Phoder1.Core
{
    public static class OdiumAttributes
    {
        public const float LABEL_WIDTH = 80f;
        public class LabelWidthAttribute : Sirenix.OdinInspector.LabelWidthAttribute
        {
            public LabelWidthAttribute() : base(LABEL_WIDTH)
            {
            }
        }
    }
}
