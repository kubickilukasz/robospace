using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>{

    public event Action onStartLoadScene;
    public event Action onEndLoadScene;

    [SerializeField] ScenesManager scenesManager;
    NetworkController networkController;
    RoomsManager roomsManager;

    public bool IsLoadingScene { get; private set; }

    public bool isMaster => networkController.isMaster;

    bool shouldRestart = false;
    Action onJoinRoom;

    void Update() {
        if (shouldRestart && !networkController.isRunning) {
            networkController.Restart();
            shouldRestart = false;
        }
    }

    protected override void OnInit() {
        base.OnInit();
        DontDestroyOnLoad(gameObject);
        networkController = NetworkController.instance;
        roomsManager = RoomsManager.instance;
        DontDestroyOnLoad(roomsManager.gameObject);

        this.InvokeOnEndOfFrame(() => {
            networkController.Init();
            networkController.onStartMaster += OnRestartMaster;
            networkController.onStopMaster += KeepHostAlive;
            networkController.onStopPublicHost += CleanUp;
            networkController.onJoinRoom += OnJoinRoom;
            networkController.onLeaveRoom += OnLeaveRoom;
            KeepHostAlive();
        });
    }

    public void JoinRoom(string ip, string localIp, Action onComplete) {
        scenesManager.UnloadCurrent(() => {
            onJoinRoom += onComplete;
            networkController.Connect(ip, localIp);
        });
    }

    public void StopGame() {
        networkController.DisconnectClients();
        scenesManager.LoadScene(ScenesManager.SceneType.Room);
    }

    public void StartRoom(string name, string username, string code, string password, Action callback) {
        RoomData data = networkController.Host(name, username, code, password);
        if (data != null) {
            scenesManager.LoadScene(ScenesManager.SceneType.Room, () => {
                roomsManager.InsertRoom(data, callback);
            });
        } else {
            callback?.Invoke();
        }
    }

    public bool TryLoadScene(ScenesManager.SceneType sceneType, Action onComplete = null) {
        sceneType = sceneType == ScenesManager.SceneType.Empty ? scenesManager.CurrentScene : sceneType;
        if (VignetteSceneController.Instance != null) {
            VignetteSceneController.Instance.SetFadeInFadeOut(() => scenesManager.LoadScene(sceneType, onComplete));
        } else {
            scenesManager.LoadScene(sceneType, onComplete);
        }
        return true;
    }

    public void StartGame() {
        if (!isMaster) {
            return;
        }
        scenesManager.LoadScene(ScenesManager.SceneType.ShipScene);
        roomsManager.DeleteRoom(null);
    }

    void OnJoinRoom() {
        onJoinRoom?.Invoke();
        onJoinRoom = null;
    }

    void OnLeaveRoom() {
        shouldRestart = true;
    }

    void KeepHostAlive() {
        if (networkController.isRunning) {
            OnRestartMaster();
        } else {
            networkController.Restart();
        }
    }

    void CleanUp() {
        roomsManager.DeleteRoom();
    }

    void OnRestartMaster() {
        scenesManager.Init(OnStartLoadScene, OnLoadedScene, () => { scenesManager.LoadDefaultScene(); });
    }

    void OnStartLoadScene() {
        IsLoadingScene = true;
        onStartLoadScene?.Invoke();
    }

    void OnLoadedScene() {
        IsLoadingScene = false;
        onEndLoadScene?.Invoke();
    }

}
