using UnityEngine;
using System.Collections;

public static class Globals
{
    /// <summary>
    /// TOASTS
    /// </summary>
	public const string PREF_LEVEL = "Level";
	public static int TOAST_LARGE = 150;
	public static int TOAST_MEDIUM = 50;
	public static int TOAST_SMALL = 25;
	public static float TOAST_DURATION = 3.0f;

    public static GameObject theCanvas = null;
    public static GameObject theBoard = null;

	public static string ToastToString ()
	{
		return string.Format ("Toast: L({0}), M({1}), S({2}), Time({3})", TOAST_LARGE, TOAST_MEDIUM, TOAST_SMALL, TOAST_DURATION);
	}

	public delegate void ToastCallback();

	public static void ShowToast(string txt, int size, ToastCallback callback)
	{
		Toast.Instance.gameObject.SetActive(true);
		Toast.Instance.gameObject.transform.localPosition = new Vector3(0.0f, 1000.0f, 0.0f);
		Toast.Instance.Show(txt, size, callback);
		iTween.MoveTo(Toast.Instance.gameObject, 
			iTween.Hash("position", Vector3.zero, 
				"islocal", true, 
				"time", 1.0f, 
				"easeType", "easeOutBounce"));
	}

    /// <summary>
    /// Other globals
    /// </summary>
    public static int gMinimumPlayers = 2;
}
