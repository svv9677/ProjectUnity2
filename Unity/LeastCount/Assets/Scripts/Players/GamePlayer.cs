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

    public GamePlayer()
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

    public virtual void ProcessTurn(int index, bool online)
    {
    }

    public virtual void SetCards(List<Card> cards)
    {
    }

}

