using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class MenuSystem : MachineSystem{

    [SerializeField] Panel splashPanel;
    [SerializeField] Panel welcomeScreen;
    [SerializeField] Panel menuPanel;

    [SerializeField] float durationWaitInSplash = 6f;

    [SerializeField] Image mark;
    [SerializeField] ScrollList scrollList;

    protected override void OnShow() {
        base.OnShow();
        splashPanel.Show();
        this.InvokeDelay(welcomeScreen.Show, durationWaitInSplash);
        SetMark(0f);
    }

    public int GetCurrentIndex() {
        return menuPanel.IsShown && !scrollList.IsMoving ? scrollList.CurrentElement : -1;
    }

    public void Continue() {
        if (welcomeScreen.IsShown) {
            menuPanel.Show();
        }
    }

    public void SetMark(float value) {
        if (menuPanel.IsShown) {
            mark.fillAmount = value;
        } else {
            mark.fillAmount = 0f;
        }
    }

    public void GoLeft() {
        if (menuPanel.IsShown && !scrollList.IsMoving) {
            scrollList.GoLeft();
        }
    }

    public void GoRight() {
        if (menuPanel.IsShown && !scrollList.IsMoving) {
            scrollList.GoRight();
        }
    }

}
