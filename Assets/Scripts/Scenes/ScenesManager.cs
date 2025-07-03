using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class ScenesManager {

    const string characterSceneName = "Character";
    const string roomSceneName = "Room";
    const string shipSceneName = "Ship";
    const string shootSceneName = "ShootScene";
    const string menuSceneName = "Menu";

    [SerializeField] SceneType defaultScene;

    Action onStartLoadScene;
    Action onLoadedScene;
    NetworkController controller;
    NetworkSceneManager networkSceneManager => NetworkManager.Singleton.SceneManager;

    public SceneType CurrentScene { get; private set; } = SceneType.Empty;

    ~ScenesManager() {
        if (networkSceneManager != null) {
            networkSceneManager.OnLoadComplete -= OnLoadComplete;
            networkSceneManager.OnUnloadComplete -= OnUnloadComplete;
            networkSceneManager.OnLoadEventCompleted -= OnLoadCompleteAll;
            networkSceneManager.OnUnloadEventCompleted -= OnUnloadCompleteAll;
        }
    }

    public void Init(Action onStartLoadScene, Action onLoadedScene, Action onComplete) {
        this.onStartLoadScene = onStartLoadScene;
        this.onLoadedScene = onLoadedScene;
        controller = NetworkController.instance;
        controller.StartCoroutine(WaitForInit(onComplete));
    }

    public void LoadScene(SceneType sceneType, Action onComplete = null) {
        if (controller.isMaster) {
            SwapScenes(CurrentScene, sceneType, onComplete);
        }
    }

    public void LoadDefaultScene(Action onComplete = null) {
        if (controller.isMaster) {
            SwapScenes(CurrentScene, defaultScene, onComplete);
        }
    }

    public void UnloadCurrent(Action onComplete = null) {
        if (controller.isMaster) {
            SwapScenes(CurrentScene, SceneType.Empty, onComplete);
            //controller.StartCoroutine(NotSafeUnload(roomSceneName, onComplete));
        }
    }

    void SwapScenes(SceneType unloadScene, SceneType loadScene, Action onComplete) {
        controller.StartCoroutine(UnloadAndLoad(unloadScene, loadScene, onComplete));
    }

    IEnumerator UnloadAndLoad(SceneType unloadScene, SceneType loadScene, Action onComplete) {
        onStartLoadScene?.Invoke();
        yield return NotSafeUnload(unloadScene);
        yield return NotSafeLoad(loadScene);
        CurrentScene = loadScene;
        yield return null;
        onComplete?.Invoke();
        onLoadedScene?.Invoke();
    }

    IEnumerator NotSafeLoad(SceneType sceneName) {
        if (sceneName == SceneType.Empty) {
            yield break;
        }
        Scene scene = SceneManager.GetSceneByName(SceneTypeToString(sceneName));
        if (scene.isLoaded) {
            yield break;
        }
        SceneEventProgressStatus status = networkSceneManager.LoadScene(SceneTypeToString(sceneName), LoadSceneMode.Additive);
        yield return WaitForStatus(status);
    }

    IEnumerator NotSafeUnload(SceneType sceneName, Action onComplete = null) {
        if (sceneName == SceneType.Empty) {
            yield break;
        }
        Scene scene = SceneManager.GetSceneByName(SceneTypeToString(sceneName));
        if (!scene.isLoaded) {
            yield break;
        }
        SceneEventProgressStatus status = networkSceneManager.UnloadScene(scene);
        yield return WaitForStatus(status);
        onComplete?.Invoke();
    }

    IEnumerator WaitForStatus(SceneEventProgressStatus status) {
        if (status != SceneEventProgressStatus.Started && status != SceneEventProgressStatus.SceneEventInProgress) {
            yield break;
        }
        while (status == SceneEventProgressStatus.SceneEventInProgress) {
            yield return null;
        }
    }

    IEnumerator WaitForInit(Action onComplete) {
        Scene scene = SceneManager.GetSceneByName(characterSceneName);
        while (!scene.isLoaded || networkSceneManager == null || controller.connectedPlayers == 0) {
            yield return null;
        }
        Debug.Log("NetworkSceneManager Init");
        networkSceneManager.OnLoadComplete += OnLoadComplete;
        networkSceneManager.OnUnloadComplete += OnUnloadComplete;
        networkSceneManager.OnLoadEventCompleted += OnLoadCompleteAll;
        networkSceneManager.OnUnloadEventCompleted += OnUnloadCompleteAll;
        onComplete?.Invoke();
    }

    void OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode) {
        Debug.Log($"OnLoadComplete {clientId} {sceneName} {loadSceneMode}");
    }

    void OnUnloadComplete(ulong clientId, string sceneName) {
        Debug.Log($"OnUnloadComplete {clientId} {sceneName}");
    }

    void OnLoadCompleteAll(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        Debug.Log($"OnLoadCompleteAll {sceneName} {loadSceneMode}");
    }

    void OnUnloadCompleteAll(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        Debug.Log($"OnUnloadCompleteAll {sceneName} {loadSceneMode}");
    }

    string SceneTypeToString(SceneType type) {
        switch (type) {
            case SceneType.ShootScene:return shootSceneName;
            case SceneType.ShipScene: return shipSceneName;
            case SceneType.Menu: return menuSceneName;
            case SceneType.Empty: return string.Empty;
            default: return roomSceneName;
        }
    }

    public enum SceneType {
        Empty ,Room, ShootScene, ShipScene, Prologue, Menu
    }

}
