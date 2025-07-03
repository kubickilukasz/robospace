using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Kubeec.VR.Interactions;
using UnityEngine;
using UnityEngine.Localization;
using System;

public class CoreGenerator : NetworkBehaviour, IActionBool {

    public event Action onChangeStatus;
    public event Action<bool> onActionBool;

    [SerializeField] InteractionCoreLever coreLever;
    [SerializeField] InteractionSocket socket;
    [SerializeField] ScreenHUD screen;

    [Space]

    [SerializeField] LocalizedString coreInSocketText;
    [SerializeField] LocalizedString errorText;
    [SerializeField] LocalizedString waitingText;

    public bool IsCoreActive => socket.CurrentItemInSocket != null && !socket.CanInteract;

    public override void OnNetworkSpawn() {
        base.OnNetworkSpawn();
        OnDeactive();
        coreLever.onCoreLeverActive += OnActive;
        coreLever.onCoreLeverDeactive += OnDeactive;
    }

    public override void OnNetworkDespawn() {
        base.OnNetworkDespawn();
        if (coreLever) {
            coreLever.onCoreLeverActive += OnActive;
            coreLever.onCoreLeverDeactive += OnDeactive;
        }
    }

    void OnActive() {
        if (socket.CurrentItemInSocket != null) {
            screen?.SetText(coreInSocketText.GetLocalizedString());
        } else {
            screen?.SetText(errorText.GetLocalizedString());
        }
        onActionBool?.Invoke(IsCoreActive);
        onChangeStatus?.Invoke();
    }

    void OnDeactive() {
        screen?.SetText(waitingText.GetLocalizedString());
        onActionBool?.Invoke(false);
        onChangeStatus?.Invoke();
    }

}
