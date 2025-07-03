using System.Collections.Generic;
using UnityEngine;

public class TestInstantiateCopy : EnableDisableInitableDisposable{

    [SerializeField] int number;
    [SerializeField] GameObject target;
    [SerializeField] Vector3 offset;

    List<GameObject> gos = new List<GameObject>();  

    protected override void OnInit(object data) {
        for (int i = 0; i < number; i++) {
            GameObject go = Instantiate(target);
            go.transform.position = transform.position + offset * i;
            gos.Add(go);
        }
    }

    protected override void OnDispose() {
        foreach (GameObject go in gos) {
            go.SafeSelftDestroy();
        }
        gos.Clear();
    }

}
