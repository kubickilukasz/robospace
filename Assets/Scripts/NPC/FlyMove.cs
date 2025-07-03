using UnityEngine;
using Kubeec.NPC;
using System;

public class FlyMove : NPCMove {

    [SerializeField] float acceleration;
    [SerializeField] float speedOnGround;
    [SerializeField] float speedInAir;
    [SerializeField] float brakeSpeed;
    [SerializeField] float gravityForce;

    [Space]

    [SerializeField] Rigidbody rigidbody;

    [Space]

    [SerializeField] float lengthCheckGround;
    [SerializeField] LayerMask maskCheckGround;

    bool onGround = true;
    bool isStoping = false;

    void OnDrawGizmosSelected() {
        Gizmos.DrawRay(transform.position, GetGroundDirection() * lengthCheckGround);
    }

    void FixedUpdate() {
        if (isStoping) {
            float brake = Time.fixedDeltaTime * Mathf.Min(brakeSpeed, 1f/Time.fixedDeltaTime);
            rigidbody.AddForce(-rigidbody.linearVelocity * brake, ForceMode.VelocityChange);
            rigidbody.AddTorque(-rigidbody.angularVelocity * brake, ForceMode.VelocityChange);
        }
        CheckGround();
        if (onGround) {
            Vector3 down = GetGroundDirection();
            rigidbody.AddForce(down * Time.fixedDeltaTime, ForceMode.Impulse);
        }
    }

    public override Vector3 GetDirection() {
        return rigidbody.linearVelocity;
    }

    public override float GetCurrentSpeed() {
        return base.GetCurrentSpeed();
    }

    public override bool IsOnGround() => onGround;

    protected override void OnInit(NonPlayerCharacter data) {
        base.OnInit(data);
        isStoping = false;
        rigidbody.isKinematic = false;
    }

    protected override void OnMove(Vector3 direction) {
        isStoping = false;
        direction.Normalize();
        if (onGround) {
            rigidbody.TryRotateTo(Quaternion.LookRotation(direction), Time.fixedDeltaTime, 0.5f);
        }
        Vector3 force = direction;// Vector3.ClampMagnitude(direction, 1f);
        if (onGround) {
            force *= speedOnGround * (1f - Vector3.Angle(direction, transform.forward) / 180f);
        } else {
            force *= speedInAir;
        }
        float acc = Mathf.Min(acceleration, 1f / Time.fixedDeltaTime);
        rigidbody.AddForce((force - rigidbody.linearVelocity) * Time.fixedDeltaTime * acc, ForceMode.VelocityChange);
    }

    protected override void OnMoveTo(Vector3 target) {
        OnMove(target - rigidbody.position);
    }

    protected override void OnRotateLook(Vector3 position) {
        if (!onGround) {
            rigidbody.TryRotateTo(Quaternion.LookRotation(position - rigidbody.position, Vector3.up), Time.fixedDeltaTime, 0.3f);
        }
    }

    protected override void OnStop() {
        isStoping = true;
    }

    void CheckGround() {
        Vector3 down = GetGroundDirection();
        if (Physics.Raycast(transform.position, down, lengthCheckGround, maskCheckGround)) {
            onGround = true;
        } else {
            onGround = false;
        }
    }

    Vector3 GetGroundDirection() {
        return -transform.up;
    }

}
