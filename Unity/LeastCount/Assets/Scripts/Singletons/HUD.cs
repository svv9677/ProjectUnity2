using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HUD : Singleton<HUD> 
{
	public RectTransform BG1Transform;
	public RectTransform BG2Transform;

	// Use this for initialization
	void Start () {
		DeviceChange.OnResolutionChange += HUDResolutionChanged;
	}

	public void OnQuit()
	{
		GameMode.Instance.SetMode(eMode.E_M_SPLASH);
	}
	
	public void HUDResolutionChanged(Vector2 resolution)
	{
		float aspect = resolution.x / resolution.y;
		bool isLandscape = true;
		if(aspect <1.0f)
			isLandscape = false;

		RectTransform trans = this.gameObject.GetComponent<RectTransform>() as RectTransform;
		if(isLandscape)
		{
			trans.anchoredPosition = new Vector2(10.0f, 0.0f);
			trans.sizeDelta = new Vector2(300.0f, 1000.0f);
			trans.anchorMin = new Vector2(0.0f, 0.5f);
			trans.anchorMax = new Vector2(0.0f, 0.5f);
			trans.pivot = new Vector2(0.0f, 0.5f);
			BG1Transform.sizeDelta = new Vector2(300.0f, 980.0f);
			BG2Transform.sizeDelta = new Vector2(280.0f, 960.0f);
		}
		else
		{
			trans.anchoredPosition = new Vector2(0.0f, -10.0f);
			trans.sizeDelta = new Vector2(1000.0f, 300.0f);
			trans.anchorMin = new Vector2(0.5f, 1.0f);
			trans.anchorMax = new Vector2(0.5f, 1.0f);
			trans.pivot = new Vector2(0.5f, 1.0f);
			BG1Transform.sizeDelta = new Vector2(980.0f, 300.0f);
			BG2Transform.sizeDelta = new Vector2(960.0f, 280.0f);
		}
			
		
		Debug.LogWarning("Resolution changed to: " + resolution.ToString() + ". " + (isLandscape?"landscape":"portrait"));
	}
}
