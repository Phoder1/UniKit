#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Phoder1.Core
{
    //Todo: Make a classic editor script! class is too dependent on Odin inspector!
    public enum SceneLoadMode { NextInBuild = 0, ByName = 1, ByIndex = 2 }
    public enum SceneUnloadMode { LoadFinished = 0, Delay = 1, IReadyable = 2, Manually = 3 }
    public class SceneLoader : MonoBehaviour
    {
#if ODIN_INSPECTOR
        [BoxGroup("Load")]
        [EnumToggleButtons]
#endif
        [SerializeField]
        private LoadSceneMode _loadSceneMode;

#if ODIN_INSPECTOR
        [BoxGroup("Load")]
        [EnumToggleButtons]
#endif
        [SerializeField]
        private SceneLoadMode _loadMode;

#if ODIN_INSPECTOR
        [BoxGroup("Load")]
#endif
        [Tooltip("Whether the sceneloader should be able to load multiple scenes at the same time.")]
        [SerializeField]
        private bool _lockAtLoad = true;

#if ODIN_INSPECTOR
        [BoxGroup("Load")]
#endif
        [SerializeField]
        private bool _dontDestroyOnLoad = false;

#if ODIN_INSPECTOR
        [BoxGroup("Load")]
#endif
        [SerializeField]
        private int _loadDelay;

#if ODIN_INSPECTOR
        [BoxGroup("Load")]
        [ShowIf("@_loadMode == SceneLoadMode.ByName"), ValueDropdown("ScenesNames"), InspectorName("Scene"), ValidateInput("@_sceneName != string.Empty")]
#endif
        [SerializeField]
        private string _sceneName;

#if ODIN_INSPECTOR
        [BoxGroup("Load")]
        [ShowIf("@_loadMode == SceneLoadMode.ByIndex"), ValueDropdown("ScenesIndexes"), InspectorName("Scene"), ValidateInput("@_sceneIndex >= 0")]
#endif
        [SerializeField]
        private int _sceneIndex;

#if ODIN_INSPECTOR
        [BoxGroup("Unload", VisibleIf = "@_loadSceneMode == LoadSceneMode.Additive")]
        [EnumToggleButtons]
#endif
        [SerializeField]
        private SceneUnloadMode _sceneUnloadMode;

#if ODIN_INSPECTOR
        [BoxGroup("Unload")]
        [ShowIf("@_sceneUnloadMode == SceneUnloadMode.Delay"), SuffixLabel("S")]
#endif

        [SerializeField, Min(0)]
        private float _preUnloadDelay;


#if ODIN_INSPECTOR
        [BoxGroup("Unload")]
        [ShowIf("@_sceneUnloadMode == SceneUnloadMode.Delay"), SuffixLabel("S")]
#endif
        [SerializeField, Min(0)]
        private float _postUnloadDelay;

#if ODIN_INSPECTOR
        [BoxGroup("Unload")]
        [ShowIf("@_sceneUnloadMode == SceneUnloadMode.IReadyable"), ValidateInput("@_readyable is IReadyable")]
#endif
        [SerializeField]
        private Component _readyable;

#if ODIN_INSPECTOR
        [BoxGroup("Unload")]
        [ShowIf("_lockAtLoad")]
#endif
        [Tooltip("Recocking means to enable ")]
        [SerializeField]
        private bool _unlockAfterUnload = true;
#region Events
        [SerializeField, EventsGroup]
        private UnityEvent OnStartLoadingScene;
        [SerializeField, EventsGroup]
        private UnityEvent OnFinishedLoadingScene;
        [SerializeField, EventsGroup]
        private UnityEvent OnStartUnloadingScene;
        [SerializeField, EventsGroup]
        private UnityEvent OnFinishedUnloadingScene;
#endregion

        private bool _locked = false;
#region SceneLoad
        public void LoadScene() => LoadScene(null);
        public void LoadScene(Action callback) => LoadScene(_loadSceneMode, callback);
        public void LoadScene(LoadSceneMode loadSceneMode, Action callback = null)
        {
            switch (_loadMode)
            {
                case SceneLoadMode.NextInBuild:
                    LoadNextScene(loadSceneMode, callback);
                    break;
                case SceneLoadMode.ByName:
                    LoadSceneByName(loadSceneMode, callback);
                    break;
                case SceneLoadMode.ByIndex:
                    LoadSceneByIndex(loadSceneMode, callback);
                    break;
            }
        }
        private void LoadSceneByIndex() => LoadSceneByIndex(null);
        private void LoadSceneByIndex(Action callback) => LoadSceneByIndex(_loadSceneMode, callback);
        private void LoadSceneByIndex(LoadSceneMode loadSceneMode, Action callback = null) => LoadSceneByIndex(_sceneIndex, loadSceneMode, callback);
        public void LoadSceneByIndex(int buildIndex, LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (_locked)
                return;
            var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            MainThreadDispatcher.StartCoroutine(LoadSceneRoutine(scene.name, loadSceneMode, callback));
        }
        private void LoadSceneByName() => LoadSceneByName(null);
        private void LoadSceneByName(Action callback) => LoadSceneByName(_loadSceneMode, callback);
        private void LoadSceneByName(LoadSceneMode loadSceneMode, Action callback = null) => LoadSceneByName(_sceneName, loadSceneMode, callback);
        public void LoadSceneByName(string sceneName, LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (_locked)
                return;

            MainThreadDispatcher.StartCoroutine(LoadSceneRoutine(sceneName, loadSceneMode, callback));
        }
        private void LoadNextScene() => LoadNextScene(null);
        private void LoadNextScene(Action callback) => LoadNextScene(_loadSceneMode, callback);
        public void LoadNextScene(LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (_locked)
                return;
            var loadScene = LoadSceneRoutine(SceneManager.GetActiveScene().buildIndex + 1, loadSceneMode, callback);
            MainThreadDispatcher.StartCoroutine(loadScene);
        }


        public IEnumerator LoadSceneRoutine(Action callback = null) => LoadSceneRoutine(_loadSceneMode, callback);
        public IEnumerator LoadSceneRoutine(LoadSceneMode loadSceneMode, Action callback = null)
        {
            switch (_loadMode)
            {
                case SceneLoadMode.NextInBuild:
                    yield return LoadNextSceneRoutine(loadSceneMode, callback);
                    break;
                case SceneLoadMode.ByName:
                    yield return LoadSceneByNameRoutine(loadSceneMode, callback);
                    break;
                case SceneLoadMode.ByIndex:
                    yield return LoadSceneByIndexRoutine(loadSceneMode, callback);
                    break;
            }
        }
        private IEnumerator LoadSceneByIndexRoutine(Action callback = null) => LoadSceneByIndexRoutine(_loadSceneMode, callback);
        private IEnumerator LoadSceneByIndexRoutine(LoadSceneMode loadSceneMode, Action callback = null) => LoadSceneByIndexRoutine(_sceneIndex, loadSceneMode, callback);
        public IEnumerator LoadSceneByIndexRoutine(int buildIndex, LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (_locked)
                yield break;
            var scene = SceneManager.GetSceneByBuildIndex(buildIndex);
            yield return LoadSceneRoutine(scene.name, loadSceneMode, callback);
        }
        private IEnumerator LoadSceneByNameRoutine(Action callback = null) => LoadSceneByNameRoutine(_loadSceneMode, callback);
        private IEnumerator LoadSceneByNameRoutine(LoadSceneMode loadSceneMode, Action callback = null) => LoadSceneByNameRoutine(_sceneName, loadSceneMode, callback);
        public IEnumerator LoadSceneByNameRoutine(string sceneName, LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (_locked)
                yield break;

            yield return LoadSceneRoutine(sceneName, loadSceneMode, callback);
        }
        private IEnumerator LoadNextSceneRoutine(Action callback = null) => LoadNextSceneRoutine(_loadSceneMode, callback);
        public IEnumerator LoadNextSceneRoutine(LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (_locked)
                yield break;

            yield return LoadSceneRoutine(SceneManager.GetActiveScene().buildIndex + 1, loadSceneMode, callback);
        }
        public IEnumerator LoadSceneRoutine(string sceneName, LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (!_locked)
            {
                yield return null;

                if (_dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);

                OnStartLoadingScene?.Invoke();

                if (_loadDelay != 0)
                    yield return new WaitForSecondsRealtime(_loadDelay);

                if (_lockAtLoad)
                    _locked = true;

                yield return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);

                OnFinishedLoadingScene?.Invoke();
                callback?.Invoke();

                if (loadSceneMode == LoadSceneMode.Additive && _sceneUnloadMode != SceneUnloadMode.Manually)
                    UnloadScene();
            }
        }
        public IEnumerator LoadSceneRoutine(int sceneIndex, LoadSceneMode loadSceneMode, Action callback = null)
        {
            if (!_locked)
            {
                yield return null;

                if (_dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);

                OnStartLoadingScene?.Invoke();

                if (_loadDelay != 0)
                    yield return new WaitForSecondsRealtime(_loadDelay);


                if (_lockAtLoad)
                    _locked = true;

                yield return SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);

                OnFinishedLoadingScene?.Invoke();
                callback?.Invoke();

                if (loadSceneMode == LoadSceneMode.Additive && _sceneUnloadMode != SceneUnloadMode.Manually)
                    UnloadScene();
            }
        }
#endregion
#region Unload

        private void UnloadScene()
        {
            switch (_sceneUnloadMode)
            {
                case SceneUnloadMode.LoadFinished:
                    UnloadActiveScene();
                    break;
                case SceneUnloadMode.Delay:
                    if (_preUnloadDelay <= 0)
                        UnloadActiveScene();
                    else
                        Invoke(nameof(UnloadActiveScene), _preUnloadDelay);
                    break;
                case SceneUnloadMode.IReadyable:
                    if (_readyable != null && _readyable is IReadyable readyable)
                        if (readyable.Ready)
                            UnloadActiveScene();
                        else
                            readyable.Subscribe(OnReady);
                    else
                        UnloadActiveScene();
                    break;
            }

            void OnReady(bool ready)
            {
                if (ready)
                    UnloadActiveScene();
            }
        }
        private void UnloadActiveScene()
        {
            StartCoroutine(UnloadActiveSceneRoutine());
        }
        private IEnumerator UnloadActiveSceneRoutine()
        {

            yield return null;

            OnStartUnloadingScene?.Invoke();

            if (_preUnloadDelay != 0)
                yield return new WaitForSecondsRealtime(_preUnloadDelay);

            var _activeScene = SceneManager.GetActiveScene();

            yield return SceneManager.UnloadSceneAsync(_activeScene);


            if (_postUnloadDelay != 0)
                yield return new WaitForSecondsRealtime(_postUnloadDelay);

            if (_unlockAfterUnload)
                _locked = false;

            OnFinishedUnloadingScene?.Invoke();
        }
#endregion
#if UNITY_EDITOR

        private string[] ScenesNames
        {
            get
            {
                var scenes = UnityEditor.EditorBuildSettings.scenes;
                string[] sceneNames = new string[scenes.Length];

                for (int i = 0; i < scenes.Length; i++)
                    sceneNames[i] = GetFileName(scenes[i].path);

                return sceneNames;
            }
        }
        private int[] ScenesIndexes
        {
            get
            {
                int[] sceneIndexes = new int[UnityEditor.EditorBuildSettings.scenes.Length];

                for (int i = 0; i < sceneIndexes.Length; i++)
                    sceneIndexes[i] = i;

                return sceneIndexes;
            }
        }
        private string GetFileName(string path)
        {
            var splitPath = path.Split('/');
            splitPath = splitPath[splitPath.Length - 1].Split('.');
            return splitPath[splitPath.Length - 2];
        }
        public void UnlockLoader() => _locked = false;
#endif
    }
    public interface IClickable : IEventSystemHandler
    {

    }
}

