using Kubeec.Hittable;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandHealthBar : EnableDisableInitableDisposable{

    [SerializeField] HitCollector hitCollector;
    [SerializeField] ColorDataReference maxHealthData;
    [SerializeField] ColorDataReference lowHealthData;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] List<Image> cells = new();
    [SerializeField] List<Image> others = new();
    [SerializeField] float minAlpha = 0.5f;

    float currentNormalizedHealth = 1f;

    protected override void OnInit(object data) {
        hitCollector.onHit += OnHit;
        hitCollector.onInit += OnCollectorInit;
        hitCollector.onDispose += OnCollectorDispose;
        OnCollectorInit();
    }

    protected override void OnDispose() {
        canvasGroup.alpha = 0.0f;
        if (hitCollector) {
            hitCollector.onHit -= OnHit;
            hitCollector.onInit -= OnCollectorInit;
            hitCollector.onDispose -= OnCollectorDispose;
        }
    }

    void OnHit(HitInfo info) {
        Refresh();
    }

    void OnCollectorInit() {
        canvasGroup.alpha = 1.0f;
        Refresh();
    }

    void OnCollectorDispose() {
        canvasGroup.alpha = 0.0f;
    }

    void Refresh() {
        currentNormalizedHealth = hitCollector.CurrentHealth / hitCollector.MaxHealth;
        int count = cells.Count;
        float step = 1f / count;
        float value = currentNormalizedHealth;
        Color newColor = Color.Lerp(lowHealthData.Get(), maxHealthData.Get(), currentNormalizedHealth);
        others.ForEach(x => x.color = newColor);
        for (int i = 0; i < count; i++) {
            cells[i].color = newColor;
            cells[i].color = cells[i].color.SetAlpha(value > step ? 1f : value == 0f ? 0f : Mathf.Lerp(minAlpha, 1f, value));
            value -= step;
            value = Mathf.Max(value, 0f);
        }
    }

}
