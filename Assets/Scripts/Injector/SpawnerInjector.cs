using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Level;


public class SpawnerInjector : MonoBehaviour
{
    [SerializeField] private Bridge[] _bridges;
    [SerializeField] private Transform _container;
    [SerializeField] private Figure[] _figures;
    [SerializeField] int _sizePool = 5;
    [SerializeField] float _minDistanceForBridge = 1f;

    private readonly Dictionary<FigureType, PoolFigures> _pools = new();

    private bool _isBlockSpawn = false;

    private event Action EventPause;
    private event Action EventContinue;

    private void Awake()
    {
        PoolFigures pool;
        foreach (var f in _figures)
        {
            pool = new(f, _container, _sizePool);
            _pools[f.Type] = pool;
        }
    }

    public void SpawnStart(LSpawn spawn)
    {
        List<Bridge> openBridges = new();

        foreach (var b in _bridges)
        {
            if (b.IsOpen)
                openBridges.Add(b);
        }

        StartCoroutine(Spawn(spawn, openBridges));

    }

    public void PauseSpawn()
    {
        _isBlockSpawn = true;
        EventPause?.Invoke();
    }
    public void ContinueSpawn()
    {
        _isBlockSpawn = false;
        EventContinue?.Invoke();
    }

    private void OnDeactivate(PooledObject gameObject)
    {
        Figure figure = gameObject as Figure;
        EventPause -= figure.PauseMove;
        EventContinue -= figure.ContinueMove;
    }

    private IEnumerator Spawn(LSpawn spawn, List<Bridge> bridges) 
    {
        int indexBridge = 0;
        int indexType = 0;
        int spawnCount = spawn.Count;
        WaitForSeconds pauseMulti = new(spawn.MultiTime);
        WaitForSeconds pauseSpawn = new(spawn.Time);
        WaitWhile pauseBlock = new(() => AllLastSpawnUp());
        float currentSpeed = 1f;
        FigureType currentType = FigureType.Off;
        FigureType prevType = FigureType.Off;
        Bridge currentBridge = bridges[indexBridge];
        float deltaTimeSpawnForBridge;
        Figure figure;
        int coinFigure = spawn.IsCoin ? (URandom.Range(0, spawnCount) + 1) : -1;
        Chance chanceFromOne = new(spawn.ChanceFromOne);
        Chance ChanceJumpSpawn = new(spawn.ChanceJumpSpawn);
        Chance chanceTwoSpawn = new(spawn.ChanceMultiSpawn.two);
        Chance chanceThreeSpawn = new(spawn.ChanceMultiSpawn.three);
        Chance chanceSameFigure = new(spawn.ChanceSameFigure);
        int countMultiSpawn = 0;
        int offsetNextBridge = 0;
        bool isSpawnFromOne = bridges.Count == 1;
        
        while (spawnCount > 0) 
        {
            if (!isSpawnFromOne && spawnCount > 1)
            {
                if (chanceTwoSpawn.Next)
                {
                    countMultiSpawn = 1;
                    if (spawnCount > 2 && chanceThreeSpawn.Next)
                        countMultiSpawn = 2;
                }
            }
            currentSpeed = spawn.Speed * URandom.Range(spawn.UncertaintySpawn.min, spawn.UncertaintySpawn.max);
            do
            {
                if (!isSpawnFromOne)
                {
                    offsetNextBridge = ChanceJumpSpawn.Next ? 2 : 1;
                    currentBridge = bridges.Offset(ref indexBridge, offsetNextBridge);
                }

                prevType = currentType;
                do
                {
                    indexType = URandom.Range(0, spawn.Types.Length);
                    currentType = spawn.Types[indexType];
                }
                while (currentType == prevType && (isSpawnFromOne || !chanceSameFigure.Next));

                deltaTimeSpawnForBridge = currentBridge.LastSpawn - Time.timeSinceLevelLoad;
                if (deltaTimeSpawnForBridge > 0f)
                {
                    foreach (var br in bridges)
                        if(br != currentBridge)
                            br.LastSpawn += deltaTimeSpawnForBridge;
                    yield return new WaitForSeconds(deltaTimeSpawnForBridge);
                }
                yield return pauseBlock;
                currentBridge.LastSpawn = _minDistanceForBridge / currentSpeed + Time.timeSinceLevelLoad;

                figure = _pools[currentType].GetObject(currentBridge.StartPoint) as Figure;
                figure.MoveIt(currentBridge.EndPoint, currentSpeed, coinFigure == spawnCount);
                figure.EventDeactivate += OnDeactivate;

                EventPause += figure.PauseMove;
                EventContinue += figure.ContinueMove;

                --spawnCount;
                --countMultiSpawn;

                yield return pauseMulti;

            } while (countMultiSpawn >= 0);

            isSpawnFromOne = chanceFromOne.Next;

            yield return pauseSpawn;
        }

        bool AllLastSpawnUp()
        {
            foreach (var br in bridges)
                br.LastSpawn += Time.deltaTime;

            return _isBlockSpawn;
        }
    }
}
