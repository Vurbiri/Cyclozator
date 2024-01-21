using UnityEngine;

public class GearRotationSector : GearRotation
{
    [SerializeField] private SectorAnimation _sectorAnimation;
    private void OnEnable()
    {
        _sectorAnimation.EventStartRotation.AddListener(StartRotation);
        _sectorAnimation.EventEnd.AddListener(StopRotation);
    }

    private void OnDisable()
    {
        _sectorAnimation.EventStartRotation.RemoveListener(StartRotation);
        _sectorAnimation.EventEnd.RemoveListener(StopRotation);
    }

}
