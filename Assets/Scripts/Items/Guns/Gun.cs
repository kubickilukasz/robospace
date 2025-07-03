using UnityEngine;
using System;
using Kubeec.VR.Interactions;

public class Gun : TriggerItemAddition, IAction{

    public event Action onAction;

    [SerializeField] bool fullAuto = false;
    [SerializeField] float bulletPerSeconds = 1f;

    [Space]

    [SerializeField] protected GunOutput gunOutput;
    [SerializeField] protected Recoil recoil;
    [SerializeField] protected Transform startShootTransform;
    [SerializeField] protected EffectBase startShootEffect;
    [SerializeField] protected ReloadGun reloadGun;

    protected GameObject owner;
    protected float timeFromShoot = 0f;

    bool isShoot = false;
    bool isShooting = false;

    public bool IsShooting => isShooting;

    protected override void OnInit(object data) {
        base.OnInit(data);
        timeFromShoot = bulletPerSeconds;
        isShoot = false;
        isShooting = false;
    }

    protected virtual void FixedUpdate() {
        if (isShoot) {
            Shot();
            startShootEffect?.CreateAndPlay(startShootTransform.position, startShootTransform.forward, startShootTransform);
            isShoot = false;
        }
    }

    protected override void Update() {
        base.Update();
        isShooting = false;
        foreach (HandInteractor hand in hands) {
            TryShot(hand);
        }
        if (timeFromShoot < bulletPerSeconds) {
            timeFromShoot += Time.deltaTime;
        }
    }

    public void TryShot() {
        isShooting = true;
        if (!isShoot && timeFromShoot >= bulletPerSeconds) {
            timeFromShoot = 0f;
            if (reloadGun.CanShoot()) {
                isShoot = true;
                onAction?.Invoke();
            }
        }
    }

    protected override void RefreshHands() {
        base.RefreshHands();
        if (hands.Count > 0) {
            owner = hands[0].gameObject;
        } else {
            owner = gameObject;
        }
        if (recoil) {
            recoil.SetStability(hands.Count);
        }
    }

    protected void Shot() {
        gunOutput.Shot(owner);
    }

    void TryShot(HandInteractor hand) {
        if (!CanTrigger(hand)) {
            return;
        }
        if (fullAuto) {
            if (isPressed.Invoke(hand)) {
                TryShot();
            }
        } else {
            if (isDown.Invoke(hand)) {
                TryShot();
            }
        }
    }

}
