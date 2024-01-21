using System.Collections.Generic;
using UnityEngine;
using static GameStorage;

public class Levels
{
    private readonly List<Level> _levels = new();
    private int _currentIndex = 0;
    private int _maxLevel = 0;

    private float _currentSpeed = 0.4715f;
    private const float stepSpeed = 0.0041f;
    private const float ratioSpeedPhase = 42.24f;
    private float Speed => SpeedGet(1f);

    private float _currentSpawn = 2.6f;
    private const float stepSpawn = -0.025f;
    private const float ratioSpawnPhase = 45.54f;
    private float Spawn => SpawnGet(1f);

    private const int levelInPhase = 45;
    private const int levelInPhaseX2 = levelInPhase * 2;

    private readonly Buff[] _buffs;

    public Levels(Buff[] buffs)
    {
        _buffs = buffs;
        CreateLevels();
        _levels.TrimExcess();
    }
    public Levels(Buff[] buffs, int startLevel) : this(buffs)
    {
        _currentIndex = startLevel - 1;

        while (startLevel > _maxLevel)
        {
            _currentIndex -= _levels.Count;
            CreateLevels();
        }
    }
    public Levels(Buff[] buffs, int startLevel, GameSave gameSave) : this(buffs, startLevel) => _levels[_currentIndex].Modification(gameSave);

    public Level Next()
    {
        if (_currentIndex >= _levels.Count)
        {
            _currentIndex = 0;
            CreateLevels();
        }
        return _levels[_currentIndex++];
    }

    private void CreateLevels()
    {
        _levels.Clear();

        Turn[] turns = { Turn.Degree_22_5, Turn.Degree_67_5, Turn.Degree_45, Turn.Degree_0 };
        float ratioSpeedOneBridge = 3.3f;
        ratioSpeedOneBridge = Mathf.Clamp(ratioSpeedOneBridge - _maxLevel / 22f, 0.1f, ratioSpeedOneBridge);
        float ratioSpawnOneBridge = ratioSpeedOneBridge * 1.5f;

        SpeedGet(ratioSpeedOneBridge);
        SpawnGet(ratioSpawnOneBridge);
        _levels.Add(new(++_maxLevel, BridgeType.Nord, 1, Turn.Degree_0, 5, Speed, Spawn, _buffs)); //01
        _levels.Add(new(++_maxLevel, BridgeType.East, 2, Turn.Degree_67_5, 5, Speed, Spawn, _buffs)); //02
        if (_maxLevel > levelInPhaseX2) 
            _levels.Add(new(++_maxLevel, BridgeType.East, 3, turns[Random.Range(2, 4)], 5, Speed, Spawn, _buffs)); // 03
        else
            _levels.Add(new(++_maxLevel, BridgeType.South, 0, turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //03
        _levels.Add(new(++_maxLevel, BridgeType.East, 2, turns[Random.Range(0, 4)], 6, Speed, Spawn, _buffs)); //04
        _levels.Add(new(++_maxLevel, BridgeType.West, Diff(_maxLevel), turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //05
        SpeedGet(-ratioSpeedOneBridge);
        SpawnGet(-ratioSpawnOneBridge);
        _levels.Add(new(++_maxLevel, BridgeType.Nord | BridgeType.East, 0, turns[Random.Range(0, 2)], 4, Speed, Spawn, _buffs)); //06
        _levels.Add(new(++_maxLevel, BridgeType.Nord | BridgeType.South, 1, turns[Random.Range(2, 4)], 4, Speed, Spawn, _buffs)); //07
        _levels.Add(new(++_maxLevel, Diff(_maxLevel), Diff(_maxLevel), turns[Random.Range(0, 4)], 4, Speed, Spawn, _buffs)); //08
        _levels.Add(new(++_maxLevel, 3, 0, turns[Random.Range(0, 2)], 4, Speed, Spawn, _buffs)); //09
        _levels.Add(new(++_maxLevel, 3, 1, turns[Random.Range(0, 4)], 4, Speed, Spawn, _buffs)); //10
        _levels.Add(new(++_maxLevel, 4, 1, turns[Random.Range(0, 2)], 4, Speed, Spawn, _buffs)); //11
        _levels.Add(new(++_maxLevel, 3, 2, turns[Random.Range(2, 4)], 4, Speed, Spawn, _buffs)); //12
        if (_maxLevel > levelInPhaseX2)
            _levels.Add(new(++_maxLevel, 3, 3, turns[Random.Range(0, 4)], 4, Speed, Spawn, _buffs)); //13
        else
            _levels.Add(new(++_maxLevel, 4, 0, turns[Random.Range(0, 2)], 4, Speed, Spawn, _buffs)); //13
        _levels.Add(new(++_maxLevel, 4, 1, turns[Random.Range(0, 4)], 4, Speed, Spawn, _buffs)); //14
        _levels.Add(new(++_maxLevel, 4, 2, turns[Random.Range(0, 2)], 4, Speed, Spawn, _buffs)); //15
        _levels.Add(new(++_maxLevel, 4, Diff(_maxLevel), turns[Random.Range(0, 4)], 4, Speed, Spawn, _buffs)); //16

        _levels.Add(new(++_maxLevel, Diff(_maxLevel), 0, turns[Random.Range(0, 2)], 5, Speed, Spawn, _buffs)); //17
        _levels.Add(new(++_maxLevel, 3, 0, turns[Random.Range(2, 4)], 5, Speed, Spawn, _buffs)); //18
        _levels.Add(new(++_maxLevel, Diff(_maxLevel), 1, turns[Random.Range(0, 2)], 5, Speed, Spawn, _buffs)); //19
        _levels.Add(new(++_maxLevel, 3, 1, turns[Random.Range(2, 4)], 5, Speed, Spawn, _buffs)); //20
        _levels.Add(new(++_maxLevel, 4, 0, turns[Random.Range(0, 2)], 5, Speed, Spawn, _buffs)); //21
        _levels.Add(new(++_maxLevel, 4, 1, turns[Random.Range(0, 4)], 5, Speed, Spawn, _buffs)); //22
        _levels.Add(new(++_maxLevel, 4, 2, turns[Random.Range(2, 4)], 5, Speed, Spawn, _buffs)); //23
        _levels.Add(new(++_maxLevel, 2, 2, turns[Random.Range(0, 2)], 5, Speed, Spawn, _buffs)); //24
        _levels.Add(new(++_maxLevel, 3, 2, turns[Random.Range(0, 4)], 5, Speed, Spawn, _buffs)); //25
        _levels.Add(new(++_maxLevel, 3, Diff(_maxLevel), turns[Random.Range(0, 2)], 5, Speed, Spawn, _buffs)); //26
        _levels.Add(new(++_maxLevel, 4, 2, turns[Random.Range(0, 2)], 5, Speed, Spawn, _buffs)); //27
        _levels.Add(new(++_maxLevel, 3, 2, turns[Random.Range(0, 4)], 5, Speed, Spawn, _buffs)); //28
        _levels.Add(new(++_maxLevel, 2, 3, turns[Random.Range(0, 4)], _maxLevel < levelInPhase ? 4 : 5, Speed, Spawn, _buffs)); //29
        _levels.Add(new(++_maxLevel, 4, 1, turns[Random.Range(0, 4)], 5, Speed, Spawn, _buffs)); //30
        _levels.Add(new(++_maxLevel, 4, 2, turns[Random.Range(0, 2)], 5, Speed, Spawn, _buffs)); //31
        _levels.Add(new(++_maxLevel, 4, Diff(_maxLevel), turns[Random.Range(2, 4)], 5, Speed, Spawn, _buffs)); //32

        _levels.Add(new(++_maxLevel, BridgeType.Nord | BridgeType.South, 0, turns[Random.Range(2, 4)], 6, Speed, Spawn, _buffs)); //33
        _levels.Add(new(++_maxLevel, BridgeType.South | BridgeType.West, 1, turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //34
        _levels.Add(new(++_maxLevel, 2, Diff(_maxLevel), turns[Random.Range(0, 4)], 6, Speed, Spawn, _buffs)); //35
        _levels.Add(new(++_maxLevel, 3, 0, turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //36
        _levels.Add(new(++_maxLevel, 3, 1, turns[Random.Range(0, 4)], 6, Speed, Spawn, _buffs)); //37
        _levels.Add(new(++_maxLevel, 4, 2, turns[Random.Range(0, 4)], 6, Speed, Spawn, _buffs)); //38
        _levels.Add(new(++_maxLevel, 3, 1, turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //39
        _levels.Add(new(++_maxLevel, 3, 2, turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //40
        _levels.Add(new(++_maxLevel, Diff(_maxLevel), 3, turns[Random.Range(2, 4)], _maxLevel < levelInPhase ? 5 : 6, Speed, Spawn, _buffs)); //41
        _levels.Add(new(++_maxLevel, 4, 0, turns[Random.Range(0, 4)], 6, Speed, Spawn, _buffs)); //42
        _levels.Add(new(++_maxLevel, 4, 1, turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //43
        _levels.Add(new(++_maxLevel, 4, 2, turns[Random.Range(2, 4)], 6, Speed, Spawn, _buffs)); //44
        if (_maxLevel > levelInPhaseX2)
            _levels.Add(new(++_maxLevel, 4, 3, turns[Random.Range(0, 4)], 6, Speed, Spawn, _buffs)); //45
        else
            _levels.Add(new(++_maxLevel, 4, 2, turns[Random.Range(0, 2)], 6, Speed, Spawn, _buffs)); //45
        

        float ratio = _maxLevel / 10f;
        SpeedGet(-Mathf.Clamp(ratioSpeedPhase - ratio, 4f , ratioSpeedPhase));
        SpawnGet(-Mathf.Clamp(ratioSpawnPhase - ratio, 5f, ratioSpawnPhase));

        static int Diff(int lvl) => lvl < levelInPhase ? 2 : 3;
    }

    private float SpeedGet(float x) => _currentSpeed = Mathf.Clamp(_currentSpeed + stepSpeed * x, 0.1f, 1.5f);
    private float SpawnGet(float x) => _currentSpawn = Mathf.Clamp(_currentSpawn + stepSpawn * x, 0.6f, 5f);
}
