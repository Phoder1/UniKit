using Phoder1.Async;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Phoder1.Core.Cooldown
{
    public class CooldownHandler : ICooldownHandler
    {
        #region Events
        public event Action OnReady;
        public event Action OnUnready;
        public event Action<float> OnCooldownChange;
        public event Action<float> OnCooldownfillPrecentage;
        #endregion
        #region Class Data
        private bool _countCooldown = true;
        private bool _isReady;
        private bool _alive = true;
        private float _cooldownMeter;
        #endregion
        #region Properties
        public float Cooldown { get; set; }
        public bool CountCooldown { get => _countCooldown; set => _countCooldown = value; }
        public float CooldownMeter
        {
            get => _cooldownMeter;
            set
            {
                value = Mathf.Clamp(value, 0, Cooldown);
                _cooldownMeter = value;
                CheckReadyState();
            }
        }
        public float CooldownProgress => CooldownMeter / Cooldown;
        public bool CooldownReady => CooldownMeter >= Cooldown;
        public bool StartReady { get; private set; }
        public bool IsReady => CooldownReady;
        #endregion
        public CooldownHandler(ICooldownDefinition definition, MonoBehaviour coroutineContainer)
        {
            Cooldown = definition.Cooldown;
            CountCooldown = definition.CountCooldown;
            StartReady = definition.StartReady;

            if (StartReady)
                CooldownMeter = Cooldown;
            OnCooldownChange?.Invoke(CooldownMeter);
            OnCooldownfillPrecentage?.Invoke(Mathf.Clamp01(CooldownMeter / Cooldown));
            coroutineContainer.StartCoroutine(Tick());

            DoReset();
        }
        ~CooldownHandler()
        {
            _alive = false;
        }

        #region Unity events
        public IEnumerator Tick()
        {
            while (KeepUpdate())
            {
                yield return null;

                UpdateCooldown(Time.deltaTime);
            }

            bool KeepUpdate()
                => _alive;
        }
        #endregion
        #region Public Methods
        public bool CheckReadyState()
        {
            bool wasReady = _isReady;
            _isReady = IsReady;

            if (wasReady && !_isReady)
                OnUnready?.Invoke();
            else if (!wasReady && _isReady)
                OnReady?.Invoke();

            return _isReady;
        }
        public void DoReset()
        {
            if (StartReady)
                ForceReady();
            else
                Deplete();
        }
        public void UpdateCooldown(float deltaTime)
        {
            if (CountCooldown)
            {
                CooldownMeter += deltaTime;
                OnCooldownChange?.Invoke(CooldownMeter);
                OnCooldownfillPrecentage?.Invoke(Mathf.Clamp01(CooldownMeter / Cooldown));
            }
        }
        public void Deplete() { 
            CooldownMeter = 0f;
            OnCooldownChange?.Invoke(CooldownMeter); 
        }
        public void ForceReady() {
            CooldownMeter = Cooldown; 
            OnCooldownChange?.Invoke(CooldownMeter);
            OnCooldownfillPrecentage?.Invoke(Mathf.Clamp01(CooldownMeter / Cooldown));
        }

        #endregion
    }
}
