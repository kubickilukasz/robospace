using DG.Tweening.Core.Easing;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class XROriginPlayerController : PlayerControllerBase {

    [SerializeField] XROrigin xrOrigin;
    [SerializeField] XRInputModalityManager xRInputModalityManager;
    [SerializeField] InputActionReference joyLeftHand;
    [SerializeField] InputActionReference joyRightHand;
    [SerializeField] InputActionReference leftGrip;
    [SerializeField] InputActionReference rightGrip;
    [SerializeField] InputActionReference leftSelect;
    [SerializeField] InputActionReference rightSelect;
    [SerializeField] float inputSensitiveToRotate = 0.5f;
    [SerializeField] float angleToRotate = 30f;

    bool isChangingRotation = false;
    Vector3 newPosition;

    void Start() {
        Application.onBeforeRender += OnUpdateHead;
    }

    void OnDestroy() {
        Application.onBeforeRender -= OnUpdateHead;
    }

    public override Vector2 GetJoyLeftHand() {
        return joyLeftHand.action.ReadValue<Vector2>();
    }

    public override Vector2 GetJoyRightHand() {
        return joyRightHand.action.ReadValue<Vector2>();
    }

    public override Vector3 GetHeadPosition() {
        return xrOrigin.Camera.transform.position;
    }

    public override Vector3 GetPositionLeftHand() {
        return xRInputModalityManager.leftController.transform.position;
    }

    public override Vector3 GetPositionRightHand() {
        return xRInputModalityManager.rightController.transform.position;
    }

    public override float GetPressValueLeftGrip() {
        return leftGrip.action.ReadValue<float>();
    }

    public override float GetPressValueLeftSelect() {
        return leftSelect.action.ReadValue<float>();
    }

    public override float GetPressValueRightGrip() {
        return rightGrip.action.ReadValue<float>();
    }

    public override float GetPressValueRightSelect() {
        return rightSelect.action.ReadValue<float>();
    }

    public override Vector3 GetRootPosition() {
        return xrOrigin.transform.position;
    }

    public override Quaternion GetHeadRotation() {
        return xrOrigin.Camera.transform.rotation;
    }

    public override Quaternion GetRotationLeftHand() {
        return xRInputModalityManager.leftController.transform.rotation;
    }

    public override Quaternion GetRotationRightHand() {
        return xRInputModalityManager.rightController.transform.rotation;
    }

    public override void SetPosition(Vector3 newPosition) {
        xrOrigin.transform.position = newPosition;
    }

    protected override void OnUpdate() {
        Vector3 v3 = GetJoyRightHand();
        float value = v3.x;
        float absValue = Mathf.Abs(value);
        if (!isChangingRotation) {
            if (absValue >= inputSensitiveToRotate) {
                isChangingRotation = true;
                value = value > 0f ? 1f : -1f;
                xrOrigin.transform.rotation *= Quaternion.Euler(0f, value * angleToRotate, 0f);
            }
        } else if (absValue < inputSensitiveToRotate) {
            isChangingRotation = false;
        }
    }

}
