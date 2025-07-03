using Kubeec.VR.Player;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ResetPositionButton : EnableDisableInitableDisposable{

    PlayerController controller;
    Button button;

    protected override void OnInit(object data) {
        button = GetComponent<Button>();
        controller = GetComponentInParent<PlayerController>();
        if (button && controller) {
            button.onClick.AddListener(OnClick);
        }
    }

    protected override void OnDispose() {
        if (button && controller) {
            button.onClick.RemoveListener(OnClick);
        }
    }

    void OnClick() {
        controller.ResetPosition();
    }

}
