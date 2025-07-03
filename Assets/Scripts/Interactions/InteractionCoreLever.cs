using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubeec.VR.Interactions {

    public class InteractionCoreLever : InteractionLever {

        public Action onCoreLeverActive;
        public Action onCoreLeverDeactive;

        [SerializeField] InteractionSocket socket;
        [SerializeField] float minStateToActive = 0.15f;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            RefreshSocket();
        }

        protected override void OnStateChange(float oldValue, float newValue) {
            base.OnStateChange(oldValue, newValue);
            RefreshSocket();
        }

        void RefreshSocket() {
            float state = GetState();
            if (state < minStateToActive && !socket.CanInteract) {
                socket.CanInteract = true;
                onCoreLeverDeactive?.Invoke();
            } else if(state > minStateToActive && socket.CanInteract) {
                socket.CanInteract = false;
                onCoreLeverActive?.Invoke();
            }
        }

    }

}
