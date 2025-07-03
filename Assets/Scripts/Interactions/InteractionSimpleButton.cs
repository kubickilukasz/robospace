using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items;
using DG.Tweening;
using Kubeec.VR.Outline;

namespace Kubeec.VR.Interactions {

    public class InteractionSimpleButton : InteractionTrigger, IOutlineable {

        const float maxAngleToPress = 80;

        [SerializeField] Button button;
        [SerializeField] List<OutlineObject> outlineObjects;
        [SerializeField] Transform boundPress;

        public override void OnNetworkSpawn() {
            base.OnNetworkSpawn();
            if (status == InteractionStatus.Active) {
                button.Press();
            } else {
                button.Release();
            }
        }

        public bool CanOutline() => true;

        public IEnumerable<OutlineObject> GetOutlineObjects(Vector3 source) {
            return outlineObjects;
        }

        protected override void OnTriggerStay(Collider other) {
            Interactor handler = other.GetComponentInParent<Interactor>();
            if (handler != null && !IsInInteraction(handler)) {
                if (CanInteractButton(handler)) {
                    StartInteract(handler);
                } else {
                    StopInteract(handler);
                }
            }
        }

        protected override void OnTriggerExit(Collider other) {
            Interactor handler = other.GetComponentInParent<Interactor>();
            StopInteract(handler);
        }

        protected override void OnInactive() {
            button.Release();
        }

        protected override void OnActive() {
            button.Press();
        }

        bool CanInteractButton(Interactor handler) {
            Vector3 dir = (boundPress.position - handler.transform.position).normalized;
            if (Vector3.Angle(dir, button.directionPress) < maxAngleToPress) {
                return true;
            }
            return false;
        }
    }

}
