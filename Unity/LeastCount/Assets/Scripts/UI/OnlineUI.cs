using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class OnlineUI : MonoBehaviour
{
    public GameObject LobbyParent;
    public GameObject RoomParent;
    public Text onlineStatusText;

    [Header("Lobby")]
    public GameObject scrollContent;
    public GameObject onlineItemPrefab;
    public InputField roomNameInput;

    [Header("Room")]
    public Text roomNameLabel;
    public Text player1Name;
    public Text player1Status;
    public Text player2Name;
    public Text player2Status;
    public Text player3Name;
    public Text player3Status;
    public Text player4Name;
    public Text player4Status;
    public GameObject startButtonObj;

    private Dictionary<string, GameObject> roomListEntries;
    private Dictionary<int, int> playerListEntries;

    private bool isPlayerReady;

    public void OnInit()
    {
        roomListEntries = new Dictionary<string, GameObject>();

        OnlineManager.Instance.SetOnConnectedCB(OnConnectedCB);
        OnlineManager.Instance.SetRoomsCB(GetRoomsCB);
        LobbyParent.SetActive(true);
        RoomParent.SetActive(false);
        isPlayerReady = false;
    }

    public void Update()
    {
        onlineStatusText.text = OnlineManager.Instance.ConnectionStatus;
    }

    public void OnConnectedCB(bool success)
    {
        if(!success)
        {
            Globals.ShowToast("Failed to coonect to Servers");
            return;
        }
    }

    private void GetRoomsCB(bool success, List<RoomInfo> roomList)
    {
        foreach(GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }
        roomListEntries.Clear();

        RectTransform pTrans = onlineItemPrefab.transform as RectTransform;
        onlineItemPrefab.SetActive(false);
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible)
                continue;
            GameObject obj = GameObject.Instantiate(onlineItemPrefab) as GameObject;
            obj.transform.SetParent(scrollContent.transform, false);
            RectTransform trans = obj.transform as RectTransform;
            trans.anchoredPosition = pTrans.anchoredPosition;
            trans.anchorMin = pTrans.anchorMin;
            trans.anchorMax = pTrans.anchorMax;
            trans.localScale = Vector3.one;
            obj.SetActive(true);
            OnlineItem item = obj.GetComponent<OnlineItem>();
            item.RoomNameText.text = info.Name;
            item.OwnerNameText.text = info.masterClientId.ToString();
            item.CountText.text = info.PlayerCount.ToString();
            item.MyOnlineUI = this;

            roomListEntries.Add(info.Name, obj);
        }
    }

    public void OnClickCreateRoom()
    {
        string finalName = roomNameInput.text.Trim();
        if (finalName == "")
        {
            Globals.ShowToast("Please enter a valid name");
            return;
        }
        // Create the room with name roomNameInput.text
        //  Callbacks will be handled in OnlineManager and info passed down to the UI for further action
        if(!OnlineManager.Instance.CreateRoom(finalName, OnClickCreateRoomCB))
        {
            Globals.ShowToast("Failed to create room");
        }
    }

    private void OnClickCreateRoomCB(bool success, short code, string info)
    {
        Debug.Log("OnClickCreateRoomCB: " + success + " : " + code + " : " + info);
        if (!success)
            Globals.ShowToast("Failed to create room\nCode: " + code.ToString() + "\nMessage: " + info);
        else
        {
            // Move away from online lobby to online game room UI
            SwitchToRoom();
        }
    }

    public void OnClickSearchAndJoinRoom()
    {
        OnClickJoinRoom(roomNameInput.text);
    }

    public void OnClickJoinRoom(string roomName)
    { 
        string finalName = roomName.Trim();
        if (finalName == "")
        {
            Globals.ShowToast("Please enter a valid name");
            return;
        }
        // Join the room with name roomNameInput.text
        if (!OnlineManager.Instance.JoinRoom(finalName, OnClickJoinRoomCB))
        {
            Globals.ShowToast("Failed to join room");
        }
    }

    private void OnClickJoinRoomCB(bool success, short code, string info)
    {
        Debug.Log("OnClickJoinRoomCB: " + success + " : " + code + " : " + info);
        if (!success)
            Globals.ShowToast("Failed to join room\nCode: " + code.ToString() + "\nMessage: " + info);
        else
        {
            // Move away from online lobby to online game room UI
            SwitchToRoom();
        }
    }

    private void SwitchToRoom()
    {
        LobbyParent.SetActive(false);
        RoomParent.SetActive(true);

        OnlineManager.Instance.SetPlayersCB(GetPlayersCB);
        OnlineManager.Instance.SetPlayerPropertiesCB(PlayerPropertiesCB);

        roomNameLabel.text = PhotonNetwork.CurrentRoom.Name;

        PopulatePlayers();
    }

    private void PopulatePlayers()
    { 
        playerListEntries = new Dictionary<int, int>();
        int i = 0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            string ready = "";
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(Globals.PLAYER_READY, out isPlayerReady))
                ready = ((bool)isPlayerReady)? "Ready!" : "";

            AssignPlayer(i, p.NickName, ready);

            playerListEntries.Add(p.ActorNumber, i);
            i++;
        }
        for(int j=i; j<4; j++)
            AssignPlayer(j, "", "");

        startButtonObj.SetActive(CheckPlayersReady());
    }

    private void PlayerPropertiesCB(Player player, string ready, bool started)
    {
        if(!started)
        {
            int outVal;
            if (playerListEntries.TryGetValue(player.ActorNumber, out outVal))
            {
                AssignPlayer(outVal, player.NickName, ready);
            }
        }
        else
        {
            // Start game!
            CompilePlayersAndStartGame();
        }
    }

    private void GetPlayersCB(bool joined, Player player)
    {
        if(joined)
        {
            // Add the new entry
            string ready = "";
            object isPlayerReady;
            if (player.CustomProperties.TryGetValue(Globals.PLAYER_READY, out isPlayerReady))
                ready = ((bool)isPlayerReady) ? "Ready!" : "";

            int i = playerListEntries.Count;
            AssignPlayer(i, player.NickName, ready);
            playerListEntries.Add(player.ActorNumber, i);
        }
        else
        {
            // Remove this entry
            PopulatePlayers();
        }

        // Re-check if we are supposed to be the master client to kick off the game!
        startButtonObj.gameObject.SetActive(CheckPlayersReady());
    }

    private void AssignPlayer(int i, string nickName, string ready)
    {
        switch (i)
        {
            case 0:
                {
                    player1Name.text = nickName;
                    player1Status.text = ready;
                }
                break;
            case 1:
                {
                    player2Name.text = nickName;
                    player2Status.text = ready;
                }
                break;
            case 2:
                {
                    player3Name.text = nickName;
                    player3Status.text = ready;
                }
                break;
            case 3:
                {
                    player4Name.text = nickName;
                    player4Status.text = ready;
                }
                break;
        }
    }

    private bool CheckPlayersReady()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            if (p.CustomProperties.TryGetValue(Globals.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void OnClickReady()
    {
        isPlayerReady = !isPlayerReady;
        Hashtable props = new Hashtable() { { Globals.PLAYER_READY, isPlayerReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        if (PhotonNetwork.IsMasterClient)
        {
            // Re-check if we are supposed to be the master client to kick off the game!
            startButtonObj.gameObject.SetActive(CheckPlayersReady());
        }
    }

    public void OnClickStart()
    {
        // We (host) set our custom property, so that all joiners will start as well
        Hashtable props = new Hashtable() { { Globals.HOST_START, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void CompilePlayersAndStartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        GameMode.Instance.SetMode(eMode.E_M_PUZZLE);
    }



}
