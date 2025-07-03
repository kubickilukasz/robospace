using UnityEngine;

public abstract class BaconAdditionalLight: EnableDisableInitableDisposable{

    public float intensity = 1f;
    public Color color = Color.white;

    [SerializeField] bool onUpdate = false;

    public bool OnUpdate => onUpdate;

    void OnTransformParentChanged() {
        OnValidate();
    }

    void OnValidate() {
        NotifyChange();
    }

    public void NotifyChange() {
        if (IsInitialized() || !Application.isPlaying) {
            BaconLightManager.instance?.UpdateVariables();
        }
    }

}
