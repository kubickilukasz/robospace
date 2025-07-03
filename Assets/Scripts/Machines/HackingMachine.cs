using Items;
using System;
using UnityEngine;

public class HackingMachine : MachineBase<HackingSystem>, IAction{

    public event Action onAction;

    [SerializeField] Button button;
    [SerializeField] KeyboardTap keyboardTap;

    [Space]

    [SerializeField] float progressOnTap = 0.1f;
    [SerializeField] float progressDeclinePerSeconds = 1.0f;

    float currentProgress = 0f;
    bool duringProgress = false;

    void Update() {
        if (duringProgress) {
            currentProgress -= Time.deltaTime * progressDeclinePerSeconds;
            RefreshProgress();
        }
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        duringProgress = false;
    }

    protected override void OnDispose() {
        base.OnDispose();
        duringProgress = false;
    }

    protected override void OnShow() {
        base.OnShow();
        machineSystem.ShowStartPanel();
        button.onAction += StartButton;
    }

    protected override void OnHide() {
        base.OnHide();
        try {
            button.onAction -= StartButton;
        } catch { };
        try {
            keyboardTap.InteractionTap.onAction -= OnTap;
        } catch { };
    }

    void StartButton() {
        button.onAction -= StartButton;
        machineSystem.ShowProgressPanel();
        currentProgress = 0f;
        keyboardTap.InteractionTap.onAction += OnTap;
        duringProgress = true;
    }

    void OnTap() {
        currentProgress += progressOnTap;
        RefreshProgress();
    }

    void RefreshProgress() {
        if (currentProgress >= 1f) {
            duringProgress = false;
            machineSystem.UpdateProgress(1f);
            keyboardTap.InteractionTap.onAction -= OnTap;
            machineSystem.ShowEndPanel();
            onAction?.Invoke();
        } else {
            currentProgress = Mathf.Max(0f, currentProgress);
            machineSystem.UpdateProgress(currentProgress);
        }
    }

}
