using System;
using System.Collections;
using UnityEngine;

public class InjectorRotation : MonoBehaviour
{
    [Tooltip("В градусах/сек")]
    [SerializeField] private Turn _maxSpeed = Turn.Degree_135;
    [Tooltip("В секундах")]
    [SerializeField] private int _accelerationTime = 4;
    [Range(2, 10)]
    [SerializeField] private int _turnCount = 3;

    private float _acceleration;
    private float _speed;
    private float MaxSpeed => _maxSpeed.ToDegree();
    private float _minSpeed = 2f;    // из-за погрешности скорость может упасть до 0 раньше чем пройден путь
    public int AccelerationTime => _accelerationTime;

    public event Action EventStartRotation;
    public event Action EventBrakingRotation;
    public event Action EventStopRotation;

    private Transform _thisTransform;

    private readonly Quaternion[] _arrayRotation = Helper.ArrayAngles;
    private Turn _turnTarget = Turn.Degree_0;
    private Turn _newTurn = Turn.Degree_0;

    private void Awake()
    {
        _thisTransform = transform;
        _thisTransform.rotation = _arrayRotation[_turnTarget.ToInt()];

        _acceleration = MaxSpeed / (float)_accelerationTime;
    }

    public void StartRotation(Turn newTurn)
    {
        _newTurn = newTurn;
        StartCoroutine(StartRotation());
    }

    private IEnumerator StartRotation()
    {
        int turnBraking = _maxSpeed.ToInt() * _accelerationTime / 2;    // расстояние которое пройдет за время _accelerationTime при падении скорости от _maxSpeed до 0
        int turnCount = CalkTurns(turnBraking);

        EventStartRotation?.Invoke();

        yield return StartCoroutine(Move(turnCount, Accelerating));

        EventBrakingRotation?.Invoke();

        yield return StartCoroutine(Move(turnBraking, Decelerating));

        _speed = 0;

        EventStopRotation?.Invoke();

    }

    private IEnumerator Move(int turnCount, Func<float, float> calkSpeed)
    {
        int turn = Turn.Degree_135.ToInt(); // за раз повернем на этот градус
        while (turnCount > turn)
        {
            turnCount -= turn;
            yield return StartCoroutine(Rotate(turn, calkSpeed));
        }
        yield return StartCoroutine(Rotate(turnCount, calkSpeed));
    }

    private IEnumerator Rotate(int turn, Func<float,float> calkSpeed)
    {
        Quaternion current = _thisTransform.rotation;
        Quaternion target = _arrayRotation[_turnTarget.Offset(turn)];

        while (current != target)
        {
            _thisTransform.rotation = current.RotateTowards(target, _speed * Time.deltaTime);
            _speed = calkSpeed(Math.Abs(Quaternion.Angle(current, target)));
            yield return null;
        }
    }

    private int CalkTurns(int turnBraking)
    {
        int count = Enum<Turn>.Count;

        int index = _turnCount * count;
        index += count - _turnTarget.ToInt();       // ставим на 0
        index += _newTurn.ToInt() - turnBraking;    // оставляем путь для торможения
        return index;
    }

    private float Accelerating(float angle)
    {
        float maxSpeed = MaxSpeed;
        return (_speed < maxSpeed) ? _speed + _acceleration * Time.deltaTime : maxSpeed;
    }
    private float Decelerating(float angle) => (_speed > _minSpeed) ? _speed - _acceleration * Time.deltaTime : _minSpeed + angle * 2f;
}
