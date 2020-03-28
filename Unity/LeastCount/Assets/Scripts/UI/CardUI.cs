using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardUI : MonoBehaviour {

    public Image theImage;
    public GameObject theBack;
    public GameObject theGray;
    public Card theCard;

    private BoxCollider theCollider;

    public void Start()
    {
        theCollider = GetComponent<BoxCollider>();
    }

    public void Open()
    {
        theGray?.SetActive(false);
        theBack?.SetActive(false);
    }

    public void Close()
    {
        theGray?.SetActive(false);
        theBack?.SetActive(true);
    }

    public void SetSelectable(bool toggle)
    {
        theGray?.SetActive(!toggle);
        if(theCollider) 
            theCollider.enabled = toggle;
    }

    public void OnMoveUpdate(Vector2 val)
    {
        RectTransform trans = this.transform as RectTransform;
        trans.anchoredPosition = val;
    }
}
