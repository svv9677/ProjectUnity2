using UnityEngine;
using System.Collections;

public class User : Singleton<User>
{
	public int Level { get; private set; }

	// Use this for initialization
	void Start ()
	{

	}

	public void Load()
	{
		this.Level = PlayerPrefs.HasKey(Globals.PREF_LEVEL) ? PlayerPrefs.GetInt(Globals.PREF_LEVEL) : 1;
	}

	public void Save()
	{
		PlayerPrefs.SetInt(Globals.PREF_LEVEL, this.Level);
	}

	public override string ToString ()
	{
		return string.Format ("Level: {0}", Level);
	}
}
