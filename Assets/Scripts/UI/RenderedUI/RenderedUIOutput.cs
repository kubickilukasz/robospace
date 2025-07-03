using Newtonsoft.Json.Bson;
using UnityEngine;

public class RenderedUIOutput : EnableDisableInitableDisposable{

    [SerializeField] RenderedUIIndex index;
    [SerializeField] Material defaultMaterial;
    [SerializeField] Renderer renderer;
    [SerializeField] int materialId = 0;
    [SerializeField] Transform pivot;
    [SerializeField] float maxAngle = 90f;
    [SerializeField] int priority = 0;

    RenderedUIController controller;
    Camera tempCamera;

    public int Priority => priority;
    public RenderedUIIndex Index => index;
    public RenderedUIReference Reference => uiReference;
    public bool IsShown => uiReference != null;

    RenderedUIReference uiReference;

    public void Show() {
        uiReference = controller.Register(this);
        OnShow();
    }

    public void Hide() {
        SetMaterial(null);
        controller.Unregister(this);
        uiReference = null;
        OnHide();
    }

    public void SetMaterial(Material material) {
        Material[] materials = new Material[renderer.materials.Length];
        for (int i = 0; i < materials.Length; i++) {
            materials[i] = renderer.materials[i];
        }
        materials[materialId] = material != null ? material : defaultMaterial;
        renderer.materials = materials;
    }

    public float Value(Vector3 camDirection, Vector3 camPosition) {
        Vector3 pos = pivot ? pivot.position : transform.position;
        return IsInAngle(camDirection) ? 1 : 10 * Vector3.SqrMagnitude(camPosition - pos);
    }

    public float GetAngle(Vector3 camDirection) {
        Vector3 direction = pivot ? pivot.forward : transform.forward;
        return Vector3.Angle(camDirection, direction);
    }

    public bool IsInAngle(Vector3 camDirection) {
        return IsInitialized() && GetAngle(camDirection) <= maxAngle;
    }

    protected override void OnInit(object data) {
        this.controller = RenderedUIController.instance;
        SetMaterial(defaultMaterial);
    }

    protected override void OnDispose() {
        base.OnDispose();
        Hide();
    }

    protected virtual void OnShow() {

    }

    protected virtual void OnHide() {

    }

}
