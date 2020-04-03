using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;

public enum eConnectionState
{
    E_CS_CONNECTED,
    E_CS_DUPLICATE_NAME,
    E_CS_NETWORK_ERROR,
}

public class OnlineManager : OnlineSingleton<OnlineManager>
{
    public string ConnectionStatus = "";

    // All callbacks for the calling functions!
    private Action<bool> ConnectCB;
    private Action<bool, short, string> CreateRoomCB;
    private Action<bool, short, string> JoinRoomCB;
    private Action<bool, List<RoomInfo>> GetRoomsCB;

    // Use this for initialization
    public void Load()
    {
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    public void Update()
    {
        ConnectionStatus = "Online Status: " + PhotonNetwork.NetworkClientState;
    }

    public void SetOnConnectedCB(Action<bool> callback)
    {
        ConnectCB = callback;
    }

    public void SetRoomsCB(Action<bool, List<RoomInfo>> callback)
    {
        GetRoomsCB = callback;
    }

    public eConnectionState ConnectAs(string name)
    {
        PhotonNetwork.LocalPlayer.NickName = name;
        bool connected = PhotonNetwork.ConnectUsingSettings();

        if (connected)
            return eConnectionState.E_CS_CONNECTED;
        else
            return eConnectionState.E_CS_NETWORK_ERROR;
    }

    public bool CreateRoom(string roomName, Action<bool, short, string> callback)
    {
        byte maxPlayers = (byte)4;
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        // This callback is to be called from OnCreateRoomFailed, OnCreatedRoom & OnJoinedRoom
        CreateRoomCB = callback;
        return PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public bool JoinRoom(string roomName, Action<bool, short, string> callback)
    {
        byte maxPlayers = (byte)4;
        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };
        // This callback is to be called from OnJoinedRoom & OnJoinedRoomFailed
        JoinRoomCB = callback;
        return PhotonNetwork.JoinRoom(roomName);
    }


    ////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////
    #region PUN CALLBACKS

    public override void OnConnectedToMaster()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        if (ConnectCB != null)
        {
            ConnectCB(true);
            ConnectCB = null;
        }
        else
            Debug.Log("OnConnectedToMaster");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        if (GetRoomsCB != null)
            GetRoomsCB(true, roomList);
        else
            Debug.Log("OnRoomListUpdate");
    }

    public override void OnLeftLobby()
    {
        Debug.Log("OnLeftLobby");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(CreateRoomCB != null)
        {
            CreateRoomCB(false, returnCode, message);
            CreateRoomCB = null;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if(JoinRoomCB != null)
        {
            JoinRoomCB(false, returnCode, message);
            JoinRoomCB = null;
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
    }

    public override void OnJoinedRoom()
    {
        if(CreateRoomCB != null)
        {
            CreateRoomCB(true, 0, "");
            CreateRoomCB = null;
        }
        else if (JoinRoomCB != null)
        {
            JoinRoomCB(true, 0, "");
            JoinRoomCB = null;
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("OnLeftRoom");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("OnMasterClientSwitched");
    }

    //public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    //{
    //}

    #endregion

}
