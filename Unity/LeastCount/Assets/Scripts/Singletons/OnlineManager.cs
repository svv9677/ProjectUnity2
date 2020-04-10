using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using System;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
    private Action<bool, Player> GetPlayersCB;
    private Action<Player, string, bool> PlayerPropertiesCB;

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

    public void SetPlayersCB(Action<bool, Player> callback)
    {
        GetPlayersCB = callback;
    }

    public void SetPlayerPropertiesCB(Action<Player, string, bool> callback)
    {
        PlayerPropertiesCB = callback;
    }

    public bool IsConnected()
    {
        return PhotonNetwork.IsConnected;
    }

    public bool IsOnlineGame()
    {
        return PhotonNetwork.CurrentRoom != null;
    }

    public bool IsMaster()
    {
        return PhotonNetwork.IsMasterClient;
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

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
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
        if(GetPlayersCB != null)
        {
            GetPlayersCB(true, newPlayer);
        }
        else
            Debug.Log("OnPlayerEnteredRoom");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (GetPlayersCB != null)
        {
            GetPlayersCB(false, otherPlayer);
        }
        else
            Debug.Log("OnPlayerLeftRoom");
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log("OnMasterClientSwitched");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if(PlayerPropertiesCB != null)
        {
            string ready = "";
            object isPlayerReady;
            if (changedProps.TryGetValue(Globals.PLAYER_READY, out isPlayerReady))
            {
                ready = ((bool)isPlayerReady) ? "Ready!" : "";
            }
            bool started = false;
            object hostStarted;
            if (changedProps.TryGetValue(Globals.HOST_START, out hostStarted))
            {
                started = ((bool)hostStarted);
            }
            PlayerPropertiesCB(targetPlayer, ready, started);
        }

    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////
    #region ONLINE NETWORKING

    public void NetworkMessage(eMessage message, string param, RpcTarget target = RpcTarget.All)
    {
        if (!IsOnlineGame())
            return;

        this.photonView.RPC("OnNetworkMessage", target, message, param);
    }

    [PunRPC]
    public void OnNetworkMessage(eMessage message, string param, PhotonMessageInfo info)
    {
        switch(message)
        {
            case eMessage.E_M_SHUFFLED_DECK:
                {
                    // Load in-coming deck 
                    GameMode.Instance.puzzle.LoadDeckFromOnine(param);
                }
                break;
            case eMessage.E_M_PLAYER_ORDER:
                {
                    // Initialize in-coming players
                    GameMode.Instance.puzzle.InitPlayersFromOnline(param);
                }
                break;
            default:
                Debug.Log(String.Format("NETWORKMESSAGE: {0} sent {1} : {2}", info.Sender.NickName, message, param));
                break;
        }
    }

    #endregion
}


public enum eMessage
{
    E_M_SHUFFLED_DECK,
    E_M_PLAYER_ORDER,
}

