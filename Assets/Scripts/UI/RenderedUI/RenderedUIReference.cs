using System;
using UI;
using UnityEngine;
using Kubeec.General;

public class RenderedUIReference : EnableDisableRectInitableDisposable<RenderedUIOutput> { 

    [SerializeField] RenderedUIIndex index;
    [SerializeField] Canvas canvas;
    [SerializeField] AnimationController controller;

    public RenderedUIIndex Index => index;

    protected RenderedUIOutput currentOutput;

    void Awake() {
        canvas.enabled = false;
    }

    protected override void OnInit(RenderedUIOutput data) {
        currentOutput = data;
        controller?.ResetToPlayBackwards();
        canvas.enabled = true;
        controller?.Play(null, OnShow);
    }

    protected override void OnDispose() {
        controller?.PlayBackwards(null, OnHide);
    }

    protected virtual void OnShow() {

    }

    protected virtual void OnHide() {
        canvas.enabled = false;
    }

}
