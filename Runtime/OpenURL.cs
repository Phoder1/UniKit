using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1.Core
{
    public class OpenURL : MonoBehaviour
    {
        [SerializeField]
        private string _url;

        private bool _openedURL = false;
        public void Open()
            => Open(_url);
        public void Open(string url)
        {
            if(_openedURL)
                return;

            Application.OpenURL(url);
        }
    }
}
