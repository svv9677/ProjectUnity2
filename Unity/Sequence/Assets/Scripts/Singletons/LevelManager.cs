using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public struct LevelData
{
	public int Level;


	public void LoadFromDict(Dictionary<string, object> data)
	{
		Level = System.Int32.Parse ((string)data ["Level"]);
	}

	public override string ToString()
	{
		System.Text.StringBuilder str = new System.Text.StringBuilder();
		str.Append(Level.ToString ());

		return str.ToString();
	}
	/*{
		"Level":"1"
	  },
	*/
}

public class LevelManager : Singleton<LevelManager>
{
	protected delegate void FileLoadedCallback(int result, string data);
	public Dictionary<int, LevelData> Levels;

	// Use this for initialization
	public void Load()
	{
        List<object> data = null;
        try
        {
    		string FilePath = System.IO.Path.Combine("Blueprints", "db_Levels.json");
	    	data = FileUtils.Instance.GetJsonAsset<List<object>>(FilePath);
        } catch(DirectoryNotFoundException)
        {
            data = null;
        }

        if(data == null)
            return;
		Levels = new Dictionary<int,LevelData> ();
		for (int i=0; i< data.Count; i++) 
		{
			Dictionary<string, object> dict = (Dictionary<string, object>)data [i];
			LevelData level = new LevelData ();
			level.LoadFromDict (dict);
			Levels.Add (level.Level, level);
			//Debug.Log ("Adding level: " + level.ToString());
		}
	}

	public LevelData GetLevelData(int level)
	{
		LevelData data;
		Levels.TryGetValue(level, out data);
		// TODO: Error check for invalid data!
		return data;
	}

	public override void Destroy()
	{
		Levels = null;

		base.Destroy();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

