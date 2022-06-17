using System;
using UnityEngine;

namespace Phoder1.Core.QA
{
    [Obsolete]
    public class InvalidComponent : MonoBehaviour, IReflectionValidateable
    {
#pragma warning disable
        [SerializeField]
        private MonoBehaviour _privateUnassigned;
        public MonoBehaviour PublicUnassigned;

        private void OnValidate()
        {
            _privateUnassigned = null;
            PublicUnassigned = null;
        }
    }
}
