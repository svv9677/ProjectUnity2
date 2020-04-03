using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum ePuzzleState {
	E_PS_SELECT_PLAYERS = 0,
    E_PS_DISTRIBUTE_CARDS,
    E_PS_PLAYER_TURN,


	E_PS_GAME_RESULTS
}
    

public class Puzzle : Mode {

    public ePuzzleState PuzzleState;
    private int NumPlayers;
    private int TurnCount;
    private int CardsToDistribute;

    public List<GamePlayer> Players;
    public List<Card> DrawPile;
    public List<Card> UsedPile;

    public int LastDroppedCardCount = 0;
    public PuzzleUI MyPuzzleUI;

	public override string ToString ()
	{
        return string.Format ("State: {0}, Turn: {1}, DrawPile: {2}, UsedPile: {3}", PuzzleState, TurnCount, DrawPile?.Count, UsedPile?.Count);
	}

	protected void SetVisible(bool hideFlags)
	{
		this.gameObject.SetActive (hideFlags);
	}

    public GameObject GetAIParentForIndex(int index)
    {
        if (index == 1)
            return MyPuzzleUI.Player1Parent;
        if (index == 2)
            return MyPuzzleUI.Player2Parent;
        if (index == 3)
            return MyPuzzleUI.Player3Parent;

        return null;
    }

    public void OnLeastCount(int playerIndex)
    {
        List<Card> Cards = Players[playerIndex].Cards;

        int total = 0;
        foreach (Card card in Cards)
        {
            int val = (int)card.mNumber;
            if (val > 10)
                val = 10;
            total += val;
        }
        string text = "Player " + playerIndex.ToString() + " - LEAST COUNT!! Count: " + total.ToString();
        Globals.ShowToast(text, 15, 5.0f);
        Debug.Log(text);
    }

    public override void EnterMode()
	{
        if(Players != null)
        {
            foreach (GamePlayer plyr in Players)
                plyr.Destroy();
        }
        if(UsedPile != null)
        {
            foreach (Card card in UsedPile)
                card.Destroy();
        }
        if(DrawPile != null)
        {
            foreach (Card card in DrawPile)
                card.Destroy();
        }

        this.PuzzleState = ePuzzleState.E_PS_SELECT_PLAYERS;
        this.MyPuzzleUI = this.gameObject.GetComponent<PuzzleUI>();
		this.SetVisible (true);

        base.EnterMode();
	}

	public override void ExitMode()
	{
        this.SetVisible (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		switch(this.PuzzleState)
		{
        case ePuzzleState.E_PS_SELECT_PLAYERS:
            // For now default to globals' min player count
            this.NumPlayers = Globals.gMinimumPlayers;
            // 7 cards per player as per rules
            this.CardsToDistribute = Globals.gCardsToDistribute;

            this.Players = new List<GamePlayer>();
            for(int i=0; i< this.NumPlayers; i++)
            {
                GameObject obj = new GameObject();
                //TODO: Get the player indices from match making and instantiate appropriate players
                // InputPlayer, AIPlayer or RemotePlayer
                if (i == 0)
                {
                    InputPlayer plyrInst = obj.AddComponent<InputPlayer>();
                    plyrInst.PlayerIndex = i;
                    this.Players.Add(plyrInst);
                }
                else
                {
                    AIPlayer plyrInst = obj.AddComponent<AIPlayer>();
                    plyrInst.PlayerIndex = i;
                    this.Players.Add(plyrInst);
                }
            }

            this.TurnCount = 0;
            this.PuzzleState = ePuzzleState.E_PS_DISTRIBUTE_CARDS;
            break;
        case ePuzzleState.E_PS_DISTRIBUTE_CARDS:
            // shuffle the deck first!
            DeckManager.Instance.mDeck.Shuffle();

            // Give 'n' cards to each player
            for (int i = 0; i < this.NumPlayers; i++)
            {
                this.Players[i].SetCards(DeckManager.Instance.mDeck.GetRange(0, this.CardsToDistribute));
                DeckManager.Instance.mDeck.RemoveRange(0, this.CardsToDistribute);
            }

            this.DrawPile = new List<Card>();
            this.UsedPile = new List<Card>();

            // facing card
            this.UsedPile.AddRange(DeckManager.Instance.mDeck.GetRange(0, 1));
            DeckManager.Instance.mDeck.RemoveRange(0, 1);

            int cnt = DeckManager.Instance.mDeck.Count;
            this.DrawPile.AddRange(DeckManager.Instance.mDeck.GetRange(0, cnt));
            DeckManager.Instance.mDeck.RemoveRange(0, cnt);

            MyPuzzleUI.UpdateDistributionDisplays(this, true);

            this.PuzzleState = ePuzzleState.E_PS_PLAYER_TURN;
            break;
        case ePuzzleState.E_PS_PLAYER_TURN:
            int plyrIndex = this.TurnCount % this.NumPlayers;

            ProcessTurnForAll(plyrIndex);
            break;
		default:
			break;
		}
	}

    void ProcessTurnForAll(int playerIndex)
    {
        for(int i=0; i<Players.Count; i++)
            Players[i].ProcessTurn(playerIndex);
    }

    public void IncrementTurn() 
    {
        this.TurnCount ++;
    }

}
