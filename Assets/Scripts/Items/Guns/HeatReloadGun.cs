using UnityEngine;

public class HeatReloadGun : ReloadGun {

    [SerializeField] float heatPerSeconds = 0.5f;
    [SerializeField] float coolPerSeconds = 0.15f;

    float currentHeat = 0f;

    void Update() {
        if (!IsInitialized()) {
            return;
        }

        if (gun.IsShooting) {
            currentHeat += heatPerSeconds * Time.deltaTime;
        } else {
            currentHeat -= coolPerSeconds * Time.deltaTime;
        }
        currentHeat = Mathf.Clamp01(currentHeat);
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        currentHeat = 0f;
        if (gun) {
            gun.onAction += OnShot;
        }
    }

    protected override void OnDispose() {
        base.OnDispose();
        if (gun) {
            gun.onAction -= OnShot;
        }
    }

    public override bool CanShoot() {
        return base.CanShoot() && currentHeat < 1f;
    }

    protected virtual void OnShot() {
        currentHeat += heatPerSeconds * Time.deltaTime;
    }

}
