using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using static GameStorage;

[RequireComponent(typeof(GameSFX))]
public class Game : Singleton<Game>
{
    [SerializeField] private Injector _injector;
    [SerializeField] private Boxes _boxes;
    [SerializeField] private Sectors _sectors;
    private GameSFX _gameSFX;
    [Space]
    [SerializeField] private float _pauseBeforeStart = 4f;
    [SerializeField] private float _pauseLevelUP = 1.5f;
    [Space]
    [SerializeField] private float _ratioSpeedBoxesRotation = 1f;
    [SerializeField] private float _farOffsetBoxes = 0f;
    [Space]
    [SerializeField] private GameObject _menuGameOver;
    [SerializeField] private LeaderboardGUI _leaderboard;
    [Space]
    [SerializeField] private int _countFiguresForSave = 3;
    private int _currentFiguresForSave = 0;

    public float PauseBeforeStart => _pauseBeforeStart;
    public ScoreGame Score { get; private set; }
    public bool IsPause { get; private set; } = false;

    public event Action<int> EventNewLevel;
    public event Action<int> EventFigureChange;
    public event Action EventGameOver;
    public event Action<bool> EventPause;

    public GameMode Mode { get; private set; } = GameMode.None;
    private Levels _levels;
    private Level _currentLevel;
    private Buff[] _buffs;

    private WaitForSeconds _pauseCoroutine;

    protected override void Awake()
    {
        _isNotDestroying = false;
        
        base.Awake();
        
        UnityEngine.Random.InitState(Helper.Seed);
        _gameSFX = GetComponent<GameSFX>();
    }

    private void Start()
    {
        _pauseCoroutine = new(_pauseBeforeStart);
        _menuGameOver.SetActive(false);
        _leaderboard.Hide();

        Setup();
        GameModeStart state = GameStorage.Inst.ModeStart;
        int multiplierForLevel = 1;
        multiplierForLevel = _buffs.Buffing(ItemType.Points, multiplierForLevel);

        if (state == GameModeStart.New)
            StartCoroutine(NewGame(multiplierForLevel));
        else if (state == GameModeStart.ContinueSimple)
            StartCoroutine(ContinueSimple(multiplierForLevel));
        else if (state == GameModeStart.Continue)
            StartCoroutine(Continue(multiplierForLevel));

        void Setup()
        {
            _injector.EventStartLevel += StartLevel;

            _boxes.EventBlocking += _injector.OnPause;
            _boxes.EventUnblocking += _injector.OnContinue;
            _boxes.EventCatching += OnScoreChange;
            _boxes.EventEndLevel += OnEndLevel;
            _boxes.EventGameOver += () => OnGameOver();
            _boxes.EventFigureChange += (f) => EventFigureChange?.Invoke(f);
            _boxes.EventAutoSave += AutoSave;

            EventPause += _boxes.Pause;

            _buffs = BuffStorage.Inst.BuffsArray;
            _ratioSpeedBoxesRotation = _buffs.Buffing(ItemType.RotSpeed, _ratioSpeedBoxesRotation);
            _farOffsetBoxes = _buffs.Buffing(ItemType.Catch, _farOffsetBoxes);

            _boxes.Setup(_ratioSpeedBoxesRotation, new(0f, 0f, _farOffsetBoxes));
        }
    }

    public void Pause(bool isPause)
    {
        if (IsPause == isPause)
            return;

        IsPause = isPause;
        EventPause?.Invoke(IsPause);

        if (IsPause)
            Time.timeScale = .0000000001f;
        else
            Time.timeScale = 1f;
   
    }
    public void Save(bool isSaveHard = true, Action<bool> callback = null)
    {
        if (Mode == GameMode.Initialize)
            GameStorage.Inst.SimpleSaveGame(Score, _currentLevel.NumberLevel, isSaveHard, callback);
        else if (Mode == GameMode.Gameplay)
            GameStorage.Inst.SaveGame(Score, _currentLevel.NumberLevel, _currentLevel.Spawn.Types, _currentLevel.Bridges, _boxes.FiguresCount, _currentLevel.Boxes.Boxes, isSaveHard, callback);
        else
            callback?.Invoke(true);
    }

    private void AutoSave(bool isSave)
    {
        if (_currentFiguresForSave++ < _countFiguresForSave && !isSave)
            return;

        Save();
        _currentFiguresForSave = 0;
    }

    public void RotationLeft() => _boxes.Controller.Rotation(DirectRotation.Left);
    public void RotationRight() => _boxes.Controller.Rotation(DirectRotation.Right);


    private IEnumerator NewGame(int ratio)
    {
        Score = new(ratio);
        _levels = new(_buffs);

        yield return _pauseCoroutine;
        InitializeLevel(true);
    }
    private IEnumerator ContinueSimple(int ratio)
    {
        GameSimpleSave simpleSave = GameStorage.Inst.SimpleSave;
        
        if (simpleSave == null)
        {
            StartCoroutine(NewGame(ratio)); 
            yield break;
        }

        Score = new(simpleSave.A, simpleSave.B, ratio);
        _levels = new(_buffs, simpleSave.Level);

        yield return _pauseCoroutine;
        InitializeLevel(true, true);
    }
    private IEnumerator Continue(int ratio)
    {
        GameSave save = GameStorage.Inst.Save;
        if (save == null)
        {
            StartCoroutine(ContinueSimple(ratio));
            yield break; 
        }
        GameSimpleSave simpleSave = GameStorage.Inst.SimpleSave;
        if (simpleSave == null)
        {
            StartCoroutine(NewGame(ratio));
            yield break; 
        }

        Score = new(simpleSave.A, simpleSave.B, ratio);
        _levels = new(_buffs, simpleSave.Level, save);

        yield return _pauseCoroutine;
        InitializeLevel(false, true);
    }

    private void InitializeLevel(bool isNew, bool isFromSave = false)
    {
        _currentLevel = _levels.Next();
        Mode = GameMode.Initialize;

        if (isNew && !isFromSave && _currentLevel.NumberLevel > 1)
            AutoSave(true);

        EventNewLevel?.Invoke(_currentLevel.NumberLevel);

        _injector.InitializeLevel(_currentLevel.Bridges, _currentLevel.Spawn);
        _sectors.InitializeLevel(_currentLevel.Sectors);
        _boxes.InitializeLevel(_currentLevel.Boxes, isNew);
 
    }

    private void StartLevel()
    {
        Mode = GameMode.Gameplay;
        _boxes.StartLevel();
    }

    private void OnEndLevel()
    {
        Score.ScoreUpPerLevel(_currentLevel.NumberLevel);
        Mode = GameMode.None;
        StartCoroutine(LevelUp());
    }

    private void OnScoreChange(CatchingEventArgs arg)
    {
        if (arg.isAptly)        
            Score.ScoreUpPerCatching(arg.isCoin);
        else
            Score.ScoreDownPerMissing(_currentLevel.NumberLevel, arg.isCoin);
    }
    
    private IEnumerator LevelUp()
    {
        yield return new WaitForSeconds(_pauseLevelUP);
        yield return StartCoroutine(_gameSFX.PlayLevelUP());

        InitializeLevel(true);
    }

    public void OnGameOver()
    {
        Time.timeScale = .0000000001f;
        RewardAndGameOver().Forget();

        async UniTaskVoid RewardAndGameOver()
        {
            Mode = GameMode.None;
            _gameSFX.PlayGameOver();
            BuffStorage.Inst.ResetBuffs(false);
            bool isRecord = await _leaderboard.TrySetScoreAndReward((int)Score.Points, false);
            GameStorage.Inst.ResetGame(Score);
            EventGameOver?.Invoke();
            Time.timeScale = 1f;
            if (isRecord)
                _leaderboard.Show();
            else
                _menuGameOver.SetActive(true);
        }
    }
}
