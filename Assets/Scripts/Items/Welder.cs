using UnityEngine;
using Kubeec.VR.Interactions;
using Kubeec.Hittable;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class Welder : TriggerItemAddition {

    [SerializeField] float maxResourcesPerSecond = 10f;

    [Space]
    [SerializeField] RayHitProvider rayHitProvider;

    [Header("Effects")]
    [SerializeField] EffectBase flameEffect;
    [SerializeField] EffectBase hitEffect;
    [SerializeField] Hologram hologram;

    HologramData hologramData;
    IStateFloat stateVariableFlameEffect;
    bool isHit = false;
    HitInfo hitInfo;

    protected override void Update() {
        if (IsInitialized()) {
            base.Update();
            if (isHit) {
                hitInfo.hitReceiver.TakeHit(hitInfo); //TODO resources
                MockResourcesData.instance.Add(-maxResourcesPerSecond * Time.deltaTime);
                hitEffect.transform.position = hitInfo.position.Value;
                hitEffect.transform.rotation = Quaternion.LookRotation(hitInfo.normal.Value, Vector3.up);
            }
        }
    }

    void FixedUpdate() {
        if (!IsInitialized() || !rayHitProvider.CanRaycast()) {
            return;
        }
        if (currentForce > 0f && rayHitProvider.TryRaycast(out hitInfo)) {
            hitInfo.damage *= currentForce;
            isHit = true;
            if (!hitEffect.IsPlaying()) {
                hitEffect.Play();
            }
        } else {
            hitEffect.Stop();
            isHit = false;
        }
    }

    protected override void OnInit(object data) {
        base.OnInit(data);
        hologramData = new HologramData();
        stateVariableFlameEffect = flameEffect as IStateFloat;
        MockResourcesData.instance.onChange += UpdateText;
    }

    protected override void OnDispose() {
        base.OnDispose();
        MockResourcesData.instance.onChange -= UpdateText;
        hologram?.Dispose();
    }

    protected override void UpdateForce(float value) {
        base.UpdateForce(value);
        if (currentForce > 0f) {
            if (!flameEffect.IsPlaying()) {
                flameEffect.Play();
            }
            flameEffect.transform.localScale = Vector3.one * currentForce;
            stateVariableFlameEffect?.SetState(currentForce);
        } else {
            isHit = false;
            stateVariableFlameEffect?.SetState(0f);
            flameEffect.Stop();
            hitEffect.Stop();
        }
    }

    protected override float ModifyValue(float value) {
        return MockResourcesData.instance.resources > 0f ? value : 0f;
    }

    protected override void RefreshHands() {
        base.RefreshHands();
        if (hands.Count > 0) {
            hologramData.baseValue = Mathf.FloorToInt(MockResourcesData.instance.resources).ToString();
            hologram.Init(hologramData);
        } else {
            hologram.Dispose();
        }
    }

    void UpdateText(float resources) {
        hologramData.baseValue = Mathf.FloorToInt(resources).ToString();
        hologram?.UpdateState(hologramData);
    }


}
