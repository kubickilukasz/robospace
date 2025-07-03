using System;
using UnityEngine;

public abstract class MachineBase<T> : RenderedUIOutput, IActionBool where T : MachineSystem {

    public event Action<bool> onActionBool;

    [SerializeField] protected CoreGenerator generator;
    protected T machineSystem;

    protected override void OnInit(object data) {
        base.OnInit(data);
        RefreshStatus();
        if (generator) {
            generator.onChangeStatus += RefreshStatus;
        }
    }

    protected override void OnDispose() {
        base.OnDispose();
        Hide();
        if (generator) {
            generator.onChangeStatus -= RefreshStatus;
        }
    }

    protected virtual void RefreshStatus() {
        if ((generator == null || generator.IsCoreActive) && !IsShown) {
            Show();
        } else if (IsShown) {
            Hide();
        }
    }

    protected override void OnShow() {
        base.OnShow();
        machineSystem = Reference as T;
        onActionBool?.Invoke(true);
    }

    protected override void OnHide() {
        base.OnHide();
        machineSystem = null;
        onActionBool?.Invoke(false);
    }

}
