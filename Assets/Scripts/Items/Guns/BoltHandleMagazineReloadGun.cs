using Kubeec.VR.Interactions;
using System.Net.Sockets;
using UnityEngine;

public class BoltHandleMagazineReloadGun : MagazineReloadGun{

    [SerializeField] float canShootBoltHandleStateValue;
    [SerializeField] InteractionLever boltHandle;

    bool isShooting = false;

    public override bool CanShoot() {
        if (!base.CanShoot()) {
            isShooting = false;
            SetCantShoot(true);
            return false;
        }
        return boltHandle.status == InteractionStatus.Inactive 
            && Mathf.Approximately(boltHandle.GetState(), canShootBoltHandleStateValue);
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        SetCantShoot();
        isShooting = false;
        boltHandle.onChangeState += ChangeState;
    }

    protected override void OnDispose() {
        base.OnDispose();
        if (boltHandle) {
            boltHandle.onChangeState -= ChangeState;
        }
    }

    protected override void OnItemAttracted(InteractionItem item) {
        base.OnItemAttracted(item);
        SetCantShoot(true);
    }

    protected override void OnItemDetracted(InteractionItem item) {
        base.OnItemDetracted(item);
        isShooting = false;
        SetCantShoot(true);
    }

    protected override void OnShot() {
        base.OnShot();
        isShooting = true;
        SetCantShoot(false);
    }

    void SetCanShoot() {
        boltHandle.SetDefaultStates(new float[] { canShootBoltHandleStateValue });
        boltHandle.CanInteract = false;
    }

    void SetCantShoot(bool canInteract = true) {
        boltHandle.SetDefaultStates(new float[] { 1f - canShootBoltHandleStateValue });
        boltHandle.CanInteract = canInteract;
    }

    void ChangeState(float state) {
        if (state == canShootBoltHandleStateValue && magazine != null) {
            SetCanShoot();
        } else if (state == 1f - canShootBoltHandleStateValue && isShooting) {
            SetCanShoot();
        }
    }

}
