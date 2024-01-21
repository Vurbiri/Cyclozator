
public class ScoreGame : Score
{
    public ScoreGame(int ratio) : base(0, 0) => _multiplierForLevel = ratio;
    public ScoreGame(long a, long b, int ratio = 1) : base(a, b) => _multiplierForLevel = ratio;

    private readonly int _multiplierForLevel;

    public void ScoreUpPerCatching(bool isCoin)
    {
        Points++;
        if (isCoin)
            Coins++;
    }

    public void ScoreDownPerMissing(int level, bool isCoin)
    {

        int points = 1;
        if (isCoin)
            points += level;

        if (Points > points)
            Points -= points;
        else
            Points = 0;
    }
    public void ScoreUpPerLevel(int level)
    {
        Points += level * _multiplierForLevel;
        if(level % 10 == 0) Coins++;
    }

    
        
}
