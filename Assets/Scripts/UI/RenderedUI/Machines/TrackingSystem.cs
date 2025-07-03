using DG.Tweening;
using UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TrackingSystem : MachineSystem {

    [SerializeField] RectTransform pattern;
    [SerializeField] RectTransform content;
    [SerializeField] RectTransform cursor;
    [SerializeField] Panel panel;
    [SerializeField] Image target;
    [SerializeField] CanvasGroup targetCanvasGroup;
    [SerializeField] Vector3 sizeLockedCursor = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] TextMeshProUGUI targetInfo;
    [SerializeField] TextMeshProUGUI cursorInfo;
    [SerializeField] LocalizedString cursorInfoTextLocked;

    Sequence sequencePattern;
    Sequence sequenceTarget;
    Sequence sequenceCursor;
    Sequence sequenceCursorPing;
    float targetTimer = 0f;

    void Update() {
        if (targetTimer > 0f) {
            targetTimer -= Time.deltaTime;
        }
    }

    public void ShowTarget(Vector2 position, float? time = null, string targetInfoString = null) {
        target.rectTransform.anchoredPosition = new Vector2(position.x * content.rect.width, position.y * content.rect.height) / 2f;
        target.enabled = true;
        if (time.HasValue) {
            targetTimer = time.Value;
            if (sequenceTarget != null) {
                sequenceTarget.Kill();
            }
            targetCanvasGroup.alpha = 1f;
            sequenceTarget = DOTween.Sequence();
            sequenceTarget.Append(targetCanvasGroup.DOFade(0f, targetTimer));
        }
        if (targetInfoString != null) {
            targetInfo.SetText(targetInfoString);
        }
    }

    public void PingCursor(float time) {
        if (sequenceCursorPing != null) {
            sequenceCursorPing.Kill();
        }
        sequenceCursorPing = DOTween.Sequence();
        sequenceCursorPing.Append(cursor.DOPunchScale(sizeLockedCursor, time, 3, 0.2f));
    }

    public void SetCursor(Vector2 position) {
        cursor.anchoredPosition = new Vector2(position.x * content.rect.width, position.y * content.rect.height) / 2f;
    }

    public void SetLocked(bool isLocked, float? time = null) {
        cursor.localScale = isLocked ? sizeLockedCursor : Vector3.one;
        cursorInfo.SetText(isLocked && time.HasValue ? cursorInfoTextLocked.GetLocalizedString(time.Value) : null);
        if (sequenceTarget != null && isLocked) {
            sequenceTarget.Kill();

        }
    }

    protected override void OnShow() {
        base.OnShow();
        AnimatePattern();
        panel.Show();
        AnimateCursor();
    }

    protected override void OnHide() {
        base.OnHide();
        if (sequencePattern != null) {
            sequencePattern.Kill();
        }
        if (sequenceCursor != null) {
            sequenceCursor.Kill();
        }
        if (sequenceCursorPing != null) {
            sequenceCursorPing.Kill();
        }
        if (sequenceTarget != null) {
            sequenceTarget.Kill();
        }
    }

    void Animate(ref Sequence sequence, RectTransform rectTransform, float duration, Action onComplete) {
        if (sequence != null) {
            sequence.Kill();
        }
        sequence = DOTween.Sequence();
        sequence.Append(rectTransform.DORotate(Vector3.forward * UnityEngine.Random.Range(-180, 180), duration, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine));
        sequence.OnComplete(() => onComplete?.Invoke());
    }

    void AnimatePattern() {
        Animate(ref sequencePattern, pattern, 2f, AnimatePattern);
    }

    void AnimateCursor() {
        Animate(ref sequenceCursor, cursor, 0.8f, AnimateCursor);
    }


}
