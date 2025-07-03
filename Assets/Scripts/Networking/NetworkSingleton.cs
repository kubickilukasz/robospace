using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkSingleton<T> {

    public static T _instance;

    public static T instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    public static bool instanceExist => instance != null;

    void Awake() {
        if (_instance == null) {
            _instance = (T)this;
        } else if (_instance != this) {
            DestroyImmediate(this);
        }
    }

}
