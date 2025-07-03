using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Lamp : MonoBehaviour, IStateFloat {

    public event Action<float> onChangeState;

    const string emissionName = "_EmissionColor";
    const string emissionKeywordName = "_EMISSION";

    [SerializeField] new Renderer renderer;
    [SerializeField] Light light;
    [SerializeField] BaconPointLight baconLight;

    [Space]

    [SerializeField] float intensityMax;
    [SerializeField] float intensityMin;
    [SerializeField] Color colorMax;
    [SerializeField] Color colorMin;
    [SerializeField] float intensityLightMax;
    [SerializeField] float intensityLightMin;
    [SerializeField] float startState;
    [SerializeField] AnimationCurve curveState;

    float current;
    float maxBaconInt = 1f;

    bool isOn => current >= 0.5f;

    void Awake() {
        //Before start if something want to setup light on start
        maxBaconInt = baconLight ? baconLight.intensity : 1f;
        renderer.sharedMaterial.EnableKeyword(emissionKeywordName);
        Refresh();
    }

    public float GetState() {
        return current;
    }

    public void SetState(float value) {
        Lerp(value);
    }

    public void Toggle() {
        if (isOn) {
            TurnOff();
        } else {
            TurnOn();
        }
    }

    public void TurnOn() {
        Lerp(1);
    }

    public void TurnOff() {
        Lerp(0);
    }

    public void Lerp(float t) {
        current = t;
        t = curveState.Evaluate(current);
        Color temp = Color.Lerp(colorMin, colorMax, t);
        if (Application.isPlaying) {
            renderer.material.SetColor(emissionName, temp * Mathf.Lerp(intensityMin, intensityMax, t));
        }
        if (light) {
            light.intensity = Mathf.Lerp(intensityLightMin, intensityLightMax, t);
            light.color = temp;
        }
        if (baconLight) {
            baconLight.intensity = Mathf.Lerp(0f, maxBaconInt, t); //todo magic values
            baconLight.NotifyChange();
        }
    }

    [Button]
    void Refresh() {
        current = startState;
        Lerp(current);
    }
}
