using Kubeec.VR.Interactions;
using System;
using UnityEngine;

public class MagazineReloadGun : ReloadGun{

    public event Action<bool> OnCanShoot;

    [SerializeField] protected InteractionSocket socket;
    [SerializeField] protected Hologram textAmmo;
    [SerializeField] protected bool getMagazineOnlyDuringHold = true;

    protected Magazine magazine;
    protected HologramData hologramData;

    public override bool CanShoot() {
        bool canShoot = socket.IsItemInSocket && magazine != null && magazine.CurrentAmmo > 0;
        OnCanShoot?.Invoke(canShoot);
        return canShoot;
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        gun.Item.onActive += OnPickUp;
        gun.Item.onInactive += OnDrop;
        socket.onItemAttracted += OnItemAttracted;
        socket.onItemDetracted += OnItemDetracted;
        gun.onAction += OnShot;
        hologramData = new HologramData() {
            baseValue = magazine ? magazine.CurrentAmmoString : "0"
        };
        textAmmo?.Dispose();
        RefreshCanInteractMagazineInSocket();
    }

    protected override void OnDispose() {
        base.OnDispose();
        if (socket) {
            socket.onItemAttracted -= OnItemAttracted;
            socket.onItemAttracted -= OnItemDetracted;
        }
        if (gun) {
            gun.Item.onActive -= OnPickUp;
            gun.Item.onInactive -= OnDrop;
            gun.onAction -= OnShot;
        }
    }

    protected virtual void OnItemAttracted(InteractionItem item) {
        if (item.TryGetComponent(out Magazine magazine)) {
            this.magazine = magazine;
            hologramData.baseValue = magazine.CurrentAmmoString;
            textAmmo?.Init(hologramData);
            RefreshCanInteractMagazineInSocket();
        }
    }

    protected virtual void OnItemDetracted(InteractionItem item) {
        magazine = null;
        textAmmo?.Dispose();
        RefreshCanInteractMagazineInSocket();
    }

    protected virtual void OnShot() {
        if (socket.IsItemInSocket && magazine != null) {
            magazine.Decrease();
            hologramData.baseValue = magazine.CurrentAmmoString;
            textAmmo?.UpdateState(hologramData);
        }
    }

    protected virtual void OnPickUp() {
        RefreshCanInteractMagazineInSocket();
    }

    protected virtual void OnDrop() {
        RefreshCanInteractMagazineInSocket();
    }

    void RefreshCanInteractMagazineInSocket() {
        if (socket.CurrentItemInSocket != null) {
            if (getMagazineOnlyDuringHold) {
                socket.CurrentItemInSocket.CanInteract = gun.Item.status == InteractionStatus.Active;
            } else {
                socket.CurrentItemInSocket.CanInteract = true;
            }
        }
    }

}
