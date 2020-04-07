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
    E_CN_A = 1,
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

    private string      mName;
    private string      mPrefix;

    public bool         mMoveDirty;

    public Card(int _set, int _color, int _type, int _number)
    {
        this.mSet = _set;
        this.mColor = (eCardColor) _color;
        this.mType = (eCardType) _type;
        this.mNumber = (eCardNumber) _number;
        this.mCardObj = null;
        this.mCardUI = null;
        this.mPrefix = "";
        this.mMoveDirty = false;
        this.FormatName();
    }

    public Card(string cardStr, int set)
    {
        this.mSet = set;
        string[] toks = cardStr.Split('_');

        if (toks[0] == "Black")
            this.mColor = eCardColor.E_CC_BLACK;
        else
            this.mColor = eCardColor.E_CC_RED;

        switch(toks[1])
        {
            case "Club":
                mType = eCardType.E_CT_CLUB;
                break;
            case "Diamond":
                mType = eCardType.E_CT_DIAMOND;
                break;
            case "Heart":
                mType = eCardType.E_CT_HEART;
                break;
            case "Spade":
                mType = eCardType.E_CT_SPADE;
                break;
        }

        switch(toks[2])
        {
            case "10":
                mNumber = eCardNumber.E_CN_10;
                break;
            case "2":
                mNumber = eCardNumber.E_CN_2;
                break;
            case "3":
                mNumber = eCardNumber.E_CN_3;
                break;
            case "4":
                mNumber = eCardNumber.E_CN_4;
                break;
            case "5":
                mNumber = eCardNumber.E_CN_5;
                break;
            case "6":
                mNumber = eCardNumber.E_CN_6;
                break;
            case "7":
                mNumber = eCardNumber.E_CN_7;
                break;
            case "8":
                mNumber = eCardNumber.E_CN_8;
                break;
            case "9":
                mNumber = eCardNumber.E_CN_9;
                break;
            case "J":
                mNumber = eCardNumber.E_CN_J;
                break;
            case "Q":
                mNumber = eCardNumber.E_CN_Q;
                break;
            case "K":
                mNumber = eCardNumber.E_CN_K;
                break;
            case "A":
                mNumber = eCardNumber.E_CN_A;
                break;
        }

        this.mCardObj = null;
        this.mCardUI = null;
        this.mPrefix = "";
        this.mMoveDirty = false;
        this.FormatName();
    }

    private void FormatName()
    {
        string retVal = "";
        if (mColor == eCardColor.E_CC_BLACK)
            retVal += "Black_";
        else
            retVal += "Red_";

        switch (mType)
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

        switch (mNumber)
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

        mName = retVal;
    }

    public void Destroy()
    {
        GameObject.Destroy(this.mCardObj);
        GameObject.Destroy(this.mCardUI);
    }

    public static bool operator ==(Card lhs, Card rhs)
    {
        // for this game, if the number matches, that's good enough
        return(rhs.mNumber == lhs.mNumber);
    }

    public static bool operator !=(Card lhs, Card rhs)
    {
        // for this game, if the number matches, that's good enough
        return (rhs.mNumber != lhs.mNumber);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public void InitCardUI(float posx = -1000.0f, float posy = -1000.0f, string defaultName = "")
    {
        if (defaultName == "")
            defaultName = Globals.PLAYER_PREFIXES[0];

        Sprite sp = Resources.Load<Sprite>("Textures/Cards/" + this.ToString());

        mCardObj = GameObject.Instantiate(Resources.Load("Prefabs/CardUI")) as GameObject;
        SetPrefix(defaultName);
        SetParentByPrefix();

        RectTransform trans = mCardObj.transform as RectTransform;
        trans.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        Vector2 pos = trans.anchoredPosition;
        pos.x = posx;
        pos.y = posy;
        trans.anchoredPosition = pos;

        mCardUI = mCardObj.GetComponent<CardUI>();
        mCardUI.theCard = this;
        mCardUI.theImage.sprite = sp;
    }

    public void Open()
    {
        this.mCardUI.Open();
    }

    public void Close()
    {
        this.mCardUI.Close();
    }

    public void SetSelectable(bool toggle)
    {
        this.mCardUI.SetSelectable(toggle);
    }

    public void MoveCard(float posx, float posy, GameObject parent=null, float delay=0f, float time=0.5f)
    {
        if(this.mCardObj == null)
            return;

        if (parent == null)
            parent = mCardObj.transform.parent.gameObject;

        // Get our current position with respect to new parent
        //  Convert current position to screen coord
        RectTransform myTrans = mCardObj.transform as RectTransform;
        RectTransform myParentTrans = mCardObj.transform.parent.transform as RectTransform;
        Vector3 currentScreenPos = Globals.LocalRectToScreenPoint(myParentTrans, myTrans.anchoredPosition3D);
        // Update the transform to the target!!
        mCardObj.transform.SetParent(parent.transform);
        // Get the new parent transform
        RectTransform myNewTrans = mCardObj.transform as RectTransform;
        RectTransform myNewParentTrans = parent.transform as RectTransform;
        // Get current pos according to new transform
        Vector3 newTransPos = Globals.ScreenPointToLocalRect(myNewParentTrans, currentScreenPos);
        // Update our starting pos as per new transform
        Vector2 newPos = Vector2.zero;
        newPos.x = newTransPos.x;
        newPos.y = newTransPos.y;
        myNewTrans.anchoredPosition = newPos;

        Vector2 pos = new Vector2(posx, posy);

        iTween.ValueTo(mCardObj,
            iTween.Hash("from", newPos,
                        "to", pos,
                        "time", time,
                        "delay", delay,
                        "onupdate", "OnMoveUpdate",
                        "onupdatetarget", mCardObj,
                        "easetype", iTween.EaseType.easeInExpo));
    }

    public Vector2 GetPosition()
    {
        RectTransform trans = mCardObj.transform as RectTransform;
        return trans.anchoredPosition;
    }

    public void SetParentByPrefix()
    {
        GameObject parent = null;
        switch(mPrefix)
        {
            case "_":
                parent = GameMode.Instance.puzzle.MyPuzzleUI.DrawPileParent;
                break;
            case "__":
                parent = GameMode.Instance.puzzle.MyPuzzleUI.UsedPileParent;
                break;
            case Globals.PLAYER0_PREFIX:
                parent = GameMode.Instance.puzzle.MyPuzzleUI.Player0Parent;
                break;
            case Globals.PLAYER1_PREFIX:
                parent = GameMode.Instance.puzzle.MyPuzzleUI.Player1Parent;
                break;
            case Globals.PLAYER2_PREFIX:
                parent = GameMode.Instance.puzzle.MyPuzzleUI.Player2Parent;
                break;
            case Globals.PLAYER3_PREFIX:
                parent = GameMode.Instance.puzzle.MyPuzzleUI.Player3Parent;
                break;
            default:
                parent = GameMode.Instance.puzzle.MyPuzzleUI.UsedPileParent;
                break;
        }
        mCardObj.transform.SetParent(parent.transform, false);
    }

    public void SetPrefix(string prefix)
    {
        mCardObj.name = prefix + this.ToString();
        mPrefix = prefix;
    }

    public override string ToString()
    {
        return mName;
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
                        for(int l=1; l<(int)eCardNumber.E_CN_TOTAL; l++)
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

    public string DeckAsString()
    {
        return MiniJSON.Json.Serialize(mDeck);
    }

    public void DeckFromString(string deckstr)
    {
        mDeck.Clear();
        List<string> deckList = (List<string>)MiniJSON.Json.Deserialize(deckstr);
        List<string> deckSet = new List<string>();
        foreach(string str in deckList)
        {
            int set = 0;
            if (deckSet.Contains(str))
                set = 1;
            Card card = new Card(str, set);
            mDeck.Add(card);
        }
    }
}
