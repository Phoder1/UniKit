using UnityEngine;

namespace Phoder1.Core
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
