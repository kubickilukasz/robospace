using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;

public class Hologram : InitableDisposable<HologramData> {

    [SerializeField] protected Canvas canvas;
    [SerializeField] protected AnimationController animationController;
    [SerializeField] protected TextMeshProUGUI headerText;
    [SerializeField] protected TextMeshProUGUI baseTextValue;

    protected HologramData current;

    void Start() {
        OnDispose(); //Reset
        canvas.enabled = false;
    }

    public void Show() {
        Init();
    }

    public void UpdateState(HologramData data) {
        if (IsInitialized()) {
            this.current = data;
            if (headerText) headerText.text = string.Empty;
            if (baseTextValue) baseTextValue.text = string.Empty;
            if (data != null) {
                if (data.headerText != null) {
                    headerText?.SetText(data.headerText);
                }
                if (data.baseValue != null) {
                    baseTextValue?.SetText(data.baseValue.ToString());
                }
            }
        }
    }

    protected override void OnInit(HologramData data) {
        canvas.enabled = true;
        UpdateState(data);
    }

    protected override void OnDispose() {
        animationController.PlayBackwards(onComplete: Hide);
    }

    void Hide() {
        if (!IsInitialized()) {
            canvas.enabled = false;
        }
    }

}

[System.Serializable]
public class HologramData {
    public string headerText;
    public object baseValue;
}
