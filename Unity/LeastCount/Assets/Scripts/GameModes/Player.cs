using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerIndex;

    protected bool MyTurn = false;

    public List<Card> Cards;

    public Player ()
    {
        Cards = new List<Card>();
    }

    public void Destroy()
    {
        foreach (Card card in Cards)
            card.Destroy();

        this.OnDisable();
    }

    public virtual void OnDisable()
    {
    }

    public virtual void ProcessTurn(int playerIndex)
    {
    }

}

