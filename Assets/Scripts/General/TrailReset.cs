using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailReset : EnableDisableInitableDisposable{

    TrailRenderer trailRenderer;

    protected override void OnInit(object data) {
        trailRenderer ??= GetComponent<TrailRenderer>();
        trailRenderer.Clear();
    }

    protected override void OnDispose() {
        trailRenderer ??= GetComponent<TrailRenderer>();
        trailRenderer.Clear();
    }

}
