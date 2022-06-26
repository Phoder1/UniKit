using UnityEngine;
using Zenject;

public class InstallManager : MonoInstaller
{
    [SerializeField]
    private Transform[] _installersTransforms;
    public override void InstallBindings()
    {
        foreach (var t in _installersTransforms)
        {
            if (t == null)
                continue;

            var installers = t.GetComponents<IInstaller>();
            foreach (var installer in installers)
            {
                if (installer == null || (object)installer == this)
                    continue;

                Container.Inject(installer);
                installer.InstallBindings();
            }
        }
    }
}
