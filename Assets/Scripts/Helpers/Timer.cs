using Cysharp.Threading.Tasks;
using System;

public class Timer 
{
    private float _tick;
    private DateTime _endTime;

    public bool IsPlay { get; private set; } = false;
   
    public event Action<TimeSpan> EventUpdate;
    public event Action EventStop;

    public void Setup(DateTime endTime, float tick = 1f)
    {
        _tick = tick;
        TimeSpan delta;
        if ((delta = endTime - DateTime.Now) > TimeSpan.Zero)
            Start(delta);
    }

    public DateTime Start(TimeSpan time)
    {
        if (IsPlay) return _endTime;

        IsPlay = true;
        Countdown(time).Forget();

        return _endTime = DateTime.Now.Add(time);
    }

    public void Stop() => IsPlay = false;

    public void Break()
    {
        EventUpdate = null;
        EventStop = null;
        IsPlay = false;
    }

    private async UniTask Countdown(TimeSpan time)
    {
        while (time > TimeSpan.Zero)
        {
            if (!IsPlay) break;
            
            EventUpdate?.Invoke(time);
            await UniTask.Delay((int)(_tick * 1000), true);
            time = _endTime - DateTime.Now;

        }

        IsPlay = false;
        EventStop?.Invoke();
    }
}
