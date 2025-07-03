using Kubeec.NPC;
using Kubeec.NPC.LazyNavigation;
using System.Collections.Generic;
using UnityEngine;

public class Chasing : NPCBehaviourState {

    SearchPlayer searchPlayer;
    PlayerReference player;
    Vector3 lastPosition;
    Zone targetZone;
    Timer timer;
    Timer timerRandomRotate;
    float chaseTime;
    float distanceToAttack;
    float timeToRandomRotateMin = 1.5f;
    float timeToRandomRotateMax = 2f;
    EffectBase spottedEffectPrefab;
    EffectBase spottedEffect;

    public Chasing(NPCBehaviour behaviour, PlayerReference player, Vector3? pos = null) : base(behaviour) {
        searchPlayer = behaviour.GetComponentInChildren<SearchPlayer>(true);
        if (player) {
            this.player = player;
            lastPosition = player.Position(position);
        } else {
            this.player = null;
            lastPosition = pos ?? position;
        }
        targetZone = zoneController.FindZone(lastPosition);
        if (behaviour is FlyBehaviour flyBehaviour) {
            chaseTime = flyBehaviour.waitEndChasing;
            distanceToAttack = flyBehaviour.distanceToAttack;
            spottedEffectPrefab = flyBehaviour.spottedEffect;
            spottedEffect = spottedEffectPrefab.CreateAndPlay(flyBehaviour.transform.position, lastPosition - flyBehaviour.transform.position);
            if (spottedEffect.lazyFollower) {
                spottedEffect.lazyFollower.target = behaviour.transform;
            }
        } else {
            chaseTime = 5f;
            distanceToAttack = 5f;
        }
    }

    public override void Dispose() {
        //if (spottedEffect != null) {
            //spottedEffect.Dispose();
        //}
        //spottedEffect = null;
        CancelRotate();
        CancelTimer();
    }

    public override void Update() {
    }

    public override void FixedUpdate() {
        if (player != null && searchPlayer.CanSee(player) && player.Distance(position) < distanceToAttack) {
            CancelRotate();
            CancelTimer();
            behaviour.ChangeState(new Attack(behaviour, player));
        } else {
            if (player == null || !searchPlayer.CanSee(player)) {
                List<PlayerReference> players = searchPlayer.Search(searchPlayer.transform.forward);
                if (players.Count > 0) {
                    CancelTimer();
                    player = players[Random.Range(0, players.Count)];
                    lastPosition = player.Position(position);
                    targetZone = zoneController.FindZone(lastPosition);
                }
            }
            CreateTimer();
            if (targetZone == null) {
                player = null;
                return;
            }
            if (targetZone == currentZone) {
                if (behaviour.Character.Move.IsClose(lastPosition)) {
                    behaviour.Character.Move.Stop();
                    RandomRotate();
                } else {
                    CancelRotate();
                    behaviour.Character.Move.MoveTo(lastPosition);
                    behaviour.Character.Move.RotateLook(lastPosition);
                }
            } else {
                CancelRotate();
                Vector3? pos = GetCurrentPositionToZone(targetZone);
                if (pos.HasValue) {
                    behaviour.Character.Move.MoveTo(pos.Value);
                    behaviour.Character.Move.RotateLook(pos.Value);
                } else {
                    EndChase();
                }
            }
        }
    }

    void RandomRotate() {
        if (timerRandomRotate == null) {
            timerRandomRotate = Timer.Start(Random.Range(timeToRandomRotateMin, timeToRandomRotateMax), _RandomRotate);
        }

        void _RandomRotate(){
            if (behaviour != null) {
                behaviour.Character.Move.RotateLook(
                     Quaternion.Euler(0, Random.Range(30f, 60f), 0) * behaviour.transform.forward
                    );
            }
        }
    }

    void CancelRotate() {
        if (timerRandomRotate != null) {
            timerRandomRotate.Stop();
            timerRandomRotate = null;
        }
    }

    void CreateTimer() {
        if (timer == null) {
            timer = Timer.Start(chaseTime, EndChase);
        }
    }

    void CancelTimer() {
        if (timer != null) {
            timer.Stop();
            timer = null;
        }
    }

    void EndChase() {
        if (behaviour != null) {
            behaviour.ChangeState(new Idle(behaviour));
        }
    }

}
