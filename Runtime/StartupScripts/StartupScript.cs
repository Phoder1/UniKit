using UnityEngine;

public abstract class StartupScript : ScriptableObject
{
    public abstract RuntimeInitializeLoadType LoadType { get; }
    public abstract void Run();
}
