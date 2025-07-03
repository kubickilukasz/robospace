using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text;
using NaughtyAttributes;
using System.Linq;
using System.Globalization;

[RequireComponent(typeof(NetworkManager))]
public class NetworkController : Singleton<NetworkController>{

    public event Action<ulong> onClientConnect;
    public event Action<ulong> onClientDisconnect;
    public event Action onJoinRoom;
    public event Action onLeaveRoom;
    public event Action onStartPublicHost;
    public event Action onStopPublicHost;
    public event Action onStartMaster;
    public event Action onStopMaster;


    const string urlToCheckGlobalIp = "https://api.ipify.org";
    const string defaultAddress = "127.0.0.1";
    const string defaultThirdAddress = "0.0.0.0";
    const ushort port = 2137;

    public bool isRunning => isMaster || networkManager.IsClient;

    public bool isMaster => networkManager.IsServer;

    public ulong id => networkManager.LocalClientId;

    public int connectedPlayers => networkManager.ConnectedClients.Count;

    string localAddress;
    string globalAddress;
    Coroutine coroutine;
    bool isPublicHost = false;
    bool isClient = false;

    NetworkManager _networkManager;
    NetworkManager networkManager {
        get {
            if (_networkManager == null) {
                _networkManager = GetComponent<NetworkManager>();
            }
            return _networkManager;
        }
    }

    UnityTransport _unityTransport;
    UnityTransport unityTransport {
        get {
            if (_unityTransport == null) {
                _unityTransport = GetComponent<UnityTransport>();
            }
            return _unityTransport;
        }
    }

    void OnDestroy() {
        if (unityTransport != null) {
            unityTransport.OnTransportEvent -= OnTransportEvent;
        }
        if (networkManager != null) {
            networkManager.OnServerStarted -= OnServerStart;
            networkManager.OnServerStopped -= OnServerStop;
            networkManager.OnClientConnectedCallback -= OnClientConnect;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnect;
            networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        }
    }

    public void Init() {
        unityTransport.OnTransportEvent += OnTransportEvent;
        networkManager.OnServerStarted += OnServerStart;
        networkManager.OnServerStopped += OnServerStop;
        networkManager.OnClientConnectedCallback += OnClientConnect;
        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        GetAddress();
    }

    public void Restart() {
        DisconnectClients();
        TryStartHost(true, true);
    }

    public void DisconnectClients() {
        if (isMaster) {
            List<ulong> ids = networkManager.ConnectedClientsIds.ToList();
            foreach (ulong id in ids) {
                if (networkManager.LocalClientId != id) {
                    networkManager.DisconnectClient(id, "force");
                }
            }
        }
    }

    [Button]
    public void ConnectLocalHost() {
        MyDebug.Log(MyDebug.TypeLog.Network, $"Try connect to myself {defaultAddress} {port}");
        coroutine = StartCoroutine(ConnectClient(defaultAddress, port, defaultThirdAddress));
    }

    public void Connect(string ip, string localIp) {
        if (coroutine != null) {
            return;
        }

        //if global adresses are the same, then connect in LAN or myself
        if (globalAddress.Equals(ip)) {
            if (localIp.Equals(localAddress)) {
                ConnectLocalHost();
            } else {
                MyDebug.Log(MyDebug.TypeLog.Network, $"Try connect by LAN {localIp} {port}");
                coroutine = StartCoroutine(ConnectClient(localIp, port, defaultThirdAddress));
            }
        } else {
            MyDebug.Log(MyDebug.TypeLog.Network, $"Try connect by WAN {ip} {port} {localIp}");
            coroutine = StartCoroutine(ConnectClient(ip, port, localIp));
        }
    }

    public RoomData Host(string name, string username, string code, string password) {
        if (!TryStartHost(false)) {
            return null;
        }

        if (globalAddress.Equals(defaultAddress)) {
            GetAddress();
        }

        RoomData roomData = new RoomData();        
        roomData.ip = globalAddress;
        roomData.localIp = localAddress;
        roomData.date = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz", CultureInfo.InvariantCulture);
        roomData.name = name;
        roomData.user = username;
        roomData.password = password.HashPassword();
        roomData.code = code;

        return roomData;
    }

    IEnumerator StartHost(bool force, bool innerCall = false) {
        if (networkManager.IsHost && !force) {
            coroutine = null;
            yield break;
        }else if (isRunning) {
            yield return Shutdown();
        }
        unityTransport.SetConnectionData(defaultAddress, port, defaultThirdAddress);
        isPublicHost = !innerCall;
        isClient = false;
        if (networkManager.StartHost()) {
            MyDebug.Log(MyDebug.TypeLog.Network, "HOST started");
        } else {
            isPublicHost = false;
        }
        coroutine = null;
    }

    IEnumerator ConnectClient(string ip, ushort port, string localIp) {
        if (isRunning) {
            yield return Shutdown();
        }
        isClient = true;
        //wan, port, local
        unityTransport.SetConnectionData(ip, port, localIp);
        if (networkManager.StartClient()) {
            MyDebug.Log(MyDebug.TypeLog.Network, "CLIENT started");
        }
        coroutine = null;
    }

    IEnumerator Shutdown() {
        networkManager.Shutdown();
        while (networkManager.ShutdownInProgress) {
            yield return null;
        }
    }

    void OnTransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receiveTime) {
        MyDebug.Log(MyDebug.TypeLog.Network, $"OnTransportEvent: {eventType} {clientId} {payload} {receiveTime}");
    }

    bool TryStartHost(bool force, bool innerCall = false) {
        if (coroutine != null) {
            return false;
        }
        coroutine = StartCoroutine(StartHost(force, innerCall));
        return true;
    }

    void OnServerStart() {
        onStartMaster?.Invoke();
        if (isPublicHost) {
            onStartPublicHost?.Invoke();
        }
    }

    void OnServerStop(bool wasHost) {
        onStopMaster?.Invoke();
        if (isPublicHost) {
            isPublicHost = false;
            onStopPublicHost?.Invoke();
        }
    }

    void OnClientConnect(ulong clientId) {
        if (isClient) {
            onJoinRoom?.Invoke();
        } else {
            onClientConnect?.Invoke(clientId);
        }
    }

    void OnClientDisconnect(ulong clientId) {
        MyDebug.Log(MyDebug.TypeLog.Network, "On Client Disconnect ", clientId, networkManager.DisconnectReason);
        if (isClient) {
            onLeaveRoom?.Invoke();
        } else {
            onClientDisconnect?.Invoke(clientId);
        }
    }

    void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) {
        //TODO PASSWORD
        response.Approved = isPublicHost || networkManager.IsHost;
        response.CreatePlayerObject = true;

        response.Position = Vector3.zero;
        response.Rotation = Quaternion.identity;

        if (!response.Approved) {
            response.Reason = "This isn't public host!";
        }

        response.Pending = false;
    }

    void GetAddress() {
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                localAddress = ip.ToString();
                break;
            }
        }
        //Get the global IP
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlToCheckGlobalIp);
        request.Method = "GET";
        request.Timeout = 1000; //time in ms
        try {
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK) {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                globalAddress = reader.ReadToEnd();
            } 
            else {
                MyDebug.LogError(MyDebug.TypeLog.Network, "Timed out? " + response.StatusDescription);
                globalAddress = defaultAddress;
            } 
        }
        catch (WebException ex) {
            MyDebug.LogError(MyDebug.TypeLog.Network, "Likely no internet connection: " + ex.Message);
            globalAddress = defaultAddress;
        }
    }


}
