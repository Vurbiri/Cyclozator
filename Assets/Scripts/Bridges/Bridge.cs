using UnityEngine;

[RequireComponent(typeof(BridgeSFX))]
public class Bridge : MonoBehaviour
{
    [SerializeField] private BridgeType _bridgeType;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    public Vector3 StartPoint => _startPoint.position;
    public Vector3 EndPoint => _endPoint.position;

    public float LastSpawn { get; set; }

    private BridgeSFX _bridgeSFX;

    private void Awake() => _bridgeSFX = GetComponent<BridgeSFX>();

    public void Open(BridgeType type)
    {
        bool isThisBridge = (type & _bridgeType) == _bridgeType;
        if (!isThisBridge || _isOpen) 
            return;

        _isOpen = true;
        _bridgeSFX.Open();
    }

    public void Close()
    {
        if (!_isOpen) return;

        _isOpen = false;
        _bridgeSFX.Close();
    }
}

