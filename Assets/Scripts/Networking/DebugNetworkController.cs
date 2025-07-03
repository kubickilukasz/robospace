using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
public class DebugNetworkController : Singleton<DebugNetworkController> {

    void Start() {
        NetworkManager.Singleton.StartHost();
    }

}
