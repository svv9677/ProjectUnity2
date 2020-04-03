using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnlineItem : MonoBehaviour
{
    public Text RoomNameText;
    public Text OwnerNameText;
    public Text CountText;

    public OnlineUI MyOnlineUI;

    public void OnClickJoinRoom()
    {
        // Join the room with name RoomNameText.text
        MyOnlineUI.OnClickJoinRoom(RoomNameText.text);
    }
}
