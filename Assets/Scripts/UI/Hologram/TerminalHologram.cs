using Kubeec.VR.Interactions;
using UI;
using UnityEngine;

public class TerminalHologram : Hologram{

    [SerializeField] InteractionCanvas interactionCanvas;
    [SerializeField] Collider collider;
    [SerializeField] PanelController panelController;

    protected override void OnInit(HologramData data) {
        base.OnInit(data);
        animationController.Play(null, () => {
            interactionCanvas.enabled = collider.enabled = true;
            panelController.Show();
        });
    }

    protected override void OnDispose() {
        interactionCanvas.enabled = collider.enabled = false;
        panelController.Hide();
        base.OnDispose();
    }

}
