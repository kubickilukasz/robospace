using Kubeec.VR.Interactions;
using System;
using UnityEngine;

[RequireComponent(typeof(InteractionTap))]
public class KeyboardTap : EnableDisableInitableDisposable, IActionBool {

    public event Action<bool> onActionBool;

    [SerializeField] string nameTapValue = "clicks";
    [SerializeField] string nameSpeedValue = "Speed";
    [SerializeField] Animator animator;

    InteractionTap interactionTap;
    public InteractionTap InteractionTap => interactionTap;

    int idTapValue;
    int idSpeedValue;
    bool wasTap;

    protected override void OnInit(object data) {
        base.OnInit(data);
        wasTap = false;
        idTapValue = Animator.StringToHash(nameTapValue);
        idSpeedValue = Animator.StringToHash(nameSpeedValue);
        interactionTap = GetComponent<InteractionTap>();
    }

    void Update() {
        if (IsInitialized() && wasTap != interactionTap.IsTap) {
            wasTap = interactionTap.IsTap;
            animator.SetBool(idTapValue, interactionTap.IsTap);
            animator.SetFloat(idSpeedValue, 2f - interactionTap.NormalizedTimeTap);
            onActionBool?.Invoke(wasTap);
        }
    }

}
