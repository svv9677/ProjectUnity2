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
        //sb.Append("<b>User</b>: "); sb.Append(User.Instance.ToString()); sb.AppendLine();
        sb.Append("<b>Puzzle</b>: "); sb.Append(GameMode.Instance.puzzle.ToString()); sb.AppendLine();
        if (GameMode.Instance.puzzle.MyPlayer != null)
        {
            sb.Append("<b>InputPlayer </b>: "); sb.Append(GameMode.Instance.puzzle.MyPlayer.ToString()); 
            sb.Append("<b>, Cards: </b>: "); sb.Append(GameMode.Instance.puzzle.MyPlayer.Cards.Count.ToString()); sb.AppendLine();
        }
        //Vector3 screenPos = Input.mousePosition;
        //Vector2 convPos1 = Globals.ScreenPointToLocalRect(GameMode.Instance.puzzle.Player0Parent.transform as RectTransform, screenPos);
        //Vector3 convPos2 = Globals.LocalRectToScreenPoint(GameMode.Instance.puzzle.Player0Parent.transform as RectTransform, new Vector3(convPos1.x, convPos1.y, 0f));
        //sb.Append("Screen: "+screenPos+ ", ==> "+convPos1); sb.AppendLine();
        //sb.Append("Back: " + convPos1 + ", ==> " + convPos2); sb.AppendLine();

        theText.text = sb.ToString();
	}
}

