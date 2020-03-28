using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class DebugMenu : Singleton<DebugMenu>
{
	public Text theText;
    public DebugMenu instanceInHierarchy;

	public void Update()
	{        
		StringBuilder sb = new StringBuilder();

		sb.Append("<b>Screen</b>: "); sb.Append(Screen.width); sb.Append(", "); sb.Append(Screen.height); sb.AppendLine();

//		sb.Append("<b>Toasts</b>: "); sb.Append(Globals.ToastToString()); sb.AppendLine();
//		sb.Append("<b>User</b>: "); sb.Append(User.Instance.ToString()); sb.AppendLine();
		sb.Append("<b>Puzzle</b>: "); sb.Append(GameMode.Instance.puzzle.ToString()); sb.AppendLine();
        sb.Append("<b>Deck count</b>: "); sb.Append(DeckManager.Instance.mDeck.Count.ToString()); sb.AppendLine();
        if(GameMode.Instance.puzzle.MyPlayer != null)
        {
            sb.Append("<b>InputPlayer </b>: "); sb.Append(GameMode.Instance.puzzle.MyPlayer.ToString()); sb.AppendLine();
            sb.Append("<b>:Cards: </b>: "); sb.Append(GameMode.Instance.puzzle.MyPlayer.Cards.Count.ToString()); sb.AppendLine();
            sb.Append("<b>:Chips: </b>: "); sb.Append(GameMode.Instance.puzzle.MyPlayer.Chips.Count.ToString()); sb.AppendLine();
        }

		theText.text = sb.ToString();

	}
}

