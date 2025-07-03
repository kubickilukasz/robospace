using UnityEngine;
using System;

public class Platform : EnableDisableInitableDisposable, IActionBool{

    const string animUpKey = "up";

    public event Action<bool> onActionBool;

    [SerializeField] Animator animator;
    [SerializeField] bool isUp;

    int animUpId;

    public void Up() {
        if (!isUp) {
            animator.SetBool(animUpId, isUp = true);
            onActionBool?.Invoke(true);
        }
    }

    public void Toggle() {
        if (isUp) {
            Down();
        } else {
            Up();
        }
    }

    public void Down() {
        if (isUp) {
            animator.SetBool(animUpKey, isUp = false);
            onActionBool?.Invoke(false);
        }
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        animUpId = Animator.StringToHash(animUpKey);
        if (isUp) {
            isUp = false;
            Up();
        } else {
            isUp = true;
            Down();
        }
    }



}
