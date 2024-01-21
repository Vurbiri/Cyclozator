
public class ScoreTotal : Score
{
    private int _conversionRate;

    public ScoreTotal() : base(0, 0) { }
    public ScoreTotal(long a, long b) : base(a, b) { }

    public void SetConversionRate(int conversionRate) => _conversionRate = conversionRate;

    public bool TryPointsToCoins(long coins)
    {
        long balance = Points - coins * _conversionRate;

        if (balance < 0) return false;

        Coins += coins;
        Points = balance;
        return true;
    }

    public bool TrySpendCoins(int coins)
    {
        if (PossibleMaxCoins() < coins) return false;

        Coins -= coins;
        
        if (Coins < 0)
            TryPointsToCoins(-Coins);

        return true;
    }

    public void AddCoins(int coins) => Coins += coins;

    public void DumpScore(ScoreGame currentScore)
    {
        Points += currentScore.Points;
        Coins += currentScore.Coins;
        currentScore.ResetScore();
    }

    public long PossibleMaxCoins()
    {
        int newCoins = (int)(Points / _conversionRate);
        return newCoins + Coins;
    }

}
