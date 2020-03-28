using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public enum eTurnState {
    E_TS_SELECT_CARD = 0,
    E_TS_SELECT_SLOT,
    E_TS_PLACE_CHIP,
    E_TS_CHECK_WINNER,
    E_TS_DRAW_CARD,

    E_TS_TOTAL_TURNS
}

public class InputPlayer : Player
{
    private bool MyTurn = false;
    private eTurnState TurnState;
    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
    private string CardNameToDeploy = "";

    public InputPlayer ()
    {
        base.TeamIndex = 0;
        this.TurnState = eTurnState.E_TS_SELECT_CARD;
        this.OnEnable();
    }

    public override string ToString()
    {
        return "State: " + this.TurnState.ToString();
    }

    protected void OnEnable()
    {
        // Hook into the OnFingerTap event
        Lean.LeanTouch.OnFingerTap += OnFingerTap;
    }

    protected void OnDisable()
    {
        // Unhook into the OnFingerTap event
        Lean.LeanTouch.OnFingerTap -= OnFingerTap;
    }

    public void SetCards(List<Card> cards)
    {
        float x = -150.0f;
        float xdelta = 80.0f;
        this.Cards.AddRange(cards);
        for(int i=0; i<this.Cards.Count; i++)
        {
            this.Cards[i].InitCardUI(x + (i*xdelta), -500.0f);
        }
        this.TurnState = eTurnState.E_TS_SELECT_CARD;
    }

    public void OnFingerTap(Lean.LeanFinger finger)
    {
        // if its not my turn, just return
        // TODO: Need to update this logic for HUD taps
        if(!this.MyTurn)
            return;
        
        // read player taps for the following states
        if(this.TurnState == eTurnState.E_TS_SELECT_CARD ||
            this.TurnState == eTurnState.E_TS_SELECT_SLOT)
        {
            if(this.TurnState == eTurnState.E_TS_SELECT_SLOT && this.CardNameToDeploy == "")
                this.TurnState = eTurnState.E_TS_SELECT_CARD;
            
            // Raycast information
            var ray = finger.GetRay();
            var hit = default(RaycastHit);

            // Was this finger pressed down on a collider?
            if (Physics.Raycast(ray, out hit, float.PositiveInfinity, LayerMask) == true)
            {
                // Is that collider of interest?
                if (hit.collider.gameObject)
                {
                    if(this.TurnState == eTurnState.E_TS_SELECT_CARD && hit.collider.gameObject.name.Contains("MyPlayer"))
                    {
                        this.CardNameToDeploy = hit.collider.gameObject.name.Substring(9);
                        this.CardNameToDeploy = this.CardNameToDeploy.Substring(this.CardNameToDeploy.IndexOf("_")+1);
                        this.TurnState = eTurnState.E_TS_SELECT_SLOT;

                        return;
                    }
                    if(this.TurnState == eTurnState.E_TS_SELECT_SLOT && hit.collider.gameObject.name.Contains("Board"))
                    {
                        if(hit.collider.gameObject.name.Contains(this.CardNameToDeploy))
                        {
                            Vector2 newSpawnPos = Vector2.zero;
                            // remove card from our list and add it to used card list
                            for(int i=0; i< this.Cards.Count; i++)
                            {
                                if(this.Cards[i].ToString().Contains(this.CardNameToDeploy))
                                {
                                    Card theCard = this.Cards[i];

                                    newSpawnPos = theCard.GetLocalPos();
                                    theCard.MoveCard(-1000.0f, -1000.0f);
                                    this.Cards.Remove(theCard);

                                    GameMode.Instance.puzzle.UsedPile.Add(theCard);
                                    break;
                                }
                            }

                            // place our color chip on the board 
                            Vector2 boardPos = (hit.collider.gameObject.transform as RectTransform).anchoredPosition;
                            Chip newChip = new Chip();
                            newChip.InitChipUI(Color.green, boardPos.x, boardPos.y);

                            // TODO: check for winning condition
                            //  and accordingly draw next card!
                            Card newCard = GameMode.Instance.puzzle.DrawPile.GetRange(0, 1)[0];
                            newCard.InitCardUI(newSpawnPos.x, -500.0f);
                            this.Cards.Add(newCard);
                            GameMode.Instance.puzzle.DrawPile.RemoveRange(0, 1);

                            this.TurnState = eTurnState.E_TS_SELECT_CARD;
                            GameMode.Instance.puzzle.IncrementTurn();
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    public void Update ()
    {
        if (Input.GetKeyDown (KeyCode.Escape)) 
        {
            GameMode.Instance.SetMode(eMode.E_M_SPLASH);
        }

        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            if(DebugMenu.Instance.gameObject.activeSelf)
                DebugMenu.Instance.gameObject.SetActive(false);
            else
                DebugMenu.Instance.gameObject.SetActive(true);
        }
        //if(Input.GetKeyDown(KeyCode.S))
        //    DeckManager.Instance.mDeck.Shuffle();

    }

    public override void ProcessTurn(int teamIndex, int playerIndex)
    {
        if(this.TeamIndex == teamIndex && this.PlayerIndex == playerIndex)
            this.MyTurn = true;
        else
            this.MyTurn = false;
    }
}

