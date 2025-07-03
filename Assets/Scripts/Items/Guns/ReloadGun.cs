using UnityEngine;

[RequireComponent(typeof(Gun))]
public class ReloadGun : EnableDisableInitableDisposable {
    
    [SerializeField] protected Gun gun;

    void Reset() {
        gun = GetComponent<Gun>();
    }

    public virtual bool CanShoot() {
        return true;
    }

}
