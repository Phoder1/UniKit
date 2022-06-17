using Sirenix.OdinInspector;
using UnityEngine;

namespace Phoder1.Debugging
{
    [HideMonoScript]
    public class EventDebugLog : MonoBehaviour
    {
        public void DebugLog(string message) => Debug.Log(message);
    }
}
