using UnityEngine;
using Kubeec.VR;
using Kubeec.VR.Player;

public class ShootSceneDeathPlayerHandler : SceneDeathPlayerHandler{

    [SerializeField] ShootSceneController controller;
    [SerializeField] PlayerSpace playerSpace;
    [SerializeField] Transform cameraPosition;
    [SerializeField] Transform rebornPosition;
    [SerializeField] float timeToReborn = 10f;

    void Reset() {
        controller = GetComponentInChildren<ShootSceneController>();
    }

    public override PlayerSpace GetPlayerSpace() {
        return playerSpace;
    }

    public override void HandleDeath(DeathPlayerHandler deathPlayer) {
        deathPlayer.SetHeadCamera(cameraPosition.position, cameraPosition.rotation);
        this.InvokeDelay(() => {
            deathPlayer.Reborn(rebornPosition.position, rebornPosition.rotation);
            controller.ResetWaves();
        }, timeToReborn);
    }

}
