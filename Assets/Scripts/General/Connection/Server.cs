using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Server{

    const string apiKeyStr = "apiKey";
    const string contentTypeStr = "Content-Type";
    const string contentTypeValue = "application/json";

    string url;
    string apiKey;
    Coroutine coroutine;
    MonoBehaviour monoBehaviour;

    public Server(MonoBehaviour monoBehaviour, string url, string apiKey) {
        this.url = url;
        this.monoBehaviour = monoBehaviour;
        this.apiKey = apiKey;
    }

    public bool Post(Endpoint endpoint, string data, Action<string> onComplete) {
        if (coroutine != null) {
            monoBehaviour.StopCoroutine(coroutine);
        }
        Debug.Log(data);
        if (monoBehaviour.isActiveAndEnabled) {
            monoBehaviour.StartCoroutine(InternalPost(endpoint, data, onComplete));
        } else {
            //if mb is not active, then force send this without waiting
            InternalPost(endpoint, data, onComplete);
            onComplete?.Invoke(null);
        }
        return true;
    }

    public bool Get(Endpoint endpoint, Action<string> onComplete){
        if(coroutine != null){
            monoBehaviour.StopCoroutine(coroutine);
        }
        monoBehaviour.StartCoroutine(InternalGet(endpoint, onComplete));
        return true;
    }

    IEnumerator InternalGet(Endpoint endpoint, Action<string> onComplete){
        UnityWebRequest webRequest = UnityWebRequest.Get(url + GetEndpoint(endpoint));
        webRequest.SetRequestHeader(contentTypeStr, contentTypeValue);
        webRequest.SetRequestHeader(apiKeyStr, apiKey);

        yield return webRequest.SendWebRequest();
        while (!webRequest.isDone) {
            yield return null;
        }

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
            onComplete?.Invoke(null);
        } else {
            onComplete?.Invoke(webRequest.downloadHandler.text);
        }
    }

    IEnumerator InternalPost(Endpoint endpoint, string data, Action<string> onComplete) {
        UnityWebRequest webRequest = UnityWebRequest.Post(url + GetEndpoint(endpoint), data, contentTypeValue);
        webRequest.SetRequestHeader(contentTypeStr, contentTypeValue);
        webRequest.SetRequestHeader(apiKeyStr, apiKey);

        yield return webRequest.SendWebRequest();
        while (!webRequest.isDone) {
            yield return null;
        }

        if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError) {
            Debug.LogError(webRequest.error);
            onComplete?.Invoke(null);
        } else {
            onComplete?.Invoke(webRequest.downloadHandler.text);
        }
    }

    public enum Endpoint {
        Get, Insert, Delete
    }

    string GetEndpoint(Endpoint endpoint) {
        switch (endpoint) {
            case Endpoint.Get:
                return "/action/find";
            case Endpoint.Insert:
                return "/action/insertOne";
            case Endpoint.Delete:
                return "/action/deleteOne";
        }
        return string.Empty;
    }

}

