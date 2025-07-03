using UnityEngine;

public class Recoil : EnableDisableInitableDisposable{
    
    [SerializeField] Vector3 recoilForce;
    [SerializeField] float yTorqueRecoilForce;
    [SerializeField] Rigidbody rigidbody;

    [Space]

    [SerializeField] Vector3 displacementForce;
    [SerializeField] float displacementCooldown = 0.5f;
    [SerializeField] float displacementAdd = 0.1f;

    [Space]

    [SerializeField] GunOutput gunOutput;

    float displacementValue;
    int stability;
    Transform defaultStartShoot;
    Transform tempStartShoot;

    void Update() {
        SetTransformOutputDisplacement();
    }

    protected override void OnInit(object data) {
        if (gunOutput) {
            gunOutput.onAction += OnShot;
        }
        displacementValue = 0f;
        SetStability(0);
    }

    protected override void OnDispose() {
        if (gunOutput) {
            gunOutput.onAction -= OnShot;
        }
    }

    public void SetStability(int value) {
        stability = Mathf.Max(value, 1);
    }

    public void DoRecoil(Transform outputBullet) {
        if (!IsInitialized()) {
            return;
        }
        if (defaultStartShoot == null) {
            defaultStartShoot = outputBullet.Copy();
            tempStartShoot = outputBullet;
        }
        if (!rigidbody.isKinematic) {
            rigidbody.AddForceAtPosition(-tempStartShoot.forward.Multiply(recoilForce) / stability, tempStartShoot.position, ForceMode.Impulse);
            rigidbody.AddTorque(-tempStartShoot.right * yTorqueRecoilForce / stability, ForceMode.Impulse);
        }
        displacementValue += displacementAdd;
    }

    void OnShot() {
        DoRecoil(gunOutput.StartShootTransform);
    }

    void SetTransformOutputDisplacement() {
        if (!IsInitialized() || tempStartShoot == null) {
            return;
        }
        displacementValue = Mathf.Clamp(displacementValue, 0f, 1f);
        Vector3 offset = Vector3.Lerp(Vector3.zero, displacementForce / stability, displacementValue);
        tempStartShoot.localRotation = defaultStartShoot.localRotation * Quaternion.Euler(Vector3.zero.GetRandomOffset(offset));
        displacementValue -= (1f/displacementCooldown) * Time.deltaTime;
    }

}
