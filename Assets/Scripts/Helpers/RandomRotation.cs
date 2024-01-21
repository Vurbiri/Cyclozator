using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [Tooltip("Мин скорость вращения")]
    [SerializeField] private float _minSpeedRotation = 10f;
    [Tooltip("Макс скорость вращения")]
    [SerializeField] private float _maxSpeedRotation = 20f;

    [Tooltip("Мин скорость вращения")]
    [SerializeField] private Vector3 _directions = Vector3.one;
    private Vector3 _rotate;

    private Transform _thisTransform;

    private void Awake() => _thisTransform = transform;
    private void OnEnable() => Setup(_minSpeedRotation, _maxSpeedRotation);
    private void Update() => _thisTransform.Rotate(_rotate * Time.deltaTime);

    public void Setup(float minSpeedRotation, float maxSpeedRotation)
    {
        float x = Random.Range(minSpeedRotation, maxSpeedRotation) * _directions.x;
        float y = Random.Range(minSpeedRotation, maxSpeedRotation) * _directions.y;
        float z = Random.Range(minSpeedRotation, maxSpeedRotation) * _directions.z;
        _rotate = new Vector3(x, y, z);
    }

   



    
}
