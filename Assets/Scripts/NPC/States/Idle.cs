using UnityEngine;
using Kubeec.NPC;
using System.Collections.Generic;

public class Idle : NPCBehaviourState {

    SearchPlayer searchPlayer;

    float waitTime;
    float timer;
    Vector3? target;
    Vector3 targetLookAt;

    public Idle(NPCBehaviour behaviour) : base(behaviour) {
        searchPlayer = behaviour.GetComponentInChildren<SearchPlayer>(true);
        if (behaviour is FlyBehaviour fly) {
            waitTime = fly.waitTime;
        } else {
            waitTime = 2f;
        }
        targetLookAt = position;
        timer = 0f;
    }

    public override void Update() {
        if (timer < waitTime) {
            behaviour.Character.Move.Stop();
            timer += Time.deltaTime;
        } else if (!target.HasValue) {
            bool onGround = Random.Range(0f, 1f) > 0.75 ? !behaviour.Character.Move.IsOnGround() : behaviour.Character.Move.IsOnGround();
            if (onGround) {
                target = currentZone.GetRandomPositionInZoneOnFloor();
            } else {
                target = currentZone.GetRandomPositionInZone();
            }
            targetLookAt = target.Value; // magic values
        }
    }

    public override void FixedUpdate() {
        if (target.HasValue) {
            behaviour.Character.Move.MoveTo(target.Value);
            behaviour.Character.Move.RotateLook(targetLookAt);
            if (behaviour.Character.Move.IsClose(target.Value)) {
                target = null;
                timer = 0f;
            }
        }
        if (searchPlayer != null) {
            List<PlayerReference> players = searchPlayer.Search(searchPlayer.transform.forward);
            if (players.Count > 0 && behaviour.IsInitialized()) {
                behaviour.ChangeState(new Chasing(behaviour, players[Random.Range(0, players.Count)]));
            }
        }
    }
}
