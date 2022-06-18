using Zenject;

public class GenericInstallerFromNew<TBind, TImplement> : MonoInstaller
    where TImplement : class, TBind, new()
    where TBind : class
{
    public override void InstallBindings()
    {
        Container.Bind<TBind>().FromNew();
    }
}