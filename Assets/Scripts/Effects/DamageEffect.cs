using TMPro;
using UnityEngine;
using DG.Tweening;
using Kubeec.VR.Interactions;

public class DamageEffect : EffectBase {

    [SerializeField] TextMeshPro textMeshPro;
    [SerializeField] Vector3 startScale;
    [SerializeField] Vector3 targetScale;
    [SerializeField] Vector3 targetOffsetPosition;
    [SerializeField] Ease ease;

    Sequence sequence;
    public float Damage { get; private set; }

    void Reset() {
        textMeshPro = GetComponentInChildren<TextMeshPro>();
    }

    public void Set(GameObject owner, float damage, Transform origin = null) {
        Damage = damage;
        textMeshPro.SetText(damage.GetString(0));
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        Damage = 0f;
    }

    protected override void OnPlay() {
        ResetSequence();
        textMeshPro.transform.localScale = startScale;
        sequence = DOTween.Sequence();
        sequence.Append(textMeshPro.transform.DOScale(targetScale, duration).SetEase(ease));
        sequence.Insert(0, textMeshPro.transform.DOMove(transform.TransformPoint(targetOffsetPosition), duration).SetEase(ease));
    }

    protected override void OnStart() {
        ResetSequence();
        textMeshPro.transform.localScale = startScale;
    }

    protected override void OnStop() {
        ResetSequence();
    }

    void ResetSequence() {
        if (sequence != null) {
            sequence.Kill();
            sequence = null;
        }
    }


}
