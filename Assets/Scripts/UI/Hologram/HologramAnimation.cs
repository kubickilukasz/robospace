using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using System;

namespace UI {
    public class HologramAnimation : AnimationBase {

        [SerializeField] float duration = 0.5f;
        [SerializeField] float delay = 0.3f;
        [SerializeField] Ease ease = Ease.InOutSine;
        [SerializeField] List<RectTransform> elements = new List<RectTransform>();

        Sequence sequence;
        Vector2 sizeDelta;
        float progress;

        protected override void OnInit(object boj = null) {
            sizeDelta = rectTransform.sizeDelta;
        }

        void OnDisable() {
            if (sequence != null) {
                progress = 0f;
                sequence.Kill();
                sequence = null;
            }
        }

        public override void Pause() {
            if (sequence != null) {
                progress = sequence.position;
                sequence.Pause();
            }
        }

        public override void Play(float? duration = null, Action onComplete = null) {
            Stop();
            duration ??= this.duration;
            sequence = DOTween.Sequence();
            float delayValue = Mathf.Max(delay - progress, 0);
            float durationValue = Mathf.Max(duration.Value - progress, 0);
            sequence.Append(rectTransform.DOSizeDelta(sizeDelta, durationValue));
            foreach (var element in elements) {
                element.localScale = Vector3.zero;
                sequence.Insert(delayValue, element.DOScale(Vector3.one, durationValue));
            }
            sequence.SetEase(ease);
            sequence.OnComplete(() => onComplete?.Invoke());
            sequence.Play();
            progress = 0f;
        }

        public override void PlayBackwards(float? duration = null, Action onComplete = null) {
            Stop();
            duration ??= this.duration;
            sequence = DOTween.Sequence();
            float delayValue = Mathf.Max(delay - progress, 0);
            float durationValue = Mathf.Max(duration.Value - progress, 0);
            foreach (var element in elements) {
                sequence.Insert(0, element.DOScale(Vector3.zero, durationValue));
            }
            sequence.Insert(delayValue, rectTransform.DOSizeDelta(Vector2.zero, durationValue));
            sequence.SetEase(ease);
            sequence.OnComplete(() => onComplete?.Invoke());
            sequence.Play();
            progress = 0f;
        }

        public override void Stop() {
            if (sequence != null) {
                progress = 0f;
                sequence.Complete();
                sequence = null;
            }
        }

        public override void ResetToPlay() {
            Play();
            Stop();
        }

        public override void ResetToPlayBackwards() {
            PlayBackwards();
            Stop();
        }

        public override float GetPlayDuration() {
            return duration;
        }

        public override float GetPlayBackwardsDuration() {
            return duration;
        }
    }

}
