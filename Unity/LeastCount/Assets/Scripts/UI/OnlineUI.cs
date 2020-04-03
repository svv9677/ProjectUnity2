using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineUI : MonoBehaviour
{
    public GameObject scrollContent;
    public GameObject onlineItemPrefab;
    public Text onlineStatusText;
    public InputField roomNameInput;

    public void OnInit()
    {

    }

    public void Update()
    {
        onlineStatusText.text = OnlineManager.Instance.ConnectionStatus;
    }

    public void OnClickCreateRoom()
    {
        string finalName = roomNameInput.text.Trim();
        if (finalName == "")
        {
            Globals.ShowToast("Please enter a valid name", 30);
            return;
        }
        // Create the room with name roomNameInput.text
        //  Callbacks will be handled in OnlineManager and info passed down to the UI for further action
        if(!OnlineManager.Instance.CreateRoom(finalName, OnClickCreateRoomCB))
        {
            Globals.ShowToast("Failed to create room", 30);
        }
    }

    private void OnClickCreateRoomCB(bool success, short code, string info)
    {
        Debug.Log("OnClickCreateRoomCB: " + success + " : " + code + " : " + info);
        if (!success)
            Globals.ShowToast("Failed to create room<br>Code: " + code.ToString() + "<br>Message: " + info, 30);
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
            Globals.ShowToast("Please enter a valid name", 30);
            return;
        }
        // Join the room with name roomNameInput.text
        if (!OnlineManager.Instance.JoinRoom(finalName, OnClickJoinRoomCB))
        {
            Globals.ShowToast("Failed to create room", 30);
        }
    }

    private void OnClickJoinRoomCB(bool success, short code, string info)
    {
        Debug.Log("OnClickJoinRoomCB: " + success + " : " + code + " : " + info);
        if (!success)
            Globals.ShowToast("Failed to join room<br>Code: " + code.ToString() + "<br>Message: " + info, 30);
        else
        {
            // Move away from online lobby to online game room UI
        }
    }


}
