using UnityEngine;
using System.Collections;

public class User : Singleton<User>
{
	public string Name;
	public int Level { get; private set; }

	// Use this for initialization
	void Start ()
	{

	}

	public void Load()
	{
		this.Level = PlayerPrefs.HasKey(Globals.PREF_LEVEL) ? PlayerPrefs.GetInt(Globals.PREF_LEVEL) : 1;
		this.Name = PlayerPrefs.HasKey(Globals.PREF_NAME) ? PlayerPrefs.GetString(Globals.PREF_NAME) : "";
		GameMode.Instance.splash.nameInput.text = this.Name;
	}

	public void Save(string name, int level=-1)
	{
		this.Name = name;
		if (level > 0)
			this.Level = level;
		PlayerPrefs.SetInt(Globals.PREF_LEVEL, this.Level);
		PlayerPrefs.SetString(Globals.PREF_NAME, this.Name);
	}

	public override string ToString ()
	{
		return string.Format ("Name: {0} - Level: {1}", Name, Level);
	}
}
