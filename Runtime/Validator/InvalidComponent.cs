using UniKit.Types;
using System;
using UnityEngine;

namespace UniKit.QA
{
    [Obsolete]
    [NullReferenceValidate]
    public class InvalidComponent : MonoBehaviour
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

        public Result IsValid()
            => false;
    }
}
