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

	public bool Save(string name, bool online, int level=-1)
	{
        if(online)
        {
			// First connect online and check for unique name
			eConnectionState connected = OnlineManager.Instance.ConnectAs(name);

			if (connected == eConnectionState.E_CS_CONNECTED)
			{
				this.Name = name;
				if (level > 0)
					this.Level = level;
				PlayerPrefs.SetInt(Globals.PREF_LEVEL, this.Level);
				PlayerPrefs.SetString(Globals.PREF_NAME, this.Name);
			}
			else if (connected == eConnectionState.E_CS_DUPLICATE_NAME)
			{
				Globals.ShowToast("Name already taken!!", 30);
			}

			return connected == eConnectionState.E_CS_CONNECTED;
		}
        else
        {
			this.Name = name;
			if (level > 0)
				this.Level = level;
			PlayerPrefs.SetInt(Globals.PREF_LEVEL, this.Level);
			PlayerPrefs.SetString(Globals.PREF_NAME, this.Name);
			return true;
		}
	}

	public override string ToString ()
	{
		return string.Format ("Name: {0} - Level: {1}", Name, Level);
	}
}
