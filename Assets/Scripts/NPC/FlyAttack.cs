using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kubeec.NPC {

    public class FlyAttack : NPCInitable, IAction {

        public event Action onAction;

        [SerializeField] EffectBase effect;
        [SerializeField] NPCAnimator animator;
        [SerializeField] AudioProcessor loadingAudio;
        [SerializeField] List<Collider> collidersActiveDuringLoading = new();
        [SerializeField] GunOutput bulletOutput;

        [Space]

        [SerializeField] float cooldown = 3f;
        [SerializeField] float loadingTime = 2f;

        bool isCooldown = false;
        bool isLoading = false;

        public bool IsLateStageLoading { get; private set; } = false;
        public bool IsCooldown => isCooldown;
        public bool IsLoading => isLoading;

        protected override void OnInit(NonPlayerCharacter data) {
            base.OnInit(data);
            isCooldown = false;
            IsLateStageLoading = false;
            isLoading = false;
            animator?.SetShooting(0f);
            collidersActiveDuringLoading.ForEach(c => c.enabled = false);
        }

        protected override void OnDispose() {
            base.OnDispose();
            StopAllCoroutines();
        }

        public bool TryShot() {
            if (isCooldown || isLoading) {
                return false;
            }
            IsLateStageLoading = false;
            animator?.SetShooting(1f);
            isLoading = true;
            StartCoroutine(LoadingAndShot());
            return true;
        }

        void Shot() {
            if (bulletOutput.Shot(gameObject)) {
                animator?.SetShooting(0f);
                onAction?.Invoke();
                isCooldown = true;
                Transform bulletOutput = this.bulletOutput.StartShootTransform;
                effect?.CreateAndPlay(bulletOutput.position, bulletOutput.rotation, bulletOutput);
                StartCoroutine(Cooling());
            }
        }

        IEnumerator LoadingAndShot() {
            collidersActiveDuringLoading.ForEach(c => c.enabled = true);
            float timer = loadingTime;
            loadingAudio?.Play();
            IsLateStageLoading = false;
            while (timer > 0) {
                float t = (1f - timer / loadingTime).SineIn();
                loadingAudio?.SetPitch(t);
                animator?.SetShooting(t);
                timer -= Time.deltaTime;
                if (timer / loadingTime < 0.3f) {
                    IsLateStageLoading = true;
                }
                yield return null; 
            }
            loadingAudio?.Stop();
            collidersActiveDuringLoading.ForEach(c => c.enabled = false);
            Shot();
            IsLateStageLoading = false;
            isLoading = false;
        }


        IEnumerator Cooling() {
            yield return new WaitForSeconds(cooldown);
            isCooldown = false;
        }

    }

}
