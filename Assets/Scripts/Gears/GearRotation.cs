using System.Collections;
using UnityEngine;

public class GearRotation : MonoBehaviour
{

    [SerializeField] protected float _speedRotation;
    private bool _isRotation;

    private Transform _thisTransform;

    private void Awake() => _thisTransform = transform;

    public virtual void StartRotation(DirectRotation direction)
    {
        if (_isRotation) return;

        _isRotation = true;
        StartCoroutine(Rotation(direction));
    }

    public void StopRotation() => _isRotation = false;

    private IEnumerator Rotation(DirectRotation direction)
    {
        while (_isRotation)
        {
            _thisTransform.Rotate(_speedRotation * direction.ToInt() * Time.deltaTime * Vector3.up);
            yield return null;
        }
    }

}
