using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

#endif
namespace Phoder1.Core
{
    public class RenderCulling : MonoBehaviour
    {
        [SerializeField]
        GameObject _parent;
        [SerializeField]
        RendererNotifier[] _rendererNotifiers;

        private void Awake()
        {
            for (int i = 0; i < _rendererNotifiers.Length; i++)
                _rendererNotifiers[i].OnBecomeVisable += SetActive;
        }
        private void OnDestroy()
        {
            for (int i = 0; i < _rendererNotifiers.Length; i++)
                _rendererNotifiers[i].OnBecomeVisable -= SetActive;
        }
        private void SetActive(bool state)
        {
            if (_parent.activeSelf != state)
                _parent.SetActive(state);
        }

#if UNITY_EDITOR
        private const string root = "Root";
        [MenuItem("Tools/Rendering/Remove Visable RendererScripts")]
        private static void RemoveAllRenderers()
        {
            var meshRenderers = FindObjectsOfType<RenderCulling>();
            var rns = FindObjectsOfType<RendererNotifier>();
            foreach (var mr in meshRenderers)
            {
                DestroyImmediate(mr);
            }
            foreach (var rn in rns)
                DestroyImmediate(rn);
        }
        [MenuItem("Tools/Rendering/Add RendererScripts To All Entities")]
        private static void AddRendererToMeshRenderer()
        {
            var roots = GameObject.FindGameObjectsWithTag(root);

            foreach (var root in roots)
                AsignRenderHandler(root);

            Debug.Log($@"Located {roots.Length} ""Root"" objects and added/assigned them the RenderHandling Component");
        }

        private static void AsignRenderHandler(GameObject go)
        {

            if (!go.TryGetComponent(out RenderCulling rh))
                rh = go.gameObject.AddComponent<RenderCulling>();

            rh._parent = go;

            var mrs = rh.GetComponentsInChildren<MeshRenderer>();
            if (mrs.Length == 0)
                return;
            List<RendererNotifier> lrn = new List<RendererNotifier>();
            foreach (var mr in mrs)
            {
                if (!mr.gameObject.TryGetComponent(out RendererNotifier rn))
                    rn = mr.gameObject.AddComponent<RendererNotifier>();

                lrn.Add(rn);
            }
            rh._rendererNotifiers = lrn.ToArray();
        }
#endif
    }
}
