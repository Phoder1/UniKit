using System;
using UnityEngine;

namespace Phoder1.Core.Cooldown
{
    [Serializable]
    public class CooldownDefinition : ICooldownDefinition
    {
        [SerializeField]
        private bool _countCooldown = true;
        [SerializeField]
        private bool _startReady = false;
        [SerializeField]
        private float _cooldown = 10;

        public float Cooldown { get => _cooldown; set => _cooldown = value; }
        public bool StartReady { get => _startReady; set => _startReady = value; }
        public bool CountCooldown => _countCooldown;
    }
}
