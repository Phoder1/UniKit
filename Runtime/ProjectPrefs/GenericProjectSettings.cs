
namespace UniKit
{
    public abstract class GenericProjectSettings<TChild> : SingletonScriptableObject<TChild>
        where TChild : GenericProjectSettings<TChild>
    {
    }
}
