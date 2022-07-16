using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class BuildScenesQuickLoad : EditorWindow
{
    private const string EnabledPrefKey = "BuildScenesQuickLoad";
    static BuildScenesQuickLoad()
    {
        if (EditorPrefs.HasKey(EnabledPrefKey) && EditorPrefs.GetBool(EnabledPrefKey))
            EnableQuickLoad();
    }

    [MenuItem("UniKit/Build Scenes Quickload/Enable")]
    public static void EnableQuickLoad()
    {
        EditorPrefs.SetBool(EnabledPrefKey, true);
        SceneView.duringSceneGui += OnScene;
    }

    [MenuItem("UniKit/Build Scenes Quickload/Disable")]
    public static void DisableQuickLoad()
    {
        EditorPrefs.SetBool(EnabledPrefKey, false);
        SceneView.duringSceneGui -= OnScene;
    }

    private static void OnScene(SceneView sceneview)
    {
        Handles.BeginGUI();
        Rect rc = new Rect(0, SceneView.lastActiveSceneView.camera.ViewportToScreenPoint(new Vector3(1, 1, 0)).y - 25,
            SceneView.lastActiveSceneView.camera.ViewportToScreenPoint(new Vector3(1, 1, 0)).x, 25);

        GUILayout.BeginArea(rc);
        GUILayout.BeginHorizontal();
        foreach (var scene in EditorBuildSettings.scenes)
            DrawSceneButton(scene.path);

        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        Handles.EndGUI();
    }

    private static void DrawSceneButton(string scenePath)
    {
        var active = EditorSceneManager.GetActiveScene();
        var def = GUI.backgroundColor;

        var sp = scenePath.Split('/', '\\', '.');
        var name = sp[sp.Length - 2];

        if (active.name == name)
            GUI.backgroundColor = Color.green;

        if (GUILayout.Button(name, GUILayout.MinHeight(25), GUILayout.MaxHeight(50), GUILayout.MinWidth(name.Length * 9), GUILayout.MaxWidth(200)))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        }
        GUI.backgroundColor = def;
    }
}
