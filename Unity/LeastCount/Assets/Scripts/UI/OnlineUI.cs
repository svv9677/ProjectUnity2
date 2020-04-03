using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class OnlineUI : MonoBehaviour
{
    public GameObject scrollContent;
    public GameObject onlineItemPrefab;
    public Text onlineStatusText;
    public InputField roomNameInput;

    private Dictionary<string, GameObject> roomListEntries;

    public void OnInit()
    {
        roomListEntries = new Dictionary<string, GameObject>();

        OnlineManager.Instance.SetOnConnectedCB(OnConnectedCB);
        OnlineManager.Instance.SetRoomsCB(GetRoomsCB);
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
        }
    }


}
