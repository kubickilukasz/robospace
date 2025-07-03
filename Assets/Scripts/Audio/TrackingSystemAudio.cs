using UnityEngine;
using Kubeec.General;

[RequireComponent(typeof(TrackingMachine))]
public class TrackingSystemAudio : AudioEndpoint{

    TrackingMachine machine;

    protected override void OnInit(object data) {
        machine = GetComponent<TrackingMachine>();
        machine.onChangeLocked += OnChangeLocked;
    }

    protected override void OnDispose() {
        base.OnDispose();
        machine.onChangeLocked -= OnChangeLocked;
    }

    void OnChangeLocked(bool value) {
        if (value) {
            Play();
        } else {
            Stop();
        }
    }


}
