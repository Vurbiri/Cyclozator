using System;
using System.Collections;
using UnityEngine;

public abstract class SmoothStartEnd<T> : Singleton<T> where T : Singleton<T>
{
    [Space]
    [SerializeField] protected ChangingType _startRun;
    [Space]
    [SerializeField] protected float _speed = 0.3f;

    private Coroutine _coroutine;
    private Action _callback;

    protected override void Awake()
    {
        _isNotDestroying = false;

        base.Awake();

        StartCoroutine(StartChanging(_startRun, _speed, null));
    }

    public virtual void Disappear(Action callback = null) => StartCoroutine(StartChanging(ChangingType.Disappearances, _speed, callback));
    public virtual void Disappear(float speed, Action callback = null) => StartCoroutine(StartChanging(ChangingType.Disappearances, speed, callback));


    public virtual void Appear(Action callback = null) => StartCoroutine(StartChanging(ChangingType.Appearances, _speed, callback));
    public virtual void Appear(float speed, Action callback = null) => StartCoroutine(StartChanging(ChangingType.Appearances, speed, callback));

    private IEnumerator StartChanging(ChangingType type, float speed, Action callback)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _callback?.Invoke();
            _callback = null;
            yield return null;
        }

        _callback = callback;
        _coroutine = StartCoroutine(Changing(type, speed));
        yield return _coroutine;

        _coroutine = null;
        callback?.Invoke();
    }

    protected abstract IEnumerator Changing(ChangingType type, float speed);
}
