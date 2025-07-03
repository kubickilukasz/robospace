using UnityEngine;

public class BaconSpotLight : BaconAdditionalLight {

    public float radius = 3f;
    public float angle = 3f;

    void OnDrawGizmosSelected() {
        Gizmos.color = color;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * radius);
    }

    protected override void OnInit(object data) {
        BaconLightManager.instance?.Add(this);
    }

    protected override void OnDispose() {
        BaconLightManager.instance?.Remove(this);
    }

}
