using UnityEngine;

public class Results : Mode
{
    [HideInInspector]
    public ResultsUI MyResultsUI;

    public override void EnterMode()
    {
        MyResultsUI = this.gameObject.GetComponent<ResultsUI>();
        MyResultsUI.OnInit();        

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
