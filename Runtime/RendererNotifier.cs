using System;
using UnityEngine;
#if UNITY_EDITOR

#endif
namespace Phoder1.Core
{
    public class RendererNotifier : MonoBehaviour
    {
        public event Action<bool> OnBecomeVisable;
        private void OnBecameInvisible()
        => OnBecomeVisable?.Invoke(false);

        private void OnBecameVisible()
       => OnBecomeVisable?.Invoke(true);
    }
}
