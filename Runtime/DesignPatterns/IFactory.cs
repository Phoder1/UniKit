using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phoder1
{
    public interface IFactory<T>
    {
        T Create();
    }
}
