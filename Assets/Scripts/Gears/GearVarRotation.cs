using System.Collections;
using UnityEngine;

public class GearVarRotation : GearRotation
{
    private float _accelerationTime;
    private float _maxSpeed;
    private float _acceleration;
    private bool isBraking;

    public void Initialize(int accelerationTime)
    {
        _accelerationTime = accelerationTime;
        _acceleration = _speedRotation / _accelerationTime;
        _maxSpeed = _speedRotation;

    }

    public override void StartRotation(DirectRotation direction)
    {
        base.StartRotation(direction);
        
        isBraking = false;
        StartCoroutine(SpeedSetting());
    }

    private IEnumerator SpeedSetting()
    {
        WaitWhile pause = new WaitWhile(() => !isBraking);

        _speedRotation = 0;

        while (_speedRotation < _maxSpeed)
        {
            CalkSpeed(Time.deltaTime);
            yield return null;
        }

        yield return pause;

        yield return null;

        while (_speedRotation > 0)
        {
            CalkSpeed(-Time.deltaTime);
            yield return null;
        }

        _speedRotation = 0;

    }
    private void CalkSpeed(float delta) => _speedRotation +=_acceleration * delta;
    public void Braking() => isBraking = true;
}
