using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Startup Scripts/Instantiate")]
public class StartupInstantiate : StartupScript
{
    [SerializeField]
    private GameObject instantiatePrefab;
    [SerializeField]
    private bool dontDestroyOnLoad;
    public override RuntimeInitializeLoadType LoadType => RuntimeInitializeLoadType.AfterSceneLoad;

    public override void Run()
    {
        var go = Instantiate(instantiatePrefab);

        if(dontDestroyOnLoad)
            DontDestroyOnLoad(go);
    }
}
