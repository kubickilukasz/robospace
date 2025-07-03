using UnityEngine;
using Kubeec.NPC;
using Kubeec.NPC.LazyNavigation;
using System.Collections.Generic;
using System.Linq;

public class Attack : NPCBehaviourState {
    SearchPlayer searchPlayer;
    PlayerReference player;
    Vector3 lastPosition;
    Zone targetZone;
    Timer timer;
    float waitToLoseTrackDuringAttack;
    float distanceToAttack;

    FlyAttack flyAttack;
    Vector3? positionDuringCooldown;
    Vector3? positionToMove;
    Vector3? lookRotation;
    bool stop = false;
    bool canSee = false;

    public Attack(NPCBehaviour behaviour, PlayerReference player) : base(behaviour) {
        searchPlayer = behaviour.GetComponentInChildren<SearchPlayer>(true);
        flyAttack = behaviour.GetComponentInChildren<FlyAttack>(true);
        this.player = player;
        lastPosition = player.transform.position;
        targetZone = zoneController.FindZone(lastPosition);
        canSee = true;
        if (behaviour is FlyBehaviour flyBehaviour) {
            waitToLoseTrackDuringAttack = flyBehaviour.waitToLoseTrackDuringAttack;
            distanceToAttack = flyBehaviour.distanceToAttack;
        } else {
            waitToLoseTrackDuringAttack = 5f;
            distanceToAttack = 5f;
        }
    }

    public override void Dispose() {
        if (timer) {
            timer.Stop();
        }
    }

    public override void Update() {
        if (flyAttack.IsLoading || (canSee && player.Distance(position) < distanceToAttack)) {
            //do attack
            CancelTimer();
            StartAttack();
        } else {
            CreateTimer();
            bool inSameZone = targetZone == currentZone;
            Vector3 playerPos = player.Position(position);
            if (inSameZone) {
                StartAttack();
            } else {
                Vector3? pos = GetCurrentPositionToZone(targetZone);
                if (pos.HasValue) {
                    positionToMove = pos.Value;
                } else {
                    EndAttack();
                }
            }
        }
    }

    public override void FixedUpdate() {
        if (!flyAttack.IsLoading) {
            canSee = searchPlayer.CanSee(player);
        }
        if (stop) {
            behaviour.Character.Move.Stop();
        } else if (positionToMove.HasValue) {
            behaviour.Character.Move.MoveTo(positionToMove.Value);
        }
        if (lookRotation.HasValue) {
            behaviour.Character.Move.RotateLook(lookRotation.Value);
        }
    }

    void StartAttack() {
        stop = false;
        Vector3 playerPos = player.Position(position);
        lookRotation = null;
        if (flyAttack.IsCooldown) {
            //Try better position;
            lookRotation = playerPos;
            if (!positionDuringCooldown.HasValue) {
                positionDuringCooldown = currentZone.GetRandomPositionInZone();
            }
            if (behaviour.Character.Move.IsClose(positionDuringCooldown.Value)) {
                positionDuringCooldown = null;
            } else {
                positionToMove = positionDuringCooldown.Value;
            }
        } else {
            positionDuringCooldown = positionToMove = null;
            //Try shoot
            stop = true;
            if (!flyAttack.TryShot()) {
                //is loading
                if (!flyAttack.IsLateStageLoading) {
                    lookRotation = playerPos.GetRandomOffset((Vector3.right + Vector3.forward) * 0.02f);
                }
            }
        }

    }

    void CreateTimer() {
        if (timer == null) {
            timer = Timer.Start(waitToLoseTrackDuringAttack, EndAttack);
        }
    }

    void CancelTimer() {
        if (timer != null) {
            timer.Stop();
            timer = null;
        }
    }

    void EndAttack() {
        CancelTimer();
        if (behaviour != null && behaviour.IsInitialized()) {
            behaviour.ChangeState(new Chasing(behaviour, player));
        }
    }

}
