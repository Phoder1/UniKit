using DG.Tweening;
using UnityEngine;

namespace Phoder1.Core
{

    public static class CustomYieldExt
    {
        public static TweenCompleteYield AsYield(this Tween tween) => new TweenCompleteYield(tween);
    }
    public class TweenCompleteYield : CustomYieldInstruction
    {
        private bool _keepWaiting = true;
        public override bool keepWaiting => _keepWaiting;
        public TweenCompleteYield(Tween tween)
        {
            tween.onComplete += Complete;

            void Complete()
            {
                tween.onComplete -= Complete;
                _keepWaiting = false;
            }
        }
    }
}
