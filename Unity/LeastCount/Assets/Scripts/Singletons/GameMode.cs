using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum eMode
{
	E_M_NONE,
	E_M_SPLASH,
    E_M_ONLINE,
	E_M_PUZZLE,
	E_M_RESULTS,
}

public class GameMode : Singleton<GameMode> {

	[HideInInspector]
	public Mode modeObject = null;
	public eMode mode { get; private set; }

	public HUDUI hudUI;
	public Puzzle puzzle;
	public Splash splash;
	public Results results;
	public Online online;

    public GameObject canvasObj;
    public GameObject boardObj;
    public GameObject cardsParentObj;

	public string modeParam;

	// Use this for initialization
	void Start () {
		this.puzzle?.gameObject.SetActive(false);
		this.splash?.gameObject.SetActive(false);
		this.results?.gameObject.SetActive(false);
		this.online?.gameObject.SetActive(false);
		this.hudUI?.gameObject.SetActive(false);

		this.mode = eMode.E_M_NONE;
		SetMode (eMode.E_M_SPLASH);
	}
	
	public void SetMode(eMode new_mode, string param = "")
	{
		ExitMode();
		this.mode = new_mode;
		this.modeParam = param;
		EnterMode();
	}

	private void ExitMode()
	{
		if(this.mode != eMode.E_M_NONE)
		{
			this.modeObject.ExitMode();
			this.modeObject.gameObject.SetActive(false);
			this.modeObject = null;
		}
	}

	private void EnterMode()
	{
        if(Globals.theCanvas == null && this.canvasObj != null)
            Globals.theCanvas = this.canvasObj;

        if(Globals.theBoard == null && this.boardObj != null)
            Globals.theBoard = this.boardObj;

        switch (this.mode)
		{
		case eMode.E_M_SPLASH:
		{
			this.modeObject = this.splash;
			break;
		}
		case eMode.E_M_PUZZLE:
		{
			this.modeObject = this.puzzle;
			break;
		}
		case eMode.E_M_RESULTS:
        {
			this.modeObject = this.results;
			break;
        }
		case eMode.E_M_ONLINE:
		{
			this.modeObject = this.online;
			break;
		}
		default:
			break;
		}

		if(this.modeObject != null)
		{
			this.modeObject.gameObject.SetActive(true);
			this.modeObject.EnterMode();
		}
	}
}
