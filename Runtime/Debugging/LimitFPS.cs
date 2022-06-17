using Sirenix.OdinInspector;
using UnityEngine;

namespace Phoder1.Core
{
    public class LimitFPS : MonoBehaviour
    {
        [SerializeField]
        private int _targetFrameRate = 30;
        // Start is called before the first frame update
        void Start()
        {
            SetFrameRate();
        }

        [Button]
        private void SetFrameRate()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}
