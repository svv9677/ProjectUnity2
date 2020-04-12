using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayer : GamePlayer
{
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
            // Now wait for online events to be handled and dispatched by OnlineManager
        }
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
            if (Cards[i].mMoveDirty)
            {
                Cards[i].mMoveDirty = false;
                Cards[i].MoveCard(Globals.PLAYERCARDS_START_X[this.PlayerIndex],
                                  Globals.PLAYERCARDS_START_Y[this.PlayerIndex],
                                  GameMode.Instance.puzzle.GetAIParentForIndex(this.PlayerIndex), 0.05f * movedCount);
                Cards[i].SetPrefix(Globals.PLAYER_PREFIXES[this.PlayerIndex]);
                movedCount++;
            }
            Cards[i].Close();
            Cards[i].SetSelectable(true);
        }
    }

    public IEnumerator PerformDisplay()
    {
        GameMode.Instance.puzzle.MyPuzzleUI.UpdateDistributionDisplays(GameMode.Instance.puzzle);

        yield return new WaitForSeconds(1);

        this.OrderCardsDisplay();

        yield return new WaitForSeconds(1);

        GameMode.Instance.puzzle.IncrementTurn();
        yield return null;
    }

}
