using UnityEngine;

public class BoxTriggerFar : BoxTriggerNear
{
    private Transform _thisTransform;
    private GameObject _thisGameObject;

    private void Awake()
    {
        _thisGameObject = gameObject;
        _thisTransform = transform;
    }

    public void Offset(Vector3 offset) => _thisTransform.localPosition = offset;
    public void Activate() => _thisGameObject.SetActive(true);
    public void Deactivate() => _thisGameObject.SetActive(false);

}
