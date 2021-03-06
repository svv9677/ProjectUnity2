﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum eTurnState {
    E_TS_NONE = 0,
    E_TS_SELECT_CARDS,
    E_TS_EXCHANGE_CARDS,

    E_TS_TOTAL_TURNS
}

public class InputPlayer : GamePlayer
{
    private eTurnState TurnState;
    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
    private List<Card> CardsSelected;

    public InputPlayer ()
    {
        this.TurnState = eTurnState.E_TS_NONE;
    }

    public override string ToString()
    {
        return "State: " + this.TurnState.ToString();
    }

    protected void OnEnable()
    {
        // Hook into the OnFingerTap event
        Lean.LeanTouch.OnFingerTap += OnFingerTap;
        //Lean.LeanTouch.OnMultiTap += OnMultiTap;
    }

    public override void OnDisabled()
    {
        // Unhook into the OnFingerTap event
        Lean.LeanTouch.OnFingerTap -= OnFingerTap;
        //Lean.LeanTouch.OnMultiTap -= OnMultiTap;
        //Debug.Log("Input Player onDisabled calleD!!");
    }

    public override void SetCards(List<Card> cards)
    {
        Cards.AddRange(cards);

        OrderCardsDisplay(true);

        SetTurnState(eTurnState.E_TS_SELECT_CARDS);
    }

    public void OrderCardsDisplay(bool init=false)
    {
        int x = Globals.PLAYERCARDS_START_X[this.PlayerIndex];
        int xdelta = 180;
        int halfCount = Cards.Count / 2;
        for(int i=0; i<Cards.Count; i++)
        {
            int xx = i - halfCount;
            if (init)
                Cards[i].InitCardUI(x + (xx * xdelta), Globals.PLAYERCARDS_START_Y[this.PlayerIndex],
                                    Globals.PLAYER_PREFIXES[this.PlayerIndex]);
            else
            {
                Cards[i].MoveCard(x + (xx * xdelta), Globals.PLAYERCARDS_START_Y[this.PlayerIndex],
                                  GameMode.Instance.puzzle.MyPuzzleUI.Player0Parent, 0.05f*i);
                Cards[i].SetPrefix(Globals.PLAYER_PREFIXES[this.PlayerIndex]);
            }
            Cards[i].Open();
            Cards[i].SetSelectable(true);
            Cards[i].mMoveDirty = false;
        }
    }

    public void SetTurnState(eTurnState state)
    {
        switch(state)
        {
            case eTurnState.E_TS_SELECT_CARDS:
                CardsSelected = new List<Card>();
                break;
            default:
                break;
        }
        this.TurnState = state;
    }

//    public void OnMultiTap(int tapCount)
//    {
//#if UNITY_EDITOR
//        if (tapCount == 4)
//            ToggleDebugMenu();
//        else if(tapCount >= 5)
//            GameMode.Instance.SetMode(eMode.E_M_SPLASH);
//#endif
//    }

    public void OnFingerTap(Lean.LeanFinger finger)
    {
        // if its not my turn, just return
        // TODO: Need to update this logic for HUD taps
        if(!this.MyTurn)
            return;
        
        // read player taps for the following states
        if(this.TurnState == eTurnState.E_TS_SELECT_CARDS)
        {  
            // Raycast information
            var ray = finger.GetRay();
            var hit = default(RaycastHit);

            // Was this finger pressed down on a collider?
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
            {
                // Is that collider of interest?
                if (hit.collider.gameObject)
                {
                    CardUI cardUI = hit.collider.gameObject.GetComponent<CardUI>();
                    if (cardUI == null)
                        return;

                    // if we have one or more cards selected, check where we want to discard them!
                    if (CardsSelected.Count > 0)
                    {
                        // Did we select a card on the used pile or draw pile?
                        bool usedPileTapped = GameMode.Instance.puzzle.UsedPile.Contains(cardUI.theCard);
                        bool drawPileTapped = GameMode.Instance.puzzle.DrawPile.Contains(cardUI.theCard);
                        if (usedPileTapped || drawPileTapped)
                        {
                            // check if the last card in used pile matches our cards selected to drop
                            int count = GameMode.Instance.puzzle.UsedPile.Count;
                            Card lastUsedPileCard = GameMode.Instance.puzzle.UsedPile[count - 1];
                            bool cardMatches = (CardsSelected[0] == lastUsedPileCard);
                            if (usedPileTapped)
                            {
                                // if its a different card, we need to pick the top 'x' cards from used pile
                                //  and add it to our card list
                                if (!cardMatches)
                                {
                                    Dictionary<string, string> param = new Dictionary<string, string>();
                                    if (GameMode.Instance.puzzle.LastDroppedCardCount == 0)
                                        GameMode.Instance.puzzle.LastDroppedCardCount = 1;
                                    for (int i = 0; i < GameMode.Instance.puzzle.LastDroppedCardCount; i++)
                                    {
                                        count = GameMode.Instance.puzzle.UsedPile.Count;
                                        // Err, this is happening some times, dunno why
                                        if (count == 0)
                                        {
                                            GameMode.Instance.puzzle.UsedPile.AddRange(GameMode.Instance.puzzle.DrawPile.GetRange(0, 1));
                                            GameMode.Instance.puzzle.DrawPile.RemoveRange(0, 1);
                                            count = 1;
                                        }
                                        lastUsedPileCard = GameMode.Instance.puzzle.UsedPile[count - 1];
                                        string hash = lastUsedPileCard.OnlineHash();

                                        // Remove from used pile
                                        GameMode.Instance.puzzle.UsedPile.Remove(lastUsedPileCard);
                                        // add the card to our list of cards
                                        lastUsedPileCard.SetPrefix(Globals.PLAYER_PREFIXES[PlayerIndex]);
                                        lastUsedPileCard.mMoveDirty = true;
                                        Cards.Add(lastUsedPileCard);

                                        // Handle Online gameplay
                                        param.Add("used-rem&cards-add-"+i.ToString(), hash);
                                    }
                                    OnlineManager.Instance.NetworkMessage(eMessage.E_M_PLAYER_ACTION, MiniJSON.Json.Serialize(param), this);
                                }
                            }
                            else
                            {
                                // if its a different card, we need to pick the top card from draw pile
                                //  and add it to our card list
                                if (!cardMatches)
                                {
                                    Dictionary<string, string> param = new Dictionary<string, string>();
                                    count = GameMode.Instance.puzzle.DrawPile.Count;
                                    Card lastDrawPileCard = GameMode.Instance.puzzle.DrawPile[count - 1];
                                    string hash = lastDrawPileCard.OnlineHash();

                                    // Remove from draw pile
                                    GameMode.Instance.puzzle.DrawPile.Remove(lastDrawPileCard);
                                    // add the card to our list of cards
                                    lastDrawPileCard.SetPrefix(Globals.PLAYER_PREFIXES[PlayerIndex]);
                                    lastDrawPileCard.mMoveDirty = true;
                                    Cards.Add(lastDrawPileCard);

                                    // Handle Online gameplay
                                    param.Add("draw-rem&cards-add", hash);

                                    OnlineManager.Instance.NetworkMessage(eMessage.E_M_PLAYER_ACTION, MiniJSON.Json.Serialize(param), this);
                                }
                            }

                            // Set how many cards we are dropping onto used pile this turn
                            GameMode.Instance.puzzle.LastDroppedCardCount = CardsSelected.Count;

                            // Handle Online gameplay
                            Dictionary<string, string> param1 = new Dictionary<string, string>();
                            param1.Add("last-dropped-count", CardsSelected.Count.ToString());

                            // drop our selected cards onto used pile, also remove them from our card list
                            int dd = 0;
                            foreach (Card card in CardsSelected)
                            {
                                string hash1 = card.OnlineHash();
                                Cards.Remove(card);
                                card.SetPrefix("__");
                                card.mMoveDirty = true;
                                GameMode.Instance.puzzle.UsedPile.Add(card);

                                param1.Add("cards-rem&used-add-"+dd.ToString(), hash1);
                                dd++;
                            }

                            TurnState = eTurnState.E_TS_EXCHANGE_CARDS;
                            StartCoroutine(PerformDisplay());

                            param1.Add("incr-turn", "");
                            OnlineManager.Instance.NetworkMessage(eMessage.E_M_PLAYER_ACTION, MiniJSON.Json.Serialize(param1), this);

                            return;
                        }
                    }

                    // Did we select a card from our pile?
                    if (Cards.Contains(cardUI.theCard))
                    {
                        // are there any other cards selected?
                        if (CardsSelected.Count > 0)
                        {
                            // check if current selected card matches already selected!
                            foreach (Card card in CardsSelected)
                            {
                                // if it does not match, just do nothing!
                                if (card != cardUI.theCard)
                                    return;
                            }
                        }
                        // we are here because we selected a card that matches existing selected cards

                        // is this already a selected card, tapped again?
                        if (CardsSelected.Contains(cardUI.theCard))
                        {
                            // de-highlight the card
                            Vector2 pos = (cardUI.theCard.mCardObj.transform as RectTransform).anchoredPosition;
                            pos.x -= 10;
                            pos.y -= 10;
                            cardUI.theCard.MoveCard(pos.x, pos.y, null, 0f, 0.1f);
                            CardsSelected.Remove(cardUI.theCard);
                        }
                        else
                        {
                            // highlight the selected card
                            Vector2 pos = (cardUI.theCard.mCardObj.transform as RectTransform).anchoredPosition;
                            pos.x += 10;
                            pos.y += 10;
                            cardUI.theCard.MoveCard(pos.x, pos.y, null, 0f, 0.1f);
                            CardsSelected.Add(cardUI.theCard);
                        }

                        // disable all non-selectable cards in hand, based on current selection
                        Card oneSelectedCard = null;
                        if (CardsSelected.Count > 0)
                            oneSelectedCard = CardsSelected[0];
                        foreach (Card card in Cards)
                        {
                            card.SetSelectable(CardsSelected.Contains(card) || CardsSelected.Count == 0 ||
                                                card == oneSelectedCard);
                        }
                    }
                }
            }
        }
    }

    public IEnumerator PerformDisplay()
    {
        GameMode.Instance.puzzle.MyPuzzleUI.UpdateDistributionDisplays(GameMode.Instance.puzzle);

        yield return new WaitForSeconds(1);

        OrderCardsDisplay();

        yield return new WaitForSeconds(1);

        TurnState = eTurnState.E_TS_NONE;

        GameMode.Instance.puzzle.IncrementTurn();
        yield return null;
    }

    // Update is called once per frame
    //public void Update ()
    //{
    //    if (Input.GetKeyDown (KeyCode.Escape)) 
    //        GameMode.Instance.SetMode(eMode.E_M_SPLASH);

    //    if(Input.GetKeyDown(KeyCode.BackQuote))
    //        ToggleDebugMenu();

    //    if(Input.GetKeyDown(KeyCode.R))
    //        GameMode.Instance.SetMode(eMode.E_M_RESULTS);
    //}

    //public void ToggleDebugMenu()
    //{
    //    if (DebugMenu.Instance.gameObject.activeSelf)
    //        DebugMenu.Instance.gameObject.SetActive(false);
    //    else
    //        DebugMenu.Instance.gameObject.SetActive(true);
    //}

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
            SetTurnState(eTurnState.E_TS_SELECT_CARDS);
        }
    }
}

