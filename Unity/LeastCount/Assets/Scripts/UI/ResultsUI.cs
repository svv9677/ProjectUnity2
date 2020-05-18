using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour
{
    public Text Player1Text;
    public Text Player2Text;
    public Text Player3Text;
    public Text Player4Text;

    public Text Message;

    public GameObject scrollContent;
    public GameObject resultItemPrefab;

    public void OnInit()
    {
        ScoringManager mgr = ScoringManager.Instance;
        Player1Text.text = GameMode.Instance.puzzle.Players[0].NickName + "\n" + mgr.Totals.Scores[0];
        Player2Text.text = GameMode.Instance.puzzle.Players[1].NickName + "\n" + mgr.Totals.Scores[1];
        Player3Text.text = GameMode.Instance.puzzle.Players[2].NickName + "\n" + mgr.Totals.Scores[2];
        Player4Text.text = GameMode.Instance.puzzle.Players[3].NickName + "\n" + mgr.Totals.Scores[3];

        for(int i=0; i<scrollContent.transform.childCount; i++)
        {
            GameObject.Destroy(scrollContent.transform.GetChild(i).gameObject);
        }

        RectTransform pTrans = resultItemPrefab.transform as RectTransform;
        resultItemPrefab.SetActive(false);
        for (int i = mgr.Rounds.Count-1; i >=0; i--)
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
            if (i == mgr.Rounds.Count - 1)
            {
                prefix = "<b>";
                postfix = "</b>";
            }
            item.Round.text = prefix + (i+1).ToString() + postfix;
            item.Player1Score.text = string.Format("{0}{1}({2}){3}", prefix, mgr.Rounds[i].Scores[0], mgr.Counts[i].Scores[0], postfix);
            item.Player2Score.text = string.Format("{0}{1}({2}){3}", prefix, mgr.Rounds[i].Scores[1], mgr.Counts[i].Scores[1], postfix);
            item.Player3Score.text = string.Format("{0}{1}({2}){3}", prefix, mgr.Rounds[i].Scores[2], mgr.Counts[i].Scores[2], postfix);
            item.Player4Score.text = string.Format("{0}{1}({2}){3}", prefix, mgr.Rounds[i].Scores[3], mgr.Counts[i].Scores[3], postfix);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollContent.transform as RectTransform);

        Message.text = "Press the back button on top-right part of the screen to continue to the next round!";
    }

}
