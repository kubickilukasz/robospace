using Kubeec.Hittable;
using Kubeec.Network;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : NetworkBehaviour, IDestructible, IInitable<BulletData> {

    public event Action onDestruct;

    [SerializeField] EffectBase effectBase;
    [SerializeField] Collider collider;
    [SerializeField] HitProvider hitProvider;
    [SerializeField] List<GameObject> manualActiveObjects = new();

    Timer timer;
    Rigidbody rigidbody;

    public float timeToDie { get; private set; } = 5f;
    public GameObject prefab { get; private set; }
    public GameObject owner => hitProvider.Owner;

    public Rigidbody Rigidbody {
        get {
            if (rigidbody == null) {
                rigidbody = GetComponent<Rigidbody>();
            }
            return rigidbody;
        }
    }

    public Collider Collider => collider;

    public void Init(BulletData data = null) {
        manualActiveObjects.ForEach(gameObject => gameObject.SetActive(false));
        if (data != null) {
            timeToDie = data.timeToDie;
            prefab = data.prefab;
            hitProvider.Owner = data.owner;
            hitProvider.damageMultiplier = data.damageMultiplier;
        }
    }

    public bool IsInitialized() {
        return false;
    }

    public override void OnNetworkSpawn() {
        manualActiveObjects.ForEach(gameObject => gameObject.SetActive(true));
        if (IsServer) {
            timer = Timer.Start(timeToDie + 0.1f, () => {
                if (this != null && isActiveAndEnabled && gameObject != null) {
                    timer = null;
                    Destruct();
                }
            });
        }
    }

    public void Destruct() {
        timer?.Stop();
        if (IsSpawned && IsServer) {
            effectBase?.CreateAndPlay(transform.position, -Rigidbody.linearVelocity.normalized);
            onDestruct?.Invoke();
            onDestruct = null;
            manualActiveObjects.ForEach(gameObject => gameObject.SetActive(false));
            if (prefab) {
                NetworkObject.Despawn(false);
                NetworkObjectPool.Singleton.ReturnNetworkObject(NetworkObject, prefab);
            } else {
                NetworkObject.Despawn(true);
            }
        }
    }

}

public class BulletData {
    public GameObject prefab;
    public GameObject owner;
    public float timeToDie;
    public float damageMultiplier = 1f;
}
