using System;
using UnityEngine;
using static Level;


[RequireComponent(typeof(GearsInjector), typeof(SpawnerInjector))]
public class Injector : MonoBehaviour
{
    [SerializeField] private InjectorRotation _injRotation;
    [SerializeField] private Bridges _bridges;
    private GearsInjector _gears;
    private SpawnerInjector _spawner;

    private LBridges _currentBridges;
    private LSpawn _currentSpawn;

    public event Action EventStartLevel;

    private void Awake()
    {
        _gears = GetComponent<GearsInjector>();
        _spawner = GetComponent<SpawnerInjector>();

        _gears.Initialize(_injRotation.AccelerationTime);

        _injRotation.EventStartRotation += _bridges.CloseBridges;
        _injRotation.EventStartRotation += _gears.StartAllGears;

        _injRotation.EventBrakingRotation += () => { _bridges.OpenBridges(_currentBridges.Open); };
        _injRotation.EventBrakingRotation += _gears.BrakingAllGears;

        _injRotation.EventStopRotation += _gears.StopAllGears;
        _injRotation.EventStopRotation += StartLevel;
    }

    public void InitializeLevel(LBridges bridges, LSpawn spawner)
    {
        _currentBridges = bridges;
        _currentSpawn = spawner;

        _injRotation.StartRotation(bridges.Turn);
    }

    private void StartLevel()
    {
        EventStartLevel?.Invoke();
        _spawner.SpawnStart(_currentSpawn);
    }

    public void OnPause() => _spawner.PauseSpawn();
    public void OnContinue() => _spawner.ContinueSpawn();

}
