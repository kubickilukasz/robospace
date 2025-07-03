using Kubeec.Network;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletOutput : GunOutput{

    [SerializeField] Bullet bulletPrefab;
    [SerializeField] float bulletSpeed = 10f;
    [SerializeField] float maxDistance = 100f;
    [SerializeField] float damageMultiplier = 1;

    List<Bullet> bullets = new List<Bullet>();

    protected override void OnDispose() {
        base.OnDispose();
        foreach (Bullet bullet in bullets) {
            DontIgnoreCollision(bullet);
        }
    }

    protected override bool TryShot(Vector3 start, Vector3 direction) {
        float maxDistance = this.maxDistance;
        NetworkObject obj = NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab.gameObject, start, startShootTransform.rotation);
        if (obj.TryGetComponent(out Bullet bullet)) {
            for (int i = 0; i < colliders.Length; i++) {
                Physics.IgnoreCollision(colliders[i], bullet.Collider, true);
            }
            bullet.onDestruct += () => DontIgnoreCollision(bullet);
            float timeToDie = maxDistance / (bulletSpeed);
            bullet.Init(new BulletData() {
                timeToDie = timeToDie,
                prefab = bulletPrefab.gameObject,
                owner = owner,
                damageMultiplier = damageMultiplier
            });
            obj.Spawn();
            bullet.Rigidbody.AddForce(direction * bulletSpeed, ForceMode.VelocityChange);
            bullets.Add(bullet);
            return true;
        }
        return false;
    }

    void DontIgnoreCollision(Bullet bullet) {
        SetIgnoreCollision(bullet.Collider, false);
        if (bullets.Contains(bullet)) {
            bullets.Remove(bullet);
        }
    }
}
