using UnityEngine;
using System.Collections;

public static class Globals
{
    /// <summary>
    /// TOASTS
    /// </summary>
	public const string PREF_LEVEL = "Level";
    public const string PREF_NAME = "Name";
    public static int TOAST_LARGE = 150;
	public static int TOAST_MEDIUM = 50;
	public static int TOAST_SMALL = 25;
	public static float TOAST_DURATION = 5.0f;

    public static GameObject theCanvas = null;
    public static GameObject theBoard = null;

	public static string ToastToString ()
	{
		return string.Format ("Toast: L({0}), M({1}), S({2}), Time({3})", TOAST_LARGE, TOAST_MEDIUM, TOAST_SMALL, TOAST_DURATION);
	}

	public delegate void ToastCallback();

	public static void ShowToast(string txt, int size = 30, float duration = 4.0f, ToastCallback callback = null)
	{
        Globals.TOAST_DURATION = duration;
		Toast.Instance.gameObject.SetActive(true);
		Toast.Instance.gameObject.transform.localPosition = new Vector3(0.0f, 1200.0f, 0.0f);
		Toast.Instance.Show(txt, size, callback);
		iTween.MoveTo(Toast.Instance.gameObject, 
			iTween.Hash("position", Vector3.zero, 
				"islocal", true, 
				"time", 0.5f, 
				"easeType", "easeOutBounce"));
	}

    public static Vector3 ScreenPointToLocalRect(RectTransform trans, Vector3 screenPos)
    {
        Vector2 retVal;
        Vector2 screenPos2D = new Vector2(screenPos.x, screenPos.y);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(trans, screenPos2D, Camera.main, out retVal);
        return retVal;
    }

    public static Vector3 LocalRectToScreenPoint(RectTransform trans, Vector3 localPos)
    {
        Vector2 conv = RectTransformUtility.WorldToScreenPoint(Camera.main, trans.TransformPoint(localPos));
        return new Vector3(conv.x, conv.y, 0);
    }

    /// <summary>
    /// Other globals
    /// </summary>
    public static int gMinimumPlayers = 4;
    public static int gCardsToDistribute = 7;

    // First one is for input player, rest for 3 AI players
    public const string PLAYER0_PREFIX = "ME__";
    public const string PLAYER1_PREFIX = "AI1_";
    public const string PLAYER2_PREFIX = "AI2_";
    public const string PLAYER3_PREFIX = "AI3_";
    public static readonly int[] PLAYERCARDS_START_X = { 0, 0, 0, 0 };
    public static readonly int[] PLAYERCARDS_START_Y = { 0, 0, 0, 0 };
    public static readonly string[] PLAYER_PREFIXES = { PLAYER0_PREFIX, PLAYER1_PREFIX, PLAYER2_PREFIX, PLAYER3_PREFIX };

    public const string PLAYER_READY = "IsPlayerReady";
    public const string HOST_START = "IsHostStarted";
}
