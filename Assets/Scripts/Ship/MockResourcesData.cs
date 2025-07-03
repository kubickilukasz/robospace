using UnityEngine;
using System;
using Kubeec.General;

public class MockResourcesData : Singleton<MockResourcesData> {

    public event Action<float> onChange;

    public float resources { get; private set; }

    protected override void OnInit() {
        resources = 500;
    }

    public void Set(float resources) {
        this.resources = resources;
        onChange?.Invoke(resources);
    }

    public void Add(float resources) {
        Set(this.resources + resources);
    }

}
