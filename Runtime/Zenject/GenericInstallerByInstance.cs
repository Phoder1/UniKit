using UnityEngine;
using Zenject;
using Phoder1.Core.QA;
using Phoder1.Core.Types;

namespace Phoder1.Core.Zenject
{
    public class GenericInstallerByInstance<TBind, TInstance> : MonoInstaller, IValidateable
        where TInstance : class, TBind
        where TBind : class
    {
        [SerializeField]
        private TInstance _instance;

        public override void InstallBindings()
        {
            Container.Bind<TBind>().FromInstance(_instance);
            Container.Bind<TInstance>().FromInstance(_instance);
        }

        public Result IsValid()
            => typeof(TInstance)
            .IsSerializable
            .Assert($"{typeof(TInstance).Name} is not serializable");
    }
}