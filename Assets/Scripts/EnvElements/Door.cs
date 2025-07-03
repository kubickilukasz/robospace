using System;
using UnityEngine;
using Kubeec.VR.Player;

public class Door : EnableDisableInitableDisposable, IActionBool{

    const string animOpenedKey = "opened";

    public event Action<bool> onActionBool;

    [SerializeField] Animator animator;
    [SerializeField] OnTriggerHandler onTriggerHandler;
    [SerializeField] bool isOpened;
    [SerializeField] bool autoClose = false;
    [SerializeField] bool manualClose = false;
    [SerializeField] float timeToClose;

    int animOpenedId;
    int playerCounts = 0;
    bool isOpening = false;
    Coroutine coroutine;

    public void Open() {
        if (!isOpened) {
            animator.SetBool(animOpenedId, isOpened = true);
            if (manualClose) {
                isOpening = true;
            }
            StartAutoClose();
            onActionBool?.Invoke(true);
        }
    }

    public void NotOpening() {
        isOpening = false;
        StartAutoClose();
    }

    public void Toggle() {
        if (isOpened) {
            Close();
        } else {
            Open();
        }
    }

    public void Close() {
        if (isOpened) {
            isOpening = false;
            animator.SetBool(animOpenedKey, isOpened = false);
            onActionBool?.Invoke(false);
        }
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        animOpenedId = Animator.StringToHash(animOpenedKey);
        onTriggerHandler.onTriggerEnter += TriggerEnter;
        onTriggerHandler.onTriggerExit += TriggerExit;
        if (isOpened) {
            isOpened = false;
            Open();
        } else {
            isOpened = true;
            Close();
        }
    }

    protected override void OnDispose() {
        base.OnDispose();
        if (onTriggerHandler) {
            onTriggerHandler.onTriggerEnter -= TriggerEnter;
            onTriggerHandler.onTriggerExit -= TriggerExit;
        }
    }

    void StartAutoClose() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        if (autoClose) {
            coroutine = this.InvokeDelay(AutoClose, timeToClose);
        }
    }

    void AutoClose() {
        if (!isOpening) {
            Close();
        }
    }

    void TriggerEnter(Collider other) {
        if (other.TryGetComponent(out PlayerController _)) {
            playerCounts++;
            if (coroutine != null) {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }

    void TriggerExit(Collider other) {
        if (other.TryGetComponent(out PlayerController _)) {
            playerCounts--;
            if (playerCounts <= 0) {
                Close();
            }
        }
    }

}
