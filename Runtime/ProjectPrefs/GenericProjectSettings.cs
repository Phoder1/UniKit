
using UnityEngine;

namespace UniKit
{
    public abstract class GenericProjectSettings<TChild> : SingletonScriptableObject<TChild>
        where TChild : GenericProjectSettings<TChild>
    {
        public virtual string MenuPath
            => $"{Application.productName.ToDisplayName()}/{typeof(TChild).Name.ToDisplayName()}";
    }
}
