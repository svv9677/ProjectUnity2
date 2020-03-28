using System;

public class AIPlayer: Player
{
    public AIPlayer ()
    {
    }

    public override void ProcessTurn(int teamIndex, int playerIndex)
    {
        if(this.TeamIndex == teamIndex && this.PlayerIndex == playerIndex)
        {
            // process user turn via AI and set counter to 1 when done!
            GameMode.Instance.puzzle.IncrementTurn();
        }
    }
}

