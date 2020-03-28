using System;
using System.Collections;
using System.Collections.Generic;

public class Player 
{
    public int TeamIndex;
    public int PlayerIndex;

    public List<Card> Cards;
    public List<Chip> Chips;

    public Player ()
    {
        Cards = new List<Card>();
        Chips = new List<Chip>();
    }

    public virtual void ProcessTurn(int teamIndex, int playerIndex)
    {
    }

}

