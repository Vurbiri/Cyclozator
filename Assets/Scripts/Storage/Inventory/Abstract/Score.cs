using System;

public abstract class Score
{
    private CLong _points;
    private CLong _coins;

    public long A => Coins + Points;
    public long B => Coins - Points;
  
    public long Points { get =>_points; protected set { _points = value; EventPointsChange?.Invoke(_points); } }
    public long Coins { get => _coins; protected set { _coins = value; if(_coins >= 0) EventCoinsChange?.Invoke(_coins); } }

    public event Action<string> EventPointsChange;
    public event Action<string> EventCoinsChange;

    public Score(long a, long b)
    {
        _coins = (a + b) / 2; _points = (a - b) / 2; 
    }

    public void ResetScore()
    {
        _points = 0;
        _coins = 0;
    }

    public void ResetEvent()
    {
        EventPointsChange = null;
        EventCoinsChange = null;
    }
}
