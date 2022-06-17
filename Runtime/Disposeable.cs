using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.Core
{
    public struct Disposeable : IDisposable
    {
        private Action onDispose;

        public Disposeable(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public void Dispose() => onDispose?.Invoke();
    }

}
