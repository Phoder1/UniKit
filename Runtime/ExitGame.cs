using UnityEngine;

namespace UniKit
{
    public class ExitGame : MonoBehaviour
    {
        bool _quitting = false;
        public void Exit()
        {
            if (!Application.isPlaying || _quitting)
                return;

            _quitting = true;
            Application.Quit();
        }
    }
}
