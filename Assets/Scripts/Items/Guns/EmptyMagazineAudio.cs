using UnityEngine;

[RequireComponent(typeof(MagazineReloadGun))]
public class EmptyMagazineAudio : AudioEndpoint {

    MagazineReloadGun reloadGun;

    protected override void OnInit(object data) {
        reloadGun = GetComponent<MagazineReloadGun>();
        reloadGun.OnCanShoot += OnCanShoot;
    }

    protected override void OnDispose() {
        base.OnDispose();
        if (reloadGun != null) {
            reloadGun.OnCanShoot -= OnCanShoot;
        }
    }

    void OnCanShoot(bool value) {
        if (!value) {
            Play();
        }
    }

}
