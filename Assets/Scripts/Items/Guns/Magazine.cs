using Kubeec.VR.Interactions;
using UnityEngine;

public class Magazine : EnableInitableDisposable{

    [SerializeField] int maxAmmo;
    [SerializeField] bool destructOnEmpty;
    [SerializeField] bool infinity = false;
    [SerializeField] InteractionItem item;

    int currentAmmo;

    public int MaxAmmo => maxAmmo;
    public int CurrentAmmo => currentAmmo;
    public string CurrentAmmoString => infinity ? "∞" : currentAmmo.ToString();

    protected override void OnInit(object data) {
        Fill();
        item.onInactive += OnInactive;
    }

    protected override void OnDispose() {
        if (item) {
            item.onInactive -= OnInactive;
        }
    }

    public bool Decrease() {
        if (!infinity) {
            currentAmmo = Mathf.Max(currentAmmo - 1, 0);
        }
        return currentAmmo > 0;
    }

    public void Fill() {
        currentAmmo = maxAmmo;
    }

    void OnInactive() {
        if (destructOnEmpty && currentAmmo <= 0 && !item.IsSnapped) {
            this.Destruct();
        }
    }

}
