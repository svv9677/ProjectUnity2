using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AIPlayer: GamePlayer
{
    private Dictionary<eCardNumber, int> CardTallies;
    private eCardNumber HighestTalliedCardNumber;
    private int HighestTally;
    private int MatchingCount;
    private Dictionary<string, string> OnlineParam;

    public AIPlayer ()
    {
    }

    public override void OnDisabled()
    {
        this.CardTallies = null;
        this.HighestTalliedCardNumber = eCardNumber.E_CN_TOTAL;
        this.HighestTally = 0;
        this.MatchingCount = 0;
        base.OnDisabled();
    }

    public override void SetCards(List<Card> cards)
    {
        this.Cards.AddRange(cards);
        for (int i = 0; i < this.Cards.Count; i++)
        {
            this.Cards[i].InitCardUI(Globals.PLAYERCARDS_START_X[this.PlayerIndex],
                                     Globals.PLAYERCARDS_START_Y[this.PlayerIndex],
                                     Globals.PLAYER_PREFIXES[this.PlayerIndex]);
            this.Cards[i].Close();
        }
    }

    public override void ProcessTurn(int index, bool online)
    {
        if (online)
        {
            if (this.ActorIndex != index)
            {
                this.MyTurn = false;
                return;
            }
        }
        else
        {
            if (this.PlayerIndex != index)
            {
                this.MyTurn = false;
                return;
            }
        }

        if (!this.MyTurn)
        {
            this.MyTurn = true;
            StartCoroutine(CheckAndIncrementTurn());
        }
    }

    public IEnumerator CheckAndIncrementTurn()
    {
        if (this.OnlineParam == null)
            this.OnlineParam = new Dictionary<string, string>();
        else
            this.OnlineParam.Clear();

        // Check our cards for highest tally, including dupes
        yield return StartCoroutine(TallyHighestCardsToDrop());

        // Check if we have any matching cards to the used pile
        yield return StartCoroutine(FindMatchingCardsToDrop());

        // Decide whether we want to drop matching card(s) or
        //  drop highest tally, and drop them
        yield return StartCoroutine(DecideAndDropCards());

        this.OnlineParam.Add("incr-turn", "");
        OnlineManager.Instance.NetworkMessage(eMessage.E_M_PLAYER_ACTION, MiniJSON.Json.Serialize(this.OnlineParam), this);

        yield return new WaitForSeconds(1);

        GameMode.Instance.puzzle.MyPuzzleUI.UpdateDistributionDisplays(GameMode.Instance.puzzle);

        yield return new WaitForSeconds(1);

        OrderCardsDisplay();

        // process user turn via AI and set counter to 1 when done!
        GameMode.Instance.puzzle.IncrementTurn();

        yield return null;
    }

    public IEnumerator TallyHighestCardsToDrop()
    {
        HighestTally = 0;
        CardTallies = new Dictionary<eCardNumber, int>();
        foreach(Card card in Cards)
        {
            int val = (int)card.mNumber;
            if (val > 10)
                val = 10;
            if (CardTallies.ContainsKey(card.mNumber))
                CardTallies[card.mNumber] += val;
            else
                CardTallies.Add(card.mNumber, val);

            if(CardTallies[card.mNumber] > HighestTally)
            {
                HighestTally = CardTallies[card.mNumber];
                HighestTalliedCardNumber = card.mNumber;
            }
        }

        yield return null;
    }

    public IEnumerator FindMatchingCardsToDrop()
    {
        int count = GameMode.Instance.puzzle.UsedPile.Count;
        Card lastUsedPileCard = GameMode.Instance.puzzle.UsedPile[count - 1];
        MatchingCount = 0;
        foreach (Card card in Cards)
        {
            if(card == lastUsedPileCard)
            {
                int val = (int)card.mNumber;
                if (val > 10)
                    val = 10;

                MatchingCount += val;
            }
        }

        yield return null;
    }

    public IEnumerator DecideAndDropCards()
    {
        // Logic flow
        // - count our highest tally to drop - HighestTally
        // - count matching cards tally to drop - MatchingCount
        // - get a random value for draw pile trade-off - drawPileFactor
        // - count the tally for cards to pick from used pile - usedPileFactor
        // - find the biggest loser!

        if (GameMode.Instance.puzzle.LastDroppedCardCount == 0)
            GameMode.Instance.puzzle.LastDroppedCardCount = 1;
        int usedPileFactor = 0;
        int count = GameMode.Instance.puzzle.UsedPile.Count;
        for (int i = 0; i < GameMode.Instance.puzzle.LastDroppedCardCount; i++)
        {
            Card lastUsedPileCard = GameMode.Instance.puzzle.UsedPile[count - 1 - i];
            int val = (int)lastUsedPileCard.mNumber;
            if (val > 10)
                val = 10;

            usedPileFactor += val;
        }

        int drawPileFactor = Random.Range(1, 11);

        // does dropping matching cards reduce our tally greatly
        // or does dropping biggest bunch of cards and swapping for an unknown reduce better?
        // or does dropping biggest bunch of cards and swapping for top of used pile reduce better?
        int val1 = MatchingCount;
        int val2 = HighestTally - drawPileFactor;
        int val3 = HighestTally - usedPileFactor;
        // If our replacement card, either way, is not improving our tally,
        // and we don't have a matching card to drop!!
        //  may be we should check on calling leastCount!!
        if(val1 == 0 && val2 < 0 && val3 < 0)
        {
            GameMode.Instance.puzzle.LeastCount(this);
        }
        else
        {
            List<Card> CardsSelected = new List<Card>();
            if (val1 > 0 && val1 >= val2 && val1 >= val3)
            {
                // drop matching cards and reduce our card count
                int usedCount = GameMode.Instance.puzzle.UsedPile.Count;
                Card lastUsedPileCard = GameMode.Instance.puzzle.UsedPile[usedCount - 1];
                foreach (Card card in Cards)
                {
                    if (card == lastUsedPileCard)
                    {
                        CardsSelected.Add(card);
                    }
                }
            }
            else if (val3 >= val1 && val3 >= val2)
            {
                // drop our biggest cards and swap with set from used pile
                foreach (Card card in Cards)
                {
                    if (card.mNumber == HighestTalliedCardNumber)
                    {
                        CardsSelected.Add(card);
                    }
                }

                // Add these cards to our list
                int usedCount = GameMode.Instance.puzzle.UsedPile.Count;
                for (int i = 0; i < GameMode.Instance.puzzle.LastDroppedCardCount; i++)
                {
                    Card lastUsedPileCard = GameMode.Instance.puzzle.UsedPile[usedCount - 1 - i];
                    string hash = lastUsedPileCard.OnlineHash();

                    // Remove from used pile
                    GameMode.Instance.puzzle.UsedPile.Remove(lastUsedPileCard);
                    // add the card to our list of cards
                    lastUsedPileCard.SetPrefix(Globals.PLAYER_PREFIXES[PlayerIndex]);
                    lastUsedPileCard.mMoveDirty = true;
                    Cards.Add(lastUsedPileCard);

                    // Handle Online gameplay
                    this.OnlineParam.Add("used-rem&cards-add-" + i.ToString(), hash);
                }
            }
            else
            {
                // drop our biggest cards and swap with one from draw pile
                foreach (Card card in Cards)
                {
                    if (card.mNumber == HighestTalliedCardNumber)
                    {
                        CardsSelected.Add(card);
                    }
                }

                //Add the card from draw pile to our list
                int drawCount = GameMode.Instance.puzzle.DrawPile.Count;
                Card lastDrawPileCard = GameMode.Instance.puzzle.DrawPile[drawCount - 1];
                string hash = lastDrawPileCard.OnlineHash();

                // Remove from draw pile
                GameMode.Instance.puzzle.DrawPile.Remove(lastDrawPileCard);
                // add the card to our list of cards
                lastDrawPileCard.SetPrefix(Globals.PLAYER_PREFIXES[PlayerIndex]);
                lastDrawPileCard.mMoveDirty = true;
                Cards.Add(lastDrawPileCard);

                // Handle Online gameplay
                this.OnlineParam.Add("draw-rem&cards-add", hash);
            }

            // Set how many cards we are dropping onto used pile this turn
            GameMode.Instance.puzzle.LastDroppedCardCount = CardsSelected.Count;

            // Handle Online gameplay
            this.OnlineParam.Add("last-dropped-count", CardsSelected.Count.ToString());

            // drop our selected cards onto used pile, also remove them from our card list
            int dd = 0;
            foreach (Card card in CardsSelected)
            {
                string hash1 = card.OnlineHash();
                Cards.Remove(card);
                card.SetPrefix("__");
                card.mMoveDirty = true;
                GameMode.Instance.puzzle.UsedPile.Add(card);

                this.OnlineParam.Add("cards-rem&used-add-" + dd.ToString(), hash1);
                dd++;
            }
        }

        yield return null;
    }

    public void OrderCardsDisplay(bool init = false)
    {
        int movedCount = 0;
        for (int i = 0; i < Cards.Count; i++)
        {
            if (init)
                Cards[i].InitCardUI(Globals.PLAYERCARDS_START_X[this.PlayerIndex],
                                    Globals.PLAYERCARDS_START_Y[this.PlayerIndex],
                                    Globals.PLAYER_PREFIXES[this.PlayerIndex]);
            if(Cards[i].mMoveDirty)
            {
                Cards[i].mMoveDirty = false;
                Cards[i].MoveCard(Globals.PLAYERCARDS_START_X[this.PlayerIndex],
                                  Globals.PLAYERCARDS_START_Y[this.PlayerIndex],
                                  GameMode.Instance.puzzle.GetAIParentForIndex(this.PlayerIndex), 0.05f*movedCount);
                Cards[i].SetPrefix(Globals.PLAYER_PREFIXES[this.PlayerIndex]);
                movedCount++;
            }
            Cards[i].Close();
            Cards[i].SetSelectable(true);
        }
    }

}

