using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using JetBrains.Annotations;

public class ScreenHUD : NetworkBehaviour{

    [SerializeField] float timePerCharacter = 0f;
    [SerializeField] float timeLag = 0f;
    [SerializeField] TextMeshPro _textMeshPro;

    string tempText = null;

    public override void OnNetworkSpawn() {
        if (tempText != null) {
            SetTextServerRpc(tempText);
        }
    }

    public void SetText(string text) {
        if (IsSpawned) {
            SetTextServerRpc(text);
        } else {
            tempText = text;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SetTextServerRpc(string text) {
        SetTextClientRpc(text);
    }

    [ClientRpc]
    void SetTextClientRpc(string text) {
        StopAllCoroutines();
        if (timePerCharacter > 0f || timeLag > 0f) {
            StartCoroutine(WaitAndShow(text));
        } else {
            _textMeshPro.SetText(text);
        }
    }

    IEnumerator WaitAndShow(string text) {
        string output = "";
        _textMeshPro.SetText(output);
        yield return new WaitForSeconds(timeLag);
        for (int i = 0; i < text.Length; i++) {
            yield return new WaitForSeconds(timePerCharacter);
            output += text[i];
            _textMeshPro.SetText(output);
        }
    }

}
