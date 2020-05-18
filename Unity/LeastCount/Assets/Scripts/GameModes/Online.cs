﻿using UnityEngine;

public class Online : Mode
{
    [HideInInspector]
    public OnlineUI MyOnlineUI;

    public override void EnterMode()
    {
        MyOnlineUI = this.gameObject.GetComponent<OnlineUI>();
        if (GameMode.Instance.modeParam == "ReEnter")
            MyOnlineUI.OnReEnter();
        else
            MyOnlineUI.OnInit();

        base.EnterMode();
    }

    public override void ExitMode()
    {
        MyOnlineUI.gameObject.SetActive(false);
        base.ExitMode();
    }

    public void Update()
    {

    }
}
