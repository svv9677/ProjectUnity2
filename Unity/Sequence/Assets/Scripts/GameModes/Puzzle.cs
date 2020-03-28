using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ePuzzleState {
	E_PS_SELECT_NUM_PLAYERS = 0,
    E_PS_DISTRIBUTE_CARDS,
    E_PS_PLAYER_TURN,


	E_PS_GAME_RESULTS
}
    

public class Puzzle : Mode {

    public ePuzzleState PuzzleState;
    private int NumPlayers;
    private int NumTeams;
    private int TurnCount;
    private int CardsToDistribute;

    public InputPlayer MyPlayer;
    public List<AIPlayer> AIPlayers;
    public List<Card> DrawPile;
    public List<Card> UsedPile;

	public override string ToString ()
	{
        return string.Format ("State: {0}, Turn: {1}, DrawPile: {2}, UsedPile: {3}", PuzzleState, TurnCount, DrawPile.Count, UsedPile.Count);
	}

	protected void SetVisible(bool hideFlags)
	{
		this.gameObject.SetActive (hideFlags);

		//HUD.Instance.gameObject.SetActive(hideFlags);
	}
	
	public override void EnterMode()
	{
        this.PuzzleState = ePuzzleState.E_PS_SELECT_NUM_PLAYERS;
		this.SetVisible (true);
	}

	public override void ExitMode()
	{
		this.SetVisible (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
        if(this.MyPlayer != null)
            this.MyPlayer.Update();
        
		switch(this.PuzzleState)
		{
        case ePuzzleState.E_PS_SELECT_NUM_PLAYERS:
            // For now default to 2 players & 2 teams
            this.NumPlayers = 2;
            this.NumTeams = 2;
            // this gives us 7 cards per player as per rules
            this.CardsToDistribute = 7;
            this.AIPlayers = new List<AIPlayer>();
            for(int i=1; i< this.NumPlayers; i++)
            {
                AIPlayer plyrInst = new AIPlayer();
                plyrInst.TeamIndex = 1;
                plyrInst.PlayerIndex = i;
                this.AIPlayers.Add(plyrInst);
            }

            this.MyPlayer = new InputPlayer();
            this.MyPlayer.TeamIndex = 0;
            this.MyPlayer.PlayerIndex = 0;

            this.TurnCount = 0;

            this.PuzzleState = ePuzzleState.E_PS_DISTRIBUTE_CARDS;
            break;
        case ePuzzleState.E_PS_DISTRIBUTE_CARDS:
            // shuffle the deck first!
            DeckManager.Instance.mDeck.Shuffle();
            // Give 'n' cards to my player
            this.MyPlayer.SetCards(DeckManager.Instance.mDeck.GetRange(0, this.CardsToDistribute));
            DeckManager.Instance.mDeck.RemoveRange(0, this.CardsToDistribute);

            // Give 'n' cards to each AI player
            for(int i=0; i<this.NumPlayers-1; i++)
            {
                this.AIPlayers[i].Cards.AddRange(DeckManager.Instance.mDeck.GetRange(0, this.CardsToDistribute));
                DeckManager.Instance.mDeck.RemoveRange(0, this.CardsToDistribute);
            }

            this.UsedPile = new List<Card>();

            this.DrawPile = new List<Card>();
            int cnt = DeckManager.Instance.mDeck.Count;
            this.DrawPile.AddRange(DeckManager.Instance.mDeck.GetRange(0, cnt));
            DeckManager.Instance.mDeck.RemoveRange(0, cnt);

            this.PuzzleState = ePuzzleState.E_PS_PLAYER_TURN;
            break;
        case ePuzzleState.E_PS_PLAYER_TURN:
            int teamIndex = this.TurnCount % this.NumTeams;
            int plyrIndex = this.TurnCount % this.NumPlayers;

            ProcessTurnForAll(teamIndex, plyrIndex);
            break;
		default:
			break;
		}
	}

    void ProcessTurnForAll(int teamIndex, int playerIndex)
    {
        MyPlayer.ProcessTurn(teamIndex, playerIndex);

        for(int i=0; i<AIPlayers.Count; i++)
            AIPlayers[i].ProcessTurn(teamIndex, playerIndex);
    }

    public void IncrementTurn() 
    {
        this.TurnCount ++;
    }
}
