using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum eCardType {
    E_CT_SPADE,
    E_CT_HEART,
    E_CT_CLUB, 
    E_CT_DIAMOND,
    E_CT_TOTAL,
};

public enum eCardColor {
    E_CC_RED,
    E_CC_BLACK,
    E_CC_TOTAL,
};

public enum eCardNumber {
    E_CN_2,
    E_CN_3,
    E_CN_4,
    E_CN_5,
    E_CN_6,
    E_CN_7,
    E_CN_8,
    E_CN_9,
    E_CN_10,
    E_CN_J,
    E_CN_Q,
    E_CN_K,
    E_CN_A,
    E_CN_TOTAL,
};

public class Card
{
    public eCardType    mType;
    public eCardNumber  mNumber;
    public eCardColor   mColor;
    public int          mSet;

    public GameObject   mCardObj;
    public CardUI       mCardUI;

    public Card(int _set, int _color, int _type, int _number)
    {
        this.mSet = _set;
        this.mColor = (eCardColor) _color;
        this.mType = (eCardType) _type;
        this.mNumber = (eCardNumber) _number;
        this.mCardObj = null;
    }

    public void InitCardUI(float posx = -1000.0f, float posy = -1000.0f)
    {
        mCardObj = GameObject.Instantiate(Resources.Load("Prefabs/CardUI")) as GameObject;
        mCardObj.name = "MyPlayer_"+this.ToString();
        if(Globals.theCanvas != null)
            mCardObj.transform.SetParent(Globals.theCanvas.transform, false);

        RectTransform trans = mCardObj.transform as RectTransform;
        Vector2 pos = trans.anchoredPosition;
        pos.x = posx;
        pos.y = posy;
        trans.anchoredPosition = pos;

        mCardUI = mCardObj.GetComponent<CardUI>();
        Sprite sp = Resources.Load <Sprite> ("Textures/Cards/" + this.ToString());
        mCardUI.theImage.sprite = sp;
    }

    public Vector2 GetLocalPos()
    {
        if(this.mCardObj == null)
            return Vector2.zero;

        RectTransform trans = mCardObj.transform as RectTransform;
        return trans.anchoredPosition;
    }

    public void MoveCard(float posx, float posy, Transform parent=null)
    {
        if(this.mCardObj == null)
            return;
        
        if(parent != null)
            mCardObj.transform.SetParent(parent, false);
        
        RectTransform trans = mCardObj.transform as RectTransform;
        Vector2 pos = trans.anchoredPosition;
        pos.x = posx;
        pos.y = posy;
        trans.anchoredPosition = pos;
    }

    public override string ToString() 
    {
        string retVal = "";
        if(mColor == eCardColor.E_CC_BLACK)
            retVal += "Black_";
        else
            retVal += "Red_";

        switch(mType)
        {
        case eCardType.E_CT_CLUB:
            retVal += "Club_";
            break;
        case eCardType.E_CT_DIAMOND:
            retVal += "Diamond_";
            break;
        case eCardType.E_CT_HEART:
            retVal += "Heart_";
            break;
        case eCardType.E_CT_SPADE:
            retVal += "Spade_";
            break;
        }

        switch(mNumber)
        {
        case eCardNumber.E_CN_10:
            retVal += "10";
            break;
        case eCardNumber.E_CN_2:
            retVal += "2";
            break;
        case eCardNumber.E_CN_3:
            retVal += "3";
            break;
        case eCardNumber.E_CN_4:
            retVal += "4";
            break;
        case eCardNumber.E_CN_5:
            retVal += "5";
            break;
        case eCardNumber.E_CN_6:
            retVal += "6";
            break;
        case eCardNumber.E_CN_7:
            retVal += "7";
            break;
        case eCardNumber.E_CN_8:
            retVal += "8";
            break;
        case eCardNumber.E_CN_9:
            retVal += "9";
            break;
        case eCardNumber.E_CN_J:
            retVal += "J";
            break;
        case eCardNumber.E_CN_Q:
            retVal += "Q";
            break;
        case eCardNumber.E_CN_K:
            retVal += "K";
            break;
        case eCardNumber.E_CN_A:
            retVal += "A";
            break;
        }

        return retVal;
    }
}

public class DeckManager : Singleton<DeckManager>
{
    public List<Card> mDeck = null;

    // Use this for initialization
    public void Load()
    {
        mDeck = new List<Card>();

        for(int i=0; i<2; i++) // 2 sets
        {
            for(int j=0; j<(int)eCardColor.E_CC_TOTAL; j++)
            {
                for(int k=0; k<(int)eCardType.E_CT_TOTAL; k++)
                {
                    if( (j == (int)eCardColor.E_CC_BLACK &&
                        (k == (int)eCardType.E_CT_CLUB || k == (int)eCardType.E_CT_SPADE)) 
                        || 
                        (j == (int)eCardColor.E_CC_RED &&
                        (k == (int)eCardType.E_CT_HEART || k == (int)eCardType.E_CT_DIAMOND))
                      )
                    {
                        for(int l=0; l<(int)eCardNumber.E_CN_TOTAL; l++)
                        {
                            Card card = new Card(i+1, j, k, l);
                            mDeck.Add(card);
                        }
                    }
                }
            }
        }
        mDeck.Shuffle();
    }

    public override void Destroy()
    {
        base.Destroy();
    }

    // Update is called once per frame
    void Update ()
    {
    }
}