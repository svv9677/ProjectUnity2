using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ScoreRound
{
    public int[] Scores;
    
    public ScoreRound()
    {
        Scores = new int[Globals.gMinimumPlayers];
    }

    public void Set(int score1, int score2, int score3, int score4)
    {
        Scores[0] = score1;
        Scores[1] = score2;
        Scores[2] = score3;
        Scores[3] = score4;
    }

    public void Add(int score1, int score2, int score3, int score4)
    {
        Scores[0] += score1;
        Scores[1] += score2;
        Scores[2] += score3;
        Scores[3] += score4;
    }
}

public class ScoringManager : Singleton<ScoringManager> {

    public List<ScoreRound> Rounds;
    public List<ScoreRound> Counts;
    public ScoreRound Totals;

    public ScoringManager()
    {
        Rounds = new List<ScoreRound>();
        Counts = new List<ScoreRound>();
        Totals = new ScoreRound();
    }

    // Use this for initialization
    public void Load()
    {
    }

    public void AddScores(int[] scores, int[] counts)
    {
        ScoreRound round = new ScoreRound();
        round.Set(scores[0], scores[1], scores[2], scores[3]);
        ScoreRound count = new ScoreRound();
        count.Set(counts[0], counts[1], counts[2], counts[3]);

        Totals.Add(scores[0], scores[1], scores[2], scores[3]);
        Rounds.Add(round);
        Counts.Add(count);
    }
}
