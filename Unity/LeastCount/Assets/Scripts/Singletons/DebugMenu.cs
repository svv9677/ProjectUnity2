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
        sb.Append("<b>Puzzle</b>: "); sb.Append(GameMode.Instance.puzzle.ToString()); sb.AppendLine();

        theText.text = sb.ToString();
	}
}

