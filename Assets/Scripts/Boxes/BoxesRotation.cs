using System;
using System.Collections;
using UnityEngine;

public class BoxesRotation : MonoBehaviour
{
    [SerializeField] private Gears _gears;
    [SerializeField] private float _speed = 50f;

    private DirectRotation _direction = DirectRotation.None;
    private bool _isBlockRotation = false;

    private readonly Quaternion[] _arrayRotation = Helper.ArrayAngles;
    private Turn _turnTarget = Turn.Degree_0;
    private Transform _thisTransform;
    private Coroutine _coroutine;

    public event Action<DirectRotation> EventStartRotation;
    public event Action EventStopRotation;

    private void Awake()
    {
        _thisTransform = transform;
        _thisTransform.rotation = _arrayRotation[_turnTarget.ToInt()];

        EventStartRotation += _gears.StartAllGears;
        EventStopRotation += _gears.StopAllGears;
    }

    public void Setup(float ratioSpeed) => _speed *= ratioSpeed;
    
    public void PauseRotation() => _isBlockRotation = true;
    public void ContinueRotation() => _isBlockRotation = false;

    public void Rotation(DirectRotation direction)
    {
        if (_isBlockRotation || _direction == direction) 
            return;

        _direction = direction;
        if (_coroutine != null)
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(Rotate());

    }

    private IEnumerator Rotate()
    {
        Quaternion current = _thisTransform.rotation;
        Quaternion target = _arrayRotation[_turnTarget.Offset(_direction)];

        EventStartRotation?.Invoke(_direction);

        while (current != target)
        {
            _thisTransform.rotation = current.RotateTowards(target, _speed * Time.deltaTime);
            yield return null;
        }

        _direction = DirectRotation.None;
        StartCoroutine(EndRotation());
    }

    private IEnumerator EndRotation()
    {
        yield return null;
        yield return null;
        if (_direction == DirectRotation.None)
            EventStopRotation?.Invoke();
    }
}
