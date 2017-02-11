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

    public Card(int _set, int _color, int _type, int _number)
    {
        this.mSet = _set;
        this.mColor = (eCardColor) _color;
        this.mType = (eCardType) _type;
        this.mNumber = (eCardNumber) _number;
    }
        

    public override string ToString() 
    {
        string retVal = "";
        if(mColor == eCardColor.E_CC_BLACK)
            retVal += "Black ";
        else
            retVal += "Red ";

        switch(mType)
        {
        case eCardType.E_CT_CLUB:
            retVal += "Club ";
            break;
        case eCardType.E_CT_DIAMOND:
            retVal += "Diamond ";
            break;
        case eCardType.E_CT_HEART:
            retVal += "Heart ";
            break;
        case eCardType.E_CT_SPADE:
            retVal += "Spade ";
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
                    for(int l=0; l<(int)eCardNumber.E_CN_TOTAL; l++)
                    {
                        Card card = new Card(i+1, j, k, l);
                        mDeck.Add(card);
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