using UnityEngine;
using Kubeec.NPC;
using Kubeec.Hittable;

public class FlyBehaviour : NPCBehaviour {

    public float waitTime;
    public float waitEndChasing = 5f;
    public float waitToLoseTrackDuringAttack = 3f;
    public float distanceToAttack = 5f;

    public EffectBase spottedEffect;

    [SerializeField] HitCollector collector;

    public override NPCBehaviourState GetDefault() {
        return new Chasing(this, null);
    }

    public void ChangeToChasing(Vector3? position) {
        if (IsState<Idle>() && IsInitialized()) {
            ChangeState(new Chasing(this, null, position));
        }
    }

    protected override void OnInit(NonPlayerCharacter data) {
        base.OnInit(data);
        collector.onHit += OnHit;
    }

    protected override void OnDispose() {
        base.OnDispose();
        if (collector) {
            collector.onHit -= OnHit;
        }
    }

    void OnHit(HitInfo hitInfo) {
        ChangeToChasing(hitInfo.hitProvider.Owner.transform.position);
    }

}
