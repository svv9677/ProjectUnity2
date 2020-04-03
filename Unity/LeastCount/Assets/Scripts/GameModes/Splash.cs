using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Splash : Mode {

	public Image splashImage;
	public Button startSinglePlayer;
	public Button startOnlinePlayer;
	public InputField nameInput;

	private eMode nextMode;

	// Use this for initialization
	void Start () {
		this.mode = eMode.E_M_SPLASH;
	}

	protected void StartTween(bool hideFlags, float time=0.5f)
	{
		if (time > 0.0f) 
		{
			iTween.MoveTo (this.splashImage.gameObject, 
			               iTween.Hash ( "x", hideFlags ? -1080.0f : 0.0f, "time", time, 
	                                     "easetype", hideFlags ? iTween.EaseType.easeInCirc : iTween.EaseType.easeOutElastic,
	                                     "oncomplete", "OnFadeComplete", 
	                                     "oncompleteparams", !hideFlags,
	                                     "oncompletetarget", this.gameObject));
		}
		else 
		{
			this.SetVisible (hideFlags);
		}
	}

	public void OnFadeComplete(object param)
	{
		bool hideFlag = (bool)param;

		this.SetVisible (hideFlag);
		if(!hideFlag)
			GameMode.Instance.SetMode(this.nextMode);
	}

	protected void SetVisible(bool hideFlags)
	{
		this.splashImage.gameObject.SetActive (hideFlags);
		this.gameObject.SetActive (hideFlags);
	}

	public override void EnterMode()
	{
		DebugMenu.Instance.gameObject.SetActive(false);
		Toast.Instance.gameObject.SetActive(false);

		this.SetVisible (true);
		this.splashImage.transform.localPosition = new Vector3(-1920.0f,0,0);
		this.startSinglePlayer.onClick.AddListener(OnStartSingleplayer);
		this.startOnlinePlayer.onClick.AddListener(OnStartMultiplayer);
		this.StartTween (false);

        ScoringManager.Instance.Load();
        DeckManager.Instance.Load();
		User.Instance.Load();
	}

	public override void ExitMode()
	{
		this.startSinglePlayer.onClick.RemoveListener(OnStartSingleplayer);
		this.startOnlinePlayer.onClick.RemoveListener(OnStartMultiplayer);
	}

	public void OnStartSingleplayer()
	{
		string finalName = nameInput.text.Trim();
		if (finalName == "")
		{
			Globals.ShowToast("Please enter a valid name", 30);
			return;
		}
		if (User.Instance.Save(finalName, false))
		{
			this.nextMode = eMode.E_M_PUZZLE;
			this.StartTween(true, 1.0f);
		}
	}

	public void OnStartMultiplayer()
	{
		string finalName = nameInput.text.Trim();
		if (finalName == "")
		{
			Globals.ShowToast("Please enter a valid name", 30);
			return;
		}
		if (User.Instance.Save(finalName, true))
		{
			this.nextMode = eMode.E_M_ONLINE;
			this.StartTween(true, 1.0f);
		}
	}
}
