using UnityEngine;

namespace Kubeec.NPC {

    public class NPCAnimator : NPCInitable {

        const string speedKey = "speed";
        const string flyingKey = "flying";
        const string loadingKey = "loading";

        [SerializeField] Animator animator;

        int speedHashKey, flyingHashKey, loadingHashKey;

        void Reset() {
            animator = GetComponentInChildren<Animator>(true);
        }

        void Update() {
            if (!IsInitialized()) {
                return;
            }
            animator.SetBool(flyingHashKey, !Character.Move.IsOnGround());
            animator.SetFloat(speedHashKey, Character.Move.GetCurrentSpeed());
        }

        protected override void OnInit(NonPlayerCharacter data) {
            base.OnInit(data);
            speedHashKey = Animator.StringToHash(speedKey);
            flyingHashKey = Animator.StringToHash(flyingKey);
            loadingHashKey = Animator.StringToHash(loadingKey);
        }

        public void SetShooting(float value) {
            animator.SetFloat(loadingHashKey, value);
        }

    }
}
