using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleUI : MonoBehaviour
{
    public void UpdateDistributionDisplays(Puzzle puzzle, bool init=false)
    {
        int movedCount = 0;
        for(int i=0; i<puzzle.UsedPile.Count; i++)
        {
            Card card = puzzle.UsedPile[i];
            if(init)
            {
                card.InitCardUI(0,0,"__");
                card.SetParentByPrefix();
            }
            card.Open();
            card.SetSelectable(true);
            var minCards = 1;
            if(puzzle.LastDroppedCardCount > minCards)
                minCards = puzzle.LastDroppedCardCount;
            if(card.mMoveDirty || i >= puzzle.UsedPile.Count-minCards)
            {
                float x = 0;
                if(i >= puzzle.UsedPile.Count-minCards)
                    x = (puzzle.UsedPile.Count-minCards-i) * 50f;

                card.mMoveDirty = false;
                card.MoveCard(x, 0f, GameMode.Instance.puzzle.UsedPileParent, 0.05f*movedCount);
                card.mCardObj.transform.SetAsLastSibling();
                movedCount++;
            }
            if(i < puzzle.UsedPile.Count-minCards)
                card.SetSelectable(false);
        }
        
        movedCount = 0;
        for(int i=0; i<puzzle.DrawPile.Count; i++)
        {
            Card card = puzzle.DrawPile[i];
            if(init)
            {
                card.InitCardUI(0,0,"_");
                card.SetParentByPrefix();
            }
            card.Close();
            card.SetSelectable(true);
            if(card.mMoveDirty)
            {
                card.mMoveDirty = false;
                card.MoveCard(0f, 0f, GameMode.Instance.puzzle.DrawPileParent, 0.05f*movedCount);
                card.mCardObj.transform.SetAsLastSibling();
            }
        }
    }

}
