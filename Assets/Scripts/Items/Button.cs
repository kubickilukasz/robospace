using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace Items {

    public class Button : MonoBehaviour, IAction {

        public event Action onAction;

        [SerializeField] Transform target;
        [SerializeField] Transform defaultTransform;
        [SerializeField] Transform pressedTransform;
        [SerializeField] float pressTime = 0.2f;
        [SerializeField] float releaseTime = 0.2f;

        public Vector3 directionPress => (pressedTransform.position - defaultTransform.position).normalized;

        bool isPressed => currentValue > 0.6f;
        bool isReleased => currentValue < 0.4f;

        [ShowNonSerializedField] float currentValue = 0f;
        [ShowNonSerializedField] float currentForce = 0f;

       
        void Update() {
            if (currentValue > 0 && currentValue < 1f) {
                AddForce();
            } else {
                currentForce = 0f;
                currentValue = Mathf.Clamp(currentValue, 0f, 1f);
            }
            target.position = Vector3.SlerpUnclamped(defaultTransform.position, pressedTransform.position, currentValue);
        }

        [Button]
        public void Press() {
            if (isPressed) {
                return;
            }
            onAction?.Invoke();
            currentForce = GetForce(pressTime, Vector3.Distance(target.position, pressedTransform.position));
            AddForce();
        }

        [Button]
        public void Release() {
            if (isReleased) {
                return;
            }
            currentForce = GetForce(-releaseTime, Vector3.Distance(target.position, defaultTransform.position));
            AddForce();
        }

        float GetForce(float duration, float distance) {
            return (1f / duration);
        }

        void AddForce() {
            currentValue += currentForce * Time.deltaTime;
        }

    }

}
