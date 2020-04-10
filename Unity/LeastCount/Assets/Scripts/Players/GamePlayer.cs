using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    public int PlayerIndex;
    public int ActorIndex;
    public string NickName;

    protected bool MyTurn = false;

    public List<Card> Cards;

    public void Start()
    {
        Cards = new List<Card>();
    }

    public void Destroy()
    {
        foreach (Card card in Cards)
            card.Destroy();

        this.OnDisabled();
    }

    public virtual void OnDisabled()
    {
    }

    public virtual void ProcessTurn(int playerIndex)
    {
    }

    public virtual void SetCards(List<Card> cards)
    {
    }

}

