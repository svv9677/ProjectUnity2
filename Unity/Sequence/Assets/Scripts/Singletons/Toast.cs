using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Text;

public class Toast : Singleton<Toast>
{
	public Text theText;
	Globals.ToastCallback theCallback;

	public void Show(string txt, int size, Globals.ToastCallback callback)
	{
		theText.text = txt;
		theText.fontSize = size;
		theCallback = callback;

		iTween.MoveTo(Toast.Instance.gameObject, 
			iTween.Hash("position", new Vector3(0.0f, 1000.0f, 0.0f), 
				"islocal", true, 
				"time", 1.0f, "delay", Globals.TOAST_DURATION,
				"easeType", "easeOutBounce",
				"oncomplete", "Hide", "oncompletetarget", Toast.Instance.gameObject));
	}

	public void Hide()
	{
		theCallback();
		theCallback = null;
	}

}

