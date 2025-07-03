using UnityEngine;

public class BaconPointLight : BaconAdditionalLight {

    public float radius = 3f;

    void OnDrawGizmosSelected() {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    protected override void OnInit(object data) {
        BaconLightManager.instance?.Add(this);
    }

    protected override void OnDispose() {
        BaconLightManager.instance?.Remove(this);
    }

}
