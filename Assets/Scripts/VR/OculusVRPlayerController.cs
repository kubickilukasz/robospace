using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculusVRPlayerController : PlayerControllerBase {

    //[SerializeField] OVRCameraRig cameraRig;
    [SerializeField] GameObject windowsPlayerController;

    Vector3 newPosition;

    void Reset() {
        
    }


    protected override void Awake() {
        if (windowsPlayerController != null && windowsPlayerController.activeInHierarchy) {
            DestroyImmediate(gameObject);
        } else {

        }
    }
    protected override void OnFixedUpdate() {
       // cameraRig.transform.localPosition = newPosition;
    }

    protected override void OnUpdate() {
        // if (!OVRManager.OVRManagerinitialized) return;
        //cameraRig.transform.position = newPosition;
    }

    public override Vector3 GetBodyPosition() {
        Vector3 headPosition = GetHeadPosition();
        Vector3 offset = GetHeadRotation() * new Vector3(0,-0.1f,0);
        offset.y = 0f;
        return headPosition - offset;
    }

    public override Vector3 GetPositionLeftHand() {
        throw new System.Exception();
        //return cameraRig.leftHandAnchor.position;
    }

    public override Vector3 GetPositionRightHand() {
        throw new System.Exception();
    }

    public override Vector3 GetRootPosition() {
        throw new System.Exception();
    }

    public override Vector3 GetHeadPosition() {
        throw new System.Exception();
    }

    public override Quaternion GetHeadRotation() {
        throw new System.Exception();
    }

    public override Quaternion GetRotationLeftHand() {
        throw new System.Exception();
    }

    public override Quaternion GetRotationRightHand() {
        throw new System.Exception();
    }

    public override Vector2 GetJoyLeftHand() {
        throw new System.Exception();
    }

    public override Vector2 GetJoyRightHand() {
        throw new System.Exception();
    }

    public override float GetPressValueLeftSelect() {
        throw new System.Exception();
    }

    public override float GetPressValueRightSelect() {
        throw new System.Exception();
    }

    public override float GetPressValueLeftGrip() {
        throw new System.Exception();
    }

    public override float GetPressValueRightGrip() {
        throw new System.Exception();
    }

    public override void SetPosition(Vector3 newPosition) {
        throw new System.NotImplementedException();
    }
}
