using UnityEngine;
using System.Collections;
using System.IO;

public class FileUtils : Singleton<FileUtils> {

	// Use this for initialization
	void Start () {
	
	}
	
	public T GetJsonAsset<T>(string FilePath)
	{
		string appliedFilePath = GetStreamingAssetsPath(FilePath);
		
		string json = "";
		if(appliedFilePath.Contains("://"))
		{
			json = getJsonTextFromWWW(appliedFilePath);
		}
		else
		{
			json = File.ReadAllText(appliedFilePath);
		}
		
		T des = (T)MiniJSON.Json.Deserialize(json);
		
		return des;
	}

	public string GetStreamingAssetsPath(string fname)
	{
		string oldPath = System.IO.Path.Combine(Application.streamingAssetsPath, fname);
		
		string patchPath = GetPersistentAssetsPath();
		if (!string.IsNullOrEmpty(patchPath))
		{
			string newPath = System.IO.Path.Combine(patchPath, fname);
			if (System.IO.File.Exists(newPath))
				return newPath;
		}
		return oldPath;
	}

	public static string GetPersistentAssetsPath()
	{
		return Path.Combine(Application.persistentDataPath, "Contents");
	}

	public static string GetStreamingAssetsPath()
	{
		return Application.streamingAssetsPath;
	}
	
	public static string getJsonTextFromWWW(string filePath)
	{
		WWW www = null;
		www = new WWW(filePath);
		
		while (www.isDone == false) {
			// do nothing
		}
		
		return www.text;
	}
}
