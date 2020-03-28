using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Chip
{
    public GameObject   mChipObj;
    public ChipUI       mChipUI;

    public Chip()
    {
        this.mChipObj = null;
    }

    public void InitChipUI(Color color, float posx = -1000.0f, float posy = -1000.0f)
    {
        mChipObj = GameObject.Instantiate(Resources.Load("Prefabs/ChipUI")) as GameObject;
        mChipObj.name = "MyPlayer_Chip";
        if(Globals.theBoard != null)
            mChipObj.transform.SetParent(Globals.theBoard.transform, false);

        RectTransform trans = mChipObj.transform as RectTransform;
        Vector2 pos = trans.anchoredPosition;
        pos.x = posx;
        pos.y = posy;
        trans.anchoredPosition = pos;

        mChipUI = mChipObj.GetComponent<ChipUI>();
        mChipUI.SetColor(color);
    }

    public Vector2 GetLocalPos()
    {
        if(this.mChipObj == null)
            return Vector2.zero;

        RectTransform trans = mChipObj.transform as RectTransform;
        return trans.anchoredPosition;
    }

    public void MoveChip(float posx, float posy, Transform parent=null)
    {
        if(this.mChipObj == null)
            return;

        if(parent != null)
            mChipObj.transform.SetParent(parent, false);

        RectTransform trans = mChipObj.transform as RectTransform;
        Vector2 pos = trans.anchoredPosition;
        pos.x = posx;
        pos.y = posy;
        trans.anchoredPosition = pos;
    }

}