using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BoardElement
{
    public int xIndex;
    public int yIndex;
    public Card theCard;

    public BoardElement()
    {
        xIndex = yIndex = 0;
        theCard = null;
    }

    public void LoadFromDict(Dictionary<string, object> dict)
    {
//        try
//        {
            xIndex = System.Int32.Parse ((string)dict ["xIndex"]);
            yIndex = System.Int32.Parse ((string)dict ["yIndex"]);
            int set = System.Int32.Parse ((string)dict ["set"]);
            int color = System.Int32.Parse ((string)dict ["color"]);
            int type = System.Int32.Parse ((string)dict ["type"]);
            int number = System.Int32.Parse ((string)dict ["number"]);
            theCard = new Card(set, color, type, number);
//        } catch(Exception)
//        {
//            Debug.LogError("Error while loading from dictionary: " + dict.ToString());
//        }
    }

    public override string ToString()
    {
        return xIndex.ToString() + ", " + yIndex.ToString() + " : " + theCard.ToString();
    }
}

public class ScoringManager : Singleton<ScoringManager> {

    public List<BoardElement> theElements;

    // Use this for initialization
    public void Load()
    {
        List<object> data = null;
        try
        {
            string FilePath = System.IO.Path.Combine("Blueprints", "db_BoardElements.json");
            data = FileUtils.Instance.GetJsonAsset<List<object>>(FilePath);
        } catch(DirectoryNotFoundException)
        {
            data = null;
        }

        if(data == null)
            return;
        theElements = new List<BoardElement> ();
        for (int i=0; i< data.Count; i++) 
        {
            Dictionary<string, object> dict = (Dictionary<string, object>)data [i];
            BoardElement elem = new BoardElement ();
            elem.LoadFromDict (dict);
            theElements.Add (elem);
            Debug.Log ("Adding element: " + elem.ToString());
        }
    }

    public void CheckGameForInputPlayer()
    {
        

    }
}
