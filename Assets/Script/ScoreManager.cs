using System;

public class ScoreManager
{
    public delegate void ScoreChangedDelegate(int newScore);
    public event ScoreChangedDelegate OnScoreChanged;

    private int score;

    public int Score
    {
        get => score;
        set
        {
            score = value;
            OnScoreChanged?.Invoke(score);
        }
    }

    public void Reset(int defaultScore = 0) => Score = defaultScore;    
}
