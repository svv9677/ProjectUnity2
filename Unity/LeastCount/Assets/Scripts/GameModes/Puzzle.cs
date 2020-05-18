using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public enum ePuzzleState {
    E_PS_SHUFFLE_CARDS,
    E_PS_SELECT_PLAYERS,
    E_PS_DISTRIBUTE_CARDS,
    E_PS_PLAYER_TURN,


	E_PS_GAME_RESULTS
}


public class Puzzle : Mode {

    public ePuzzleState PuzzleState;
    public int NumPlayers;
    private int TurnCount;
    private int CardsToDistribute;
    private int[] ActorIndices = {-1, -1, -1, -1};

    public List<GamePlayer> Players;
    public List<Card> DrawPile;
    public List<Card> UsedPile;

    public int LastDroppedCardCount = 0;
    public PuzzleUI MyPuzzleUI;

	public override string ToString ()
	{
        return string.Format ("State: {0}, Turn: {1}, DrawPile: {2}, UsedPile: {3}", PuzzleState, TurnCount, DrawPile?.Count, UsedPile?.Count);
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

    public void LeastCount(GamePlayer player)
    {
        if (OnlineManager.Instance.IsOnlineGame())
            OnlineManager.Instance.NetworkMessage(eMessage.E_M_PLAYER_LEAST_COUNT, "", player, Photon.Pun.RpcTarget.All);
        else
            OnLeastCount(player);
    }

    public void OnLeastCountClicked()
    {
        GamePlayer player = Players[0];
        LeastCount(player);
    }

    public void OnLeastCount(GamePlayer player)
    {
        List<Card> Cards = player.Cards;

        int total = 0;
        foreach (Card card in Cards)
        {
            int val = (int)card.mNumber;
            if (val > 10)
                val = 10;
            total += val;
        }
        string text = "LEAST COUNT called by " + player.NickName + "!! They scored " + total.ToString() + "\n";

        int[] totals = new int[NumPlayers];
        int[] scores = new int[NumPlayers];
        int highest_total = 0;
        int lowest_total = 1000;
        int lowest_index = -1;

        // Now compare other players' scores and determine the winner
        for(int i=0; i<NumPlayers; i++)
        {
            // ignore current player
            if (i == player.PlayerIndex)
                continue;

            List<Card> cards = Players[i].Cards;

            int curr_total = 0;
            foreach (Card card in cards)
            {
                int val = (int)card.mNumber;
                if (val > 10)
                    val = 10;
                curr_total += val;
            }

            totals[i] = curr_total;
            if (curr_total >= highest_total)
                highest_total = curr_total;
            if (curr_total < lowest_total)
            {
                lowest_total = curr_total;
                lowest_index = i;
            }
        }

        // if the lowest total from other players is lower than LC claim
        if(lowest_total <= total)
        {
            for(int i=0; i<NumPlayers; i++)
            {
                // LC claim player gets highest total
                if (i == player.PlayerIndex)
                    scores[i] = highest_total;
                // Others get their total - lowest total
                else
                    scores[i] = totals[i] - lowest_total;
            }
            text += "However, " + Players[lowest_index].NickName + " won with " + lowest_total.ToString() + " score!";
        }
        // if not, LC claim  is winner
        else
        {
            for (int i = 0; i < NumPlayers; i++)
            {
                // LC claim player gets 0
                if (i == player.PlayerIndex)
                    scores[i] = 0;
                // Others get their total - player's total
                else
                    scores[i] = totals[i] - total;
            }
            text += "and is the winner!";
        }

        totals[player.PlayerIndex] = total;

        Globals.ShowToast(text, 15, 5.0f);
        Debug.Log(text);

        // Add the scores to Scoring Manager
        ScoringManager.Instance.AddScores(scores, totals);

        // Kick off Results UI
        GameMode.Instance.SetMode(eMode.E_M_RESULTS);
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

        // For now default to globals' min player count
        this.NumPlayers = Globals.gMinimumPlayers;
        // 7 cards per player as per rules
        this.CardsToDistribute = Globals.gCardsToDistribute;

        this.Players = new List<GamePlayer>();

        this.PuzzleState = ePuzzleState.E_PS_SHUFFLE_CARDS;
        this.MyPuzzleUI = this.gameObject.GetComponent<PuzzleUI>();

        DeckManager.Instance.Load();

        base.EnterMode();
	}

	public override void ExitMode()
	{
	}
	
	// Update is called once per frame
	void Update () 
	{
        bool online = OnlineManager.Instance.IsOnlineGame();
        bool master = OnlineManager.Instance.IsMaster();

        switch (this.PuzzleState)
		{
            case ePuzzleState.E_PS_SHUFFLE_CARDS:
                {
                    // If not an online game, let's shuffle the cards
                    // else, if we are host, shuffle AND let everyone know the new deck
                    if (!online || (online && master))
                    {
                        // shuffle the deck first!
                        DeckManager.Instance.mDeck.Shuffle();
                        //Debug.Log(DeckManager.Instance.DeckAsString());
                        // set to next state
                        this.PuzzleState = ePuzzleState.E_PS_SELECT_PLAYERS;

                        OnlineManager.Instance.NetworkMessage(eMessage.E_M_SHUFFLED_DECK, DeckManager.Instance.DeckAsString());
                    }
                    // else, we need to wait to hear about it from the OnlineManager
                }
                break;
            case ePuzzleState.E_PS_SELECT_PLAYERS:
                {
                    if(!online)
                    {
                        for (int i = 0; i < this.NumPlayers; i++)
                        {
                            GameObject obj = new GameObject();
                            // input player is at base 0
                            if (i == 0)
                            {
                                InputPlayer plyrInst = obj.AddComponent<InputPlayer>();
                                plyrInst.PlayerIndex = i;
                                plyrInst.NickName = User.Instance.Name;
                                this.Players.Add(plyrInst);
                            }
                            // rest take bases 1-3
                            else
                            {
                                AIPlayer plyrInst = obj.AddComponent<AIPlayer>();
                                plyrInst.PlayerIndex = i;
                                plyrInst.NickName = Globals.AI_PLAYER_NAMES[i - 1];
                                this.Players.Add(plyrInst);
                            }
                        }
                        this.TurnCount = 0;
                        this.PuzzleState = ePuzzleState.E_PS_DISTRIBUTE_CARDS;
                    }
                    if(online && master)
                    {
                        int count = 0;
                        GameObject obj;
                        foreach (Player p in PhotonNetwork.PlayerList)
                        {
                            if (p.IsLocal)
                            {
                                obj = new GameObject();
                                // Set local player as index 0
                                InputPlayer plyrInst = obj.AddComponent<InputPlayer>();
                                plyrInst.PlayerIndex = count;
                                plyrInst.ActorIndex = p.ActorNumber;
                                plyrInst.NickName = p.NickName;
                                this.Players.Add(plyrInst);
                                this.ActorIndices[count] = plyrInst.ActorIndex;
                                count++;
                                break;
                            }
                        }
                        // Now instantiate other online players
                        foreach (Player p in PhotonNetwork.PlayerList)
                        {
                            if (!p.IsLocal)
                            {
                                obj = new GameObject();
                                // Set local player as index 0
                                OnlinePlayer plyrInst = obj.AddComponent<OnlinePlayer>();
                                plyrInst.PlayerIndex = count;
                                plyrInst.ActorIndex = p.ActorNumber;
                                plyrInst.NickName = p.NickName;
                                this.Players.Add(plyrInst);
                                this.ActorIndices[count] = plyrInst.ActorIndex;
                                count++;
                            }
                        }
                        // Now instantiate and fill remaining spots with AI Players
                        int ai_index = 0;
                        for (int i = count; i < this.NumPlayers; i++)
                        {
                            obj = new GameObject();
                            AIPlayer plyrInst = obj.AddComponent<AIPlayer>();
                            plyrInst.PlayerIndex = i;
                            plyrInst.ActorIndex = Globals.AI_PLAYER_INDEX_MULTIPLIER * (ai_index+1);
                            plyrInst.NickName = Globals.AI_PLAYER_NAMES[ai_index];
                            this.Players.Add(plyrInst);
                            this.ActorIndices[i] = plyrInst.ActorIndex;
                            ai_index++;
                            count++;
                        }

                        // Send the sequence of players to everyone, so they get same set of cards after shuffle
                        OnlineManager.Instance.NetworkMessage(eMessage.E_M_PLAYER_ORDER, MiniJSON.Json.Serialize(this.ActorIndices));
                        this.TurnCount = 0;
                        this.PuzzleState = ePuzzleState.E_PS_DISTRIBUTE_CARDS;
                    }
                }
                break;
            case ePuzzleState.E_PS_DISTRIBUTE_CARDS:
                {
                    if ((online && master) || !online)
                    {
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

                        MyPuzzleUI.UpdatePlayerNames(this.Players[1].NickName, this.Players[2].NickName, this.Players[3].NickName);
                        MyPuzzleUI.UpdateDistributionDisplays(this, true);

                        this.PuzzleState = ePuzzleState.E_PS_PLAYER_TURN;
                    }
                }
                break;
            case ePuzzleState.E_PS_PLAYER_TURN:
                {
                    if(!online)
                    {
                        int playerIndex = this.TurnCount % this.NumPlayers;
                        ProcessTurnForAll(playerIndex);
                    }
                    else
                    {
                        int actorIndex = this.ActorIndices[this.TurnCount % this.NumPlayers];
                        ProcessTurnForAll(actorIndex, true);
                    }
                }
                break;
    		default:
	    		break;
		}
	}

    public void InitPlayersFromOnline(string param)
    {
        int[] indices = { -1, -1, -1, -1 };
        int j = 0;
        List<object> indicesobjs = (List<object>)MiniJSON.Json.Deserialize(param);
        foreach(object idx in indicesobjs)
        {
            indices[j] = System.Convert.ToInt32(idx);
            j++;
        }
        this.ActorIndices[0] = indices[0];
        this.ActorIndices[1] = indices[1];
        this.ActorIndices[2] = indices[2];
        this.ActorIndices[3] = indices[3];

        int count = 0;
        int indices_idx = 0;
        GameObject obj;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.IsLocal)
            {
                obj = new GameObject();
                // Set local player first
                InputPlayer plyrInst = obj.AddComponent<InputPlayer>();
                plyrInst.PlayerIndex = count;
                plyrInst.ActorIndex = p.ActorNumber;
                plyrInst.NickName = p.NickName;
                this.Players.Add(plyrInst);
                count++;
                for(int k=0; k<4; k++)
                {
                    if(indices[k] == p.ActorNumber)
                    {
                        indices_idx = k;
                        break;
                    }
                }
                break;
            }
        }

        // Setup remaining 3 players
        for(int p=0; p<3; p++)
        {
            // get the next index from indices
            indices_idx++;
            indices_idx = indices_idx % 4;

            // Now instantiate other online players
            bool found = false;
            foreach (Player pl in PhotonNetwork.PlayerList)
            {
                if (!pl.IsLocal && pl.ActorNumber == indices[indices_idx])
                {
                    obj = new GameObject();
                    // Set local player as index 0
                    OnlinePlayer plyrInst1 = obj.AddComponent<OnlinePlayer>();
                    plyrInst1.PlayerIndex = count;
                    plyrInst1.ActorIndex = pl.ActorNumber;
                    plyrInst1.NickName = pl.NickName;
                    this.Players.Add(plyrInst1);
                    count++;
                    found = true;
                    break;
                }
            }
            if (found)
                continue;

            // If we have come here, our next player is an AI player on master
            // Now instantiate them as Online Player
            obj = new GameObject();
            int ai_index = indices[indices_idx] / Globals.AI_PLAYER_INDEX_MULTIPLIER;
            ai_index--;
            OnlinePlayer plyrInst2 = obj.AddComponent<OnlinePlayer>();
            plyrInst2.PlayerIndex = count;
            plyrInst2.ActorIndex = indices[indices_idx];
            plyrInst2.NickName = Globals.AI_PLAYER_NAMES[ai_index];
            this.Players.Add(plyrInst2);
            count++;
        }

        // Give 'n' cards to each player
        for (int i = 0; i < 4; i++)
        {
            // Find the player with this actor index, indices[i]
            foreach(GamePlayer player in this.Players)
            {
                if(player.ActorIndex == indices[i])
                {
                    //Debug.Log(string.Format("Setting cards for player: {0} - {1}", player.ActorIndex, player.NickName));
                    player.SetCards(DeckManager.Instance.mDeck.GetRange(0, this.CardsToDistribute));
                    DeckManager.Instance.mDeck.RemoveRange(0, this.CardsToDistribute);
                    break;
                }
            }
        }

        this.TurnCount = 0;
        this.DrawPile = new List<Card>();
        this.UsedPile = new List<Card>();

        // facing card
        this.UsedPile.AddRange(DeckManager.Instance.mDeck.GetRange(0, 1));
        DeckManager.Instance.mDeck.RemoveRange(0, 1);

        int cnt = DeckManager.Instance.mDeck.Count;
        this.DrawPile.AddRange(DeckManager.Instance.mDeck.GetRange(0, cnt));
        DeckManager.Instance.mDeck.RemoveRange(0, cnt);

        MyPuzzleUI.UpdatePlayerNames(this.Players[1].NickName, this.Players[2].NickName, this.Players[3].NickName);
        MyPuzzleUI.UpdateDistributionDisplays(this, true);

        this.PuzzleState = ePuzzleState.E_PS_PLAYER_TURN;
    }

    public void LoadDeckFromOnine(string param)
    {
        // Load the shuffled deck
        DeckManager.Instance.DeckFromString(param);
        //Debug.Log(DeckManager.Instance.DeckAsString());
        // Set puzzle state to distribute cards and move ahead!
        GameMode.Instance.puzzle.PuzzleState = ePuzzleState.E_PS_SELECT_PLAYERS;
    }

    public void HandleOnlineAction(string param, GamePlayer player)
    {
        if (!this.isActiveAndEnabled)
            return;

        OnlinePlayer oplyr = null;
        // Trace the player
        for (int i = 0; i < this.NumPlayers; i++)
        {
            if (this.Players[i].ActorIndex == player.ActorIndex)
            {
                oplyr = (OnlinePlayer)this.Players[i];
                break;
            }
        }
        Dictionary<string, object> paramObj = (Dictionary<string, object>)MiniJSON.Json.Deserialize(param);
        foreach(KeyValuePair<string, object> entry in paramObj)
        {
            string val = (string)entry.Value;
            switch (entry.Key)
            {
                case "incr-turn":
					{
                        StartCoroutine(oplyr.PerformDisplay());
					}
                    break;
                // Add the card to used pile and remove from Cards
                case "cards-rem&used-add-0":
                case "cards-rem&used-add-1":
                case "cards-rem&used-add-2":
                case "cards-rem&used-add-3":
                case "cards-rem&used-add-4":
                case "cards-rem&used-add-5":
                case "cards-rem&used-add-6":
                    {
                        if (oplyr != null)
						{
                            // Find the card
                            for (int i=0; i<oplyr.Cards.Count; i++)
							{
                                if(oplyr.Cards[i].OnlineHash() == val)
								{
                                    Card card = oplyr.Cards[i];
                                    oplyr.Cards.Remove(card);
                                    card.SetPrefix("__");
                                    card.mMoveDirty = true;
                                    this.UsedPile.Add(card);
                                    break;
								}
							}
						}
                    }
                    break;
                // Add the card to player's cards & Remove card from used pile
                case "used-rem&cards-add-0":
                case "used-rem&cards-add-1":
                case "used-rem&cards-add-2":
                case "used-rem&cards-add-3":
                case "used-rem&cards-add-4":
                case "used-rem&cards-add-5":
                case "used-rem&cards-add-6":
                    {
                        // Trace the card in Used Pile
                        Card card = null;
                        for (int i = this.UsedPile.Count - 1; i >= 0; i--)
                        {
                            if (this.UsedPile[i].OnlineHash() == val)
                            {
                                card = this.UsedPile[i];
                                this.UsedPile.Remove(card);
                                break;
                            }
                        }
                        if (card != null)
                        {
                            if(oplyr != null)
                            { 
                                card.SetPrefix(Globals.PLAYER_PREFIXES[oplyr.PlayerIndex]);
                                card.mMoveDirty = true;
                                oplyr.Cards.Add(card);
                            }
                        }
                    }
                    break;
                // Add the card to player's cards & Remove card from draw pile
                case "draw-rem&cards-add":
                    {
                        // Trace the card in Draw Pile
                        Card card = null;
                        for (int i = this.DrawPile.Count - 1; i >= 0; i--)
                        {
                            if (this.DrawPile[i].OnlineHash() == val)
                            {
                                card = this.DrawPile[i];
                                this.DrawPile.Remove(card);
                                break;
                            }
                        }
                        if (card != null)
                        {
                            if (oplyr != null)
                            { 
                                card.SetPrefix(Globals.PLAYER_PREFIXES[oplyr.PlayerIndex]);
                                card.mMoveDirty = true;
                                oplyr.Cards.Add(card);
                            }
                        }
                    }
                    break;
                // Set last dropped card count
                case "last-dropped-count":
					{
                        int cnt = System.Convert.ToInt32(val);

                        this.LastDroppedCardCount = cnt;
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void ProcessTurnForAll(int Index, bool online = false)
    {
        for(int i=0; i<Players.Count; i++)
            Players[i].ProcessTurn(Index, online);
    }

    public void IncrementTurn() 
    {
        this.TurnCount ++;
    }

}
