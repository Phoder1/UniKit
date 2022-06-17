using UnityEngine;

namespace Phoder1.Debugging
{
    public class EventDebugLog : MonoBehaviour
    {
        public void DebugLog(string message) => Debug.Log(message);
    }
}
