using System;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerHandler : MonoBehaviour{

    public UnityEvent<Collider> onTriggerEnterEvent;
    public UnityEvent<Collider> onTriggerExitEvent;

    public event Action<Collider> onTriggerEnter;
    public event Action<Collider> onTriggerExit;

    void OnTriggerEnter(Collider other) {
        onTriggerEnter?.Invoke(other);
        onTriggerEnterEvent?.Invoke(other);
    }

    void OnTriggerExit(Collider other) {
        onTriggerExit?.Invoke(other);
        onTriggerExitEvent?.Invoke(other);
    }

}
