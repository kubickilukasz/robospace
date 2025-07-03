using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Kubeec.VR.Interactions;
using Kubeec.Hittable;
using Unity.Netcode;

public class ItemCriticalForceHandler : NetworkBehaviour{

    [SerializeField] InteractionItem item;
    [SerializeField] HitCollector hitCollcetor;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            item.onCriticalForce += OnCriticalForce;
        }
    }

    public override void OnNetworkDespawn() {
        if (IsServer) {
            item.onCriticalForce -= OnCriticalForce;
        }
    }

    [Button]
    void Test() {
        OnCriticalForce(item);
    }

    void OnCriticalForce(InteractionItem item) {
        hitCollcetor.Dispose();
    }

}
