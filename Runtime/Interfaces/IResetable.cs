using System.Collections.Generic;

namespace UniKit
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
