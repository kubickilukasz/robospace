using Kubeec.Hittable;
using UnityEngine;

[RequireComponent(typeof(HitCollector))]
public class DamageEffectSpawner : EnableDisableInitableDisposable{

    [SerializeField] DamageEffect damageEffect;

    HitCollector hitCollector;
    DamageEffect current;

    protected override void OnInit(object data) {
        hitCollector = GetComponent<HitCollector>();
        hitCollector.onHit += OnHit;
    }

    protected override void OnDispose() {
        if (hitCollector) {
            hitCollector.onHit -= OnHit;
        }
    }

    void OnHit(HitInfo hitInfo) {
        if (current == null || !current.IsInitialized()) {
            current = damageEffect.Create(hitInfo.position ?? transform.position) as DamageEffect;
        } else {
            current.transform.position = hitInfo.position ?? transform.position;
        }

        current.Set(hitInfo.hitProvider.Owner, current.Damage + hitInfo.damage);
        current.Play();
    }

}
