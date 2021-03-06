using System;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace UniKit.Zenject
{
    public class RandomInstaller : MonoInstaller
    {
        [SerializeField]
        private string seed;
        [SerializeField]
        private bool randomizeSeed = true;
        public override void InstallBindings()
        {
            Container.Bind<Random>().FromMethod(CreateRandom).Lazy();
        }

        private Random CreateRandom()
        {
            if (randomizeSeed)
                return new Random(HashCode.Combine(seed, DateTime.Now));
            else
                return new Random(seed.GetHashCode());
        }
    }
}
