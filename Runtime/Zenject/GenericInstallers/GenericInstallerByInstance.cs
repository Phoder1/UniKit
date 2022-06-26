using UniKit.Attributes;
using UniKit.QA;
using UniKit.Types;
using UnityEngine;
using Zenject;

namespace UniKit.Zenject
{
    public class GenericInstallerByInstance<TBind, TInstance> : MonoInstaller, IValidateable
        where TInstance : class, TBind
        where TBind : class
    {
        [SerializeField, Inline]
        private TInstance _instance;

        public override void InstallBindings()
        {
            Container.Bind<TBind>().FromInstance(_instance);
            if (typeof(TBind) != typeof(TInstance))
                Container.Bind<TInstance>().FromInstance(_instance);
        }

        public Result IsValid()
        {
            var type = typeof(TInstance);

            return (type.IsSerializable || typeof(Object).IsAssignableFrom(type))
                .Assert($"{typeof(TInstance).Name} is not serializable");
        }
    }
}