using System;

[Serializable]
public class RoomData : RoomDataInput {
    public string _id;
}

[Serializable]
public class RoomDataInput {
    public string name;
    public string ip;
    public string localIp;
    public string user;
    public string date;
    public string code;
    public string password;
}