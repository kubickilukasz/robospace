using Kubeec.VR.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Lamp))]
public class LeverToLamp : MonoBehaviour{

    [SerializeField] InteractionLever lever;

    Lamp lamp;

    void Start() {
        lamp = GetComponent<Lamp>();
        if (lever != null) {
            lamp.Lerp(lever.GetState());
            lever.onChangeState += lamp.Lerp;
        }
    }

    void OnDestroy() {
        if (lever != null && lamp != null) {
            lever.onChangeState -= lamp.Lerp;
        }
    }
}
