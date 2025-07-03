using UnityEngine;
using System;

public abstract class GunOutput : EnableDisableInitableDisposable, IAction {

    public event Action onAction;

    [SerializeField] protected Transform startShootTransform;
    [SerializeField] protected EffectBase startShootEffect;
    [SerializeField] protected Collider[] colliders;

    protected GameObject owner;

    public Transform StartShootTransform => startShootTransform ?? transform;

    public bool Shot(GameObject owner) {
        if (!IsInitialized()) {
            return false;
        }
        this.owner = owner;
        Vector3 start = startShootTransform.position;
        Vector3 dir = startShootTransform.forward;
        bool shot = TryShot(start, dir);
        if (shot) {
            onAction?.Invoke();
        }
        return shot;
    }

    protected abstract bool TryShot(Vector3 start, Vector3 direction);

    protected void SetIgnoreCollision(Collider collider, bool value) {
        for (int i = 0; i < colliders.Length; i++) {
            Physics.IgnoreCollision(colliders[i], collider, value);
        }
    }

}
