using UnityEngine;

namespace UniKit.Debugging
{
    public class EventDebugLog : MonoBehaviour
    {
        public void DebugLog(string message) => Debug.Log(message);
    }
}
