using Kubeec.Network;
using Kubeec.VR.Interactions;
using UnityEngine;

public class SimpleItemSpawner : CustomNetworkPool<SimpleItemSpawner> {

    [SerializeField] Bounds bounds;

    void OnDrawGizmosSelected() {
        Gizmos.DrawWireCube(transform.position + bounds.center, bounds.size);
    }

    public void Spawn() {
        GetNetworkObject(bounds.GetRandomPosition(transform.position), Quaternion.identity);
    }


}
