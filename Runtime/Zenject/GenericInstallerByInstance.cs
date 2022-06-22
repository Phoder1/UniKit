using UniKit.Core.QA;
using UniKit.Core.Types;
using UnityEngine;
using Zenject;

namespace UniKit.Core.Zenject
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
            if (typeof(TBind) != typeof(TInstance))
                Container.Bind<TInstance>().FromInstance(_instance);
        }

        public Result IsValid()
            => typeof(TInstance)
            .IsSerializable
            .Assert($"{typeof(TInstance).Name} is not serializable");
    }
}