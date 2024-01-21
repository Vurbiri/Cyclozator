using UnityEngine.Events;

public class SectorAnimation : ObjectAnimation
{
    public UnityEvent EventStart = new();
    public UnityEvent EventKnock = new();
    public UnityEvent EventEnd = new();

    public UnityEvent<DirectRotation> EventStartRotation = new();

    public override void Play()
    {
        base.Play();

        EventStartRotation.Invoke(DirectRotation.Left);
        OnStart();
    }

    public override void PlayRevers()
    {
        base.PlayRevers();

        EventStartRotation.Invoke(DirectRotation.Right);
        OnStart();
    }

    private void OnStart() => EventStart?.Invoke();
    private void OnKnock() => EventKnock?.Invoke();
    private void OnEnd() => EventEnd?.Invoke();

}

