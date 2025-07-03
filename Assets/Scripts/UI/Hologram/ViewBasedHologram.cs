using Kubeec.VR.Player;
using System.Collections.Generic;
using UnityEngine;

public class ViewBasedHologram : Hologram{

    [SerializeField] Transform[] closePoints;
    [SerializeField] float angles = 60f;
    [SerializeField] float maxDistance = 1.5f;
    [SerializeField] float minTimeToChange = 1f;

    LocalPlayerReference playerReference;
    float sqrMaxDisance;
    float timer = 0f;
    Transform currentPoint;

    void Update() {
        if (IsInitialized()) {
            CheckAngle();
        }
    }

    protected override void OnInit(HologramData data) {
        base.OnInit(data);
        timer = minTimeToChange; 
        sqrMaxDisance = maxDistance * maxDistance;
        playerReference = LocalPlayerReference.instance;
    }

    protected override void OnDispose() {
        base.OnDispose();
        currentPoint = null;
    }

    void CheckAngle() {
        if (timer < minTimeToChange) {
            timer += Time.deltaTime;
            return;
        }
        Transform cameraTransform = playerReference.Camera.transform;
        Vector3 cameraDirection = cameraTransform.forward;
        Transform current = null;
        float minDistance = float.MaxValue;
        foreach (Transform t in closePoints) {
            Vector3 hologramDirection = t.forward;
            float angle = Vector3.Angle(cameraDirection, hologramDirection);
            if (angle > angles) {
                continue;
            }
            
            float sqrDistance = (t.position - cameraTransform.position).sqrMagnitude;
            if (sqrDistance < sqrMaxDisance && sqrDistance < minDistance) {
                minDistance = sqrDistance;
                current = t;
            }
        }
        if (currentPoint != current) {
            timer = 0f;
            currentPoint = current;
            if (currentPoint == null) {
                animationController.PlayBackwards();
            } else {
                animationController.PlayBackwards(onComplete: () => {
                    transform.position = currentPoint.position;
                    transform.rotation = currentPoint.rotation;
                    animationController.Play();
                });
            }
        }
    }


}
