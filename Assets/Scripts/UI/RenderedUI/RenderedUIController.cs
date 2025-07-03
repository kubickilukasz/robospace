using Kubeec.VR.Player;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderedUIController : Singleton<RenderedUIController> {

    [SerializeField] UIReference[] uiReferences;
    [SerializeField] RenderedUIReference[] references;

    public int maxRenderers => uiReferences.Length;

    List<RenderedUIOutput> activeRenderedUIOutputs = new List<RenderedUIOutput>();
    HashSet<RenderedUIIndex> deactivedRenderedUIIndeces = new HashSet<RenderedUIIndex>();
    Dictionary<RenderedUIIndex, RenderedUIReference> renderedUIreferences = new();
    Camera camera;

    public RenderedUIReference Register(RenderedUIOutput output) {
        if (activeRenderedUIOutputs.Contains(output)) {
            return renderedUIreferences[output.Index];
        }
        activeRenderedUIOutputs.Add(output);
        RefreshUI();
        return renderedUIreferences[output.Index];
    }

    public void Unregister(RenderedUIOutput output) {
        if (!activeRenderedUIOutputs.Contains(output)) {
            return;
        }
        activeRenderedUIOutputs.Remove(output);
        RefreshUI();
    }

    protected override void OnInit() {
        camera = LocalPlayerReference.instance.Camera;
        foreach (RenderedUIReference reference in references) {
            renderedUIreferences.Add(reference.Index, reference);
        }
    }

    void RefreshUI() {
        //to do optimize this
        deactivedRenderedUIIndeces = renderedUIreferences.Keys.ToHashSet();
        Vector3 camDir = camera.transform.forward;
        Vector3 camPos = camera.transform.position;
        activeRenderedUIOutputs.Sort((x, y) => x.Value(camDir, camPos) > y.Value(camDir, camPos) ? 1 : -1);
        int max = Mathf.Min(activeRenderedUIOutputs.Count, maxRenderers);
        int i = 0;
        for (i = 0; i < max; i++) {
            RenderedUIIndex index = activeRenderedUIOutputs[i].Index;
            if (deactivedRenderedUIIndeces.Contains(index)) {
                RenderedUIReference reference = renderedUIreferences[index];
                activeRenderedUIOutputs[i].SetMaterial(uiReferences[i].material);
                reference.transform.SetParent(uiReferences[i].content, false);
                reference.transform.localPosition = new Vector3(0, 0, 0);
                reference.Init();
                deactivedRenderedUIIndeces.Remove(index);
            }
        }
        for (; i < activeRenderedUIOutputs.Count; i++) {
            activeRenderedUIOutputs[i].SetMaterial(null);
        }
        foreach (RenderedUIIndex index in deactivedRenderedUIIndeces) {
            RenderedUIReference reference = renderedUIreferences[index];
            reference.Dispose();
        }
    }

    [System.Serializable]
    public class UIReference {
        public Material material;
        public RectTransform content;
    }

}
