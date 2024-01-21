using System;
using UnityEngine;

public class Bridges : MonoBehaviour
{
    [SerializeField] private Bridge[] _bridges;

    private event Action<BridgeType> EventOpenBridges;
    private event Action EventCloseBridges;

    private void Awake()
    {
        foreach (Bridge b in _bridges)
        {
            EventOpenBridges += b.Open;
            EventCloseBridges += b.Close;
        }
    }

    public void OpenBridges(BridgeType bridgesType) => EventOpenBridges?.Invoke(bridgesType);
    public void CloseBridges() => EventCloseBridges?.Invoke();
}
