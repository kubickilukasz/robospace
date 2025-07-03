using UI;
using UnityEngine;
using UnityEngine.UI;

public class HackingSystem : MachineSystem{

    [SerializeField] PanelController panelController;
    [SerializeField] Image fillProgress;

    [Space]

    [SerializeField] Panel startPanel;
    [SerializeField] Panel progressPanel;
    [SerializeField] Panel endPanel;

    HackingMachine hackingMachine;

    protected override void OnInit(RenderedUIOutput data) {
        base.OnInit(data);
        panelController.Show();
        hackingMachine = currentOutput as HackingMachine;
    }

    public void ShowStartPanel() {
        startPanel.Show();
    }

    public void ShowProgressPanel() {
        UpdateProgress(0f);
        progressPanel.Show();
    }

    public void ShowEndPanel() {
        endPanel.Show();
    }

    public void UpdateProgress(float t) {
        fillProgress.fillAmount = t;
    }

}
