using Kubeec.VR.Player;
using System;
using UnityEngine;

[RequireComponent(typeof(Vignette))]
public class VignetteSceneController : LazySingleton<VignetteSceneController> {

    Vignette vignette;
    GameManager gameManager;

    protected override void OnInit(object data) {
        base.OnInit(data);
        gameManager = GameManager.instance;
        vignette = GetComponent<Vignette>();
        if (vignette.IsInitialized()) {
            vignette.SetFadeInFadeOut(() => !gameManager.IsLoadingScene, null, null);
        } else {
            vignette.onInit += OnInitVignette;
        }
    }

    public void SetFadeInFadeOut(Action onFadeIn) {
        vignette.SetFadeInFadeOut(() => !gameManager.IsLoadingScene, onFadeIn, null);
    }

    void OnInitVignette() {
        vignette.onInit -= OnInitVignette;
        vignette.SetFadeInFadeOut(() => !gameManager.IsLoadingScene, null, null);
    }

}
