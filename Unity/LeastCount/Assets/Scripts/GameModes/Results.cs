using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Results : Mode
{
    public Text Player1Text;
    public Text Player2Text;
    public Text Player3Text;
    public Text Player4Text;

    public GameObject scrollContent;
    public GameObject resultItemPrefab;

    public override void EnterMode()
    {
        Player1Text.text = "MEEE\n100";
        Player2Text.text = "AI Player 1\n130";
        Player3Text.text = "AI Player 2\n117";
        Player4Text.text = "AI Player 3\n230";

        RectTransform pTrans = resultItemPrefab.transform as RectTransform;
        resultItemPrefab.SetActive(false);
        for (int i =0; i<30; i++)
        {
            GameObject obj = GameObject.Instantiate(resultItemPrefab) as GameObject;
            obj.transform.SetParent(scrollContent.transform, false);
            RectTransform trans = obj.transform as RectTransform;
            trans.anchoredPosition = pTrans.anchoredPosition;
            trans.anchorMin = pTrans.anchorMin;
            trans.anchorMax = pTrans.anchorMax;
            trans.localScale = Vector3.one;
            obj.SetActive(true);
            ResultsItem item = obj.GetComponent<ResultsItem>();
            string prefix = "";
            string postfix = "";
            if(i==0)
            {
                prefix = "<b>";
                postfix = "</b>";
            }
            item.Round.text = prefix + (30 - i).ToString() + postfix;
            item.Player1Score.text = prefix + Random.Range(3, 30).ToString() + postfix;
            item.Player2Score.text = prefix + Random.Range(3, 30).ToString() + postfix;
            item.Player3Score.text = prefix + Random.Range(3, 30).ToString() + postfix;
            item.Player4Score.text = prefix + Random.Range(3, 30).ToString() + postfix;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.transform as RectTransform);

        base.EnterMode();
    }

    public override void ExitMode()
    {

        base.ExitMode();
    }

    public void Update()
    {
        
    }
}
