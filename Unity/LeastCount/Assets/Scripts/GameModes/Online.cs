﻿using UnityEngine;

public class Online : Mode
{
    [HideInInspector]
    public OnlineUI MyOnlineUI;

    public override void EnterMode()
    {
        MyOnlineUI = this.gameObject.GetComponent<OnlineUI>();
        MyOnlineUI.OnInit();

        base.EnterMode();
    }

    public override void ExitMode()
    {

        base.ExitMode();
    }

    public void Update()
    {

    }
}
