using Kubeec.VR.Interactions;
using System;
using UnityEngine;

public class TrackingMachine : MachineBase<TrackingSystem>, IAction {

    public event Action onAction;
    public event Action<bool> onChangeLocked;

    [SerializeField] InteractionJoystick joystick;
    [SerializeField] float timeVisibleTarget = 3f;
    [SerializeField] float timeToRestartTarget = 5f;
    [SerializeField] float maxDistanceToCheck = 0.05f;
    [SerializeField] float timeToLock = 10f;
    [SerializeField] float cooldownToTry = 1f;
    [SerializeField] float targetSpeed = 0.2f;

    public bool IsLocked => isLocked;

    Vector2 currentJoystickPosition;
    Vector2 currentTargetPosition;
    Vector2 currentTargetGoal;

    bool isLocked = false;
    float timer;
    float cooldownTimer;

    void Update() {
        if (IsShown) {
            if (timer > 0f) {
                timer -= Time.deltaTime;
                if (isLocked) {
                    machineSystem.SetLocked(true, Mathf.RoundToInt(timer));
                } else {
                    MoveTarget();
                }
            } else {
                if (isLocked) {
                    Unlock();
                } else {
                    SetTarget();
                }
            }
            if (cooldownTimer > 0f) {
                cooldownTimer -= Time.deltaTime;
            }
        }
    }

    protected override void OnShow() {
        base.OnShow();
        isLocked = false;
        currentJoystickPosition = Vector2.zero;
        joystick.onChangeState += OnStateChanged;
        joystick.onAction += CheckPosition;
        machineSystem.SetLocked(false);
        SetTarget();
    }

    protected override void OnHide() {
        base.OnHide();
        joystick.onChangeState -= OnStateChanged; 
        joystick.onAction -= CheckPosition;
    }

    void CheckPosition() {
        if (cooldownTimer <= 0f && !isLocked) {
            cooldownTimer = cooldownToTry;
            if (Vector2.Distance(currentJoystickPosition, currentTargetPosition) <= maxDistanceToCheck) {
                isLocked = true;
                timer = timeToLock;
                currentTargetPosition = Vector2.zero;
                machineSystem.SetLocked(true, timeToLock);
                machineSystem.ShowTarget(Vector2.zero, timeToLock, "X24.213"); //todomagic value
                machineSystem.SetCursor(Vector2.zero);
                onChangeLocked?.Invoke(isLocked);
            } else {
                machineSystem.PingCursor(cooldownToTry);
            }
        }
    }

    void OnStateChanged(Vector2 vector2) {
        if (joystick.status == InteractionStatus.Active) {
            currentJoystickPosition = vector2;
            currentJoystickPosition = Vector2.ClampMagnitude(currentJoystickPosition, 1f);
            if (!isLocked) {
                machineSystem.SetCursor(currentJoystickPosition);
            }
        }
    }

    void Unlock() {
        isLocked = false;
        machineSystem.SetLocked(false);
        machineSystem.SetCursor(currentJoystickPosition);
        SetTarget();
        onChangeLocked?.Invoke(isLocked);
    }

    void SetTarget() {
        if (machineSystem != null) {
            currentTargetGoal = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
            currentTargetGoal = Vector2.ClampMagnitude(currentTargetGoal, 0.9f);
            machineSystem.ShowTarget(currentTargetPosition, timeVisibleTarget);
            timer = timeToRestartTarget;
            onAction?.Invoke();
        }
    }

    void MoveTarget() {
        if (machineSystem != null) {
            Vector2 dir = currentTargetGoal - currentTargetPosition;
            Vector2 force = Time.deltaTime * targetSpeed * dir.normalized;
            currentTargetPosition += force;
            machineSystem.ShowTarget(currentTargetPosition);
        }
    }

}
