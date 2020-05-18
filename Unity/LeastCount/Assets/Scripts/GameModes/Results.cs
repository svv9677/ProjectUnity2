﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Results : Mode
{
    [HideInInspector]
    public ResultsUI MyResultsUI;

    private bool readyToCheck = false;

    public override void EnterMode()
    {
        Hashtable props = new Hashtable() { { Globals.ROUND_READY, false } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        MyResultsUI = this.gameObject.GetComponent<ResultsUI>();
        MyResultsUI.OnInit();

        readyToCheck = false;

        base.EnterMode();
    }

    public override void ExitMode()
    {
        base.ExitMode();
    }

    public void Update()
    {
        if(readyToCheck)
        {
            int count = 0;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue(Globals.ROUND_READY, out isPlayerReady))
                {
                    if ((bool)isPlayerReady)
                        count++;
                }
            }

            if (count == Globals.gMinimumPlayers)
            {
                readyToCheck = false;
                GameMode.Instance.SetMode(eMode.E_M_PUZZLE);
            }
            else
                MyResultsUI.Message.text = "Waiting for " + (Globals.gMinimumPlayers - count).ToString() + " players...";
        }
    }

    public void CheckAndStartNextRound()
    {
        Hashtable props = new Hashtable() { { Globals.ROUND_READY, true } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        readyToCheck = true;
    }
}
