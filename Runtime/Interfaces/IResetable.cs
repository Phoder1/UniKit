using System.Collections.Generic;

namespace Phoder1.Core
{
    public interface IResetable
    {
        void DoReset();
    }
    public static class ResetableExt
    {
        public static void DoReset(this IEnumerable<IResetable> resetables)
        {
            foreach (var resetable in resetables)
                resetable.DoReset();
        }
    }
}
