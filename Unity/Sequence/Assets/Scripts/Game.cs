using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Debug.Log("Initializing / Loading Deck Manager");
        DeckManager.Instance.Load();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
