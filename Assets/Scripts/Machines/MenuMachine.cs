using Kubeec.VR.Interactions;
using System.Collections.Generic;
using UnityEngine;

public class MenuMachine : MachineBase<MenuSystem> {

    [SerializeField] InteractionSimpleButton leftButton;
    [SerializeField] InteractionSimpleButton rightButton;

    [SerializeField] InteractionSimpleButton acceptButton;
    [SerializeField] float timeHoldToAccept = 2f; 
    [SerializeField] List<ScenesManager.SceneType> sceneTypes = new();

    float timer = 0f;

    void Update() {
        if (IsInitialized()) {
            if (acceptButton.status == InteractionStatus.Active) {
                Accept();
            } else {
                StopAccept();
            }
        }
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        StopAccept();
        leftButton.onActive += machineSystem.GoLeft;
        rightButton.onActive += machineSystem.GoRight;
    }

    protected override void OnDispose() {
        base.OnDispose();
        if (machineSystem) {
            leftButton.onActive -= machineSystem.GoLeft;
            rightButton.onActive -= machineSystem.GoRight;
        }
    }

    public void Accept() {
        machineSystem.Continue();
        int i = machineSystem.GetCurrentIndex();
        if (i >= sceneTypes.Count || i < 0) {
            machineSystem.SetMark(0f);
            return;
        }
        timer += Time.unscaledDeltaTime;
        machineSystem.SetMark(timer / timeHoldToAccept);
        if (timer > timeHoldToAccept) {
            GameManager.instance.TryLoadScene(sceneTypes[i]);
        }
    }

    void StopAccept() {
        timer = 0f;
        machineSystem.SetMark(0f);
    }

}
