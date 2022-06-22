#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace UniKit
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

#if ODIN_INSPECTOR
        [Button]
#endif
        [ContextMenu("Set framerate")]
        private void SetFrameRate()
        {
            Application.targetFrameRate = _targetFrameRate;
        }
    }
}
