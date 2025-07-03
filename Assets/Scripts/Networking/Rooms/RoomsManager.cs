using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class RoomsManager : Singleton<RoomsManager>{

    const string url = "https://eu-central-1.aws.data.mongodb-api.com/app/data-hxvfngn/endpoint/data/v1";
    const string apiKey = "OAVaZ4KrtQxpbRdaRktGo7J5K3S1cQqIfFR7f8Uo83NweR4f4aUynLJJDRUeiMBw";
    const string dataSource = "Cluster0";
    const string database = "robospace";
    const string collection = "rooms";

    bool isInserted = false;

    RoomData currentCreatedRoom;

    protected override void OnInit() {
        base.OnInit();
        DontDestroyOnLoad(this);
    }

    void OnDestroy() {
        if (currentCreatedRoom != null) {
            DeleteRoom(null);
        }
    }

    public void GetList(Action<List<RoomData>> callback) {
        Server server = new Server(this, url, apiKey);

        GetEndpointDataInput data = new GetEndpointDataInput() {
            filter = new (),
            projection = new (),
            sort = new (),
            limit = 100, //TODO
        };
        data = FillBaseDataInput(data) as GetEndpointDataInput;
        server.Post(Server.Endpoint.Get, JsonUtility.ToJson(data), output => {
            Debug.Log(output);
            if (output.Contains("error")) {
                callback?.Invoke(null);
                return;
            }

            GetEndpointDataOutput roomDatas = JsonUtility.FromJson(output, typeof(GetEndpointDataOutput)) as GetEndpointDataOutput;
            if (roomDatas != null && roomDatas.documents != null) {
                callback?.Invoke(roomDatas.documents);
            } else {
                callback?.Invoke(null);
            }
        });
    }

    public void InsertRoom(RoomDataInput roomData, Action onComplete) {
        if (isInserted) {
            onComplete?.Invoke();
            return;
        }
        currentCreatedRoom = roomData as RoomData;
        isInserted = true;
        Server server = new Server(this, url, apiKey);
        PostEndpointDataInput inputData = new PostEndpointDataInput() {
            document = roomData
        };
        inputData = FillBaseDataInput(inputData) as PostEndpointDataInput;
        server.Post(Server.Endpoint.Insert, JsonUtility.ToJson(inputData), output => {
            try {
                InsertOutput insertOutput = JsonUtility.FromJson<InsertOutput>(output);
                if (insertOutput != null && currentCreatedRoom != null) {
                    currentCreatedRoom._id = insertOutput.insertedId;
                } else {
                    MyDebug.LogError(MyDebug.TypeLog.Database, "Can't get id");
                }
            } catch {
                MyDebug.LogError(MyDebug.TypeLog.Database, "Can't get id");
            }
            MyDebug.Log(MyDebug.TypeLog.Database, inputData, output);
            onComplete?.Invoke();
        });
    }

    public void DeleteRoom(Action onComplete = null) {
        if (currentCreatedRoom == null || string.IsNullOrEmpty(currentCreatedRoom._id)) {
            onComplete?.Invoke();
            return;
        }

        Server server = new Server(this, url, apiKey);
        DeleteEndpointDataInput inputData = new DeleteEndpointDataInput() {
            filter = new() { _id = currentCreatedRoom._id }
        };
        inputData = FillBaseDataInput(inputData) as DeleteEndpointDataInput;
        server.Post(Server.Endpoint.Delete, JsonUtility.ToJson(inputData), output => {
            MyDebug.Log(MyDebug.TypeLog.Database, inputData, output);
            onComplete?.Invoke();
        });
        currentCreatedRoom = null;
    }

    BaseDataInput FillBaseDataInput(BaseDataInput dataInput) {
        dataInput.dataSource = dataSource;
        dataInput.database = database;
        dataInput.collection = collection;
        return dataInput;
    }

    #region DataStructures

    [Serializable]
    class BaseDataInput {
        public string dataSource;
        public string database;
        public string collection;
    }

    [Serializable]
    class PostEndpointDataInput : BaseDataInput {
        public RoomDataInput document;
    }

    [Serializable]
    class DeleteEndpointDataInput : BaseDataInput {
        public Filter filter;

        [Serializable]
        public class Filter {
            public string _id;
        }
    }

    [Serializable]
    class GetEndpointDataInput : BaseDataInput {
        public Dictionary<string, string> filter;
        public Dictionary<string, string> projection;
        public Sort sort;
        public int limit = 100;

        public class Sort {}
    }

    [Serializable]
    class GetEndpointDataOutput {
        public List<RoomData> documents;
    }

    [Serializable]
    class InsertOutput {
        public string insertedId;
    }

    #endregion
}
