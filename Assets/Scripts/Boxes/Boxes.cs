using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Level;

[RequireComponent(typeof(BoxesRotation))]
public class Boxes : MonoBehaviour
{
    [SerializeField] private Box[] _boxes;
    [SerializeField] private BoxSettings[] _boxesSettings;
    private readonly Dictionary<FigureType, BoxSettings> _dicBoxesSettings = new();

    [Header("Экраны")]
    [Tooltip("Пауза между сменой экрана")]
    [SerializeField] private float _timeChange = 0.36f;
    [Tooltip("Количество повторов")]
    [SerializeField] private int _countChange = 5;

    public event Action EventBlocking;
    public event Action EventUnblocking;
    public event Action EventEndLevel;
    public event Action EventGameOver;
    public event Action<int> EventFigureChange;
    public event Action<CatchingEventArgs> EventCatching;
    public event Action<bool> EventAutoSave;

    public int FiguresCount { get => _figuresCount; private set { _figuresCount = value; EventFigureChange?.Invoke(_figuresCount); }}
    public BoxesRotation Controller { get; private set; }

    private int _figuresCount;
    private int _boxesCount;
    private LBoxes _lBoxes;

    private void Awake()
    {
        Controller = GetComponent<BoxesRotation>();

        foreach (var bs in _boxesSettings)
            _dicBoxesSettings[bs.Type] = bs;
        _boxesSettings = null;

        EventBlocking += Controller.PauseRotation;
        EventUnblocking += Controller.ContinueRotation;
    }

    private void Start()
    {
        foreach (var box in _boxes)
        {
            Controller.EventStartRotation += box.OnStartRotation;
            Controller.EventStopRotation += box.OnStopRotation;

            box.EventBlocking += OnBlocking;
            box.EventCatching += OnCatching;
        }

        EventBlocking?.Invoke();
    }

    public void Setup(float ratioSpeed, Vector3 farOffset)
    {
        Controller.Setup(ratioSpeed);

        foreach (var box in _boxes)
            box.Setup(farOffset);
    }

    public void InitializeLevel(LBoxes boxes, bool isNew)
    {
        _lBoxes = boxes;
        InitializeLevel();
        if (isNew)
            BoxesInitialize();
        else
            StartCoroutine(BoxesSetup(false));

        void InitializeLevel()
        {
            EventBlocking?.Invoke();

            FiguresCount = _lBoxes.FigureCount;
            _boxesCount = _lBoxes.Count;

            foreach (var box in _boxes)
                box.BoxOn();
        }
    }
    
    public void StartLevel() => EventUnblocking?.Invoke();

    public void Pause(bool paused)
    {
        if(paused)
            EventBlocking?.Invoke();
        else if (!IsBlocking())
            EventUnblocking?.Invoke();
    }

    private void BoxesInitialize()
    {
        int typeCount = _lBoxes.Types.Length;
        int remainderBoxesCount = _boxesCount % typeCount;
        int fullBoxesCount = _boxesCount - remainderBoxesCount;
        int indexType = URandom.Range(0, typeCount);
        int index = 0;
        int boxesCount = fullBoxesCount;
        do
        {
            for (; index < boxesCount; index++)
            {
                _lBoxes.Boxes[index] = _lBoxes.Types[indexType].ToInt();
                indexType = _lBoxes.Types.Right(indexType);
            }

            indexType = _lBoxes.Types.Offset(indexType, remainderBoxesCount);
            boxesCount = _boxesCount;

        } while (index < _boxesCount);

        StartCoroutine(BoxesSetup(true));
    }

    private IEnumerator BoxesSetup(bool isNew) 
    {
        WaitForSeconds pause = new(_timeChange);
        int countChange = _countChange;
        int countSettings = _dicBoxesSettings.Count - 1;
        int countBoxes = _lBoxes.Boxes.Length;

        while (countChange-- > 0) 
        { 
            for(int i = countSettings; i >= 1; i--)
            {
                for(int j = 0; j < countBoxes; j++)
                    _boxes[j].SFX.SimpleSetVisualize(_dicBoxesSettings[(FigureType)i]);

                yield return pause;
            }
        }

        for (int i = countSettings; i >= (isNew ? 1 : 0); i--)
        {
            for (int j = 0; j < countBoxes; j++)
            {
                if(i > _lBoxes.Boxes[j])
                    _boxes[j].SFX.SimpleSetVisualize(_dicBoxesSettings[(FigureType)i]);
                else if (i == _lBoxes.Boxes[j])
                    _boxes[j].SFX.SetVisualize(_dicBoxesSettings[(FigureType)i]);
            }

            yield return pause;
        }
    }

    private void OnBlocking(Box box)
    {
        if (!IsBlocking())
            EventBlocking?.Invoke();
       
        box.IsBlocker = true;
    }

    private void OnCatching(object sender, CatchingEventArgs arg) => StartCoroutine(Catching(sender as Box, arg));

    private IEnumerator Catching(Box box, CatchingEventArgs arg)
    {
        if (!BoxGreaterZero()) yield break;

        FiguresCount--;
        bool isEndLevel = _figuresCount == 0;

        EventCatching?.Invoke(arg);

        if (!arg.isAptly)
        {
            int index = Array.IndexOf(_boxes, box);
            if (CheckNotGameOver(index))
            {
                if (!isEndLevel)
                    EventAutoSave?.Invoke(true);
                yield return StartCoroutine(BoxOff(index));
            }
            else
            {
                foreach (var b in _boxes)
                    StartCoroutine(b.SFX.SelfOff(_dicBoxesSettings[FigureType.Off], 0.1f));
                yield break;
            }
        }
        else if (!isEndLevel)
        {
            EventAutoSave?.Invoke(arg.isCoin);
        }

        if (isEndLevel) EventEndLevel?.Invoke();
        if (!IsBlocking() && !isEndLevel) EventUnblocking?.Invoke();

        bool CheckNotGameOver(int box)
        {
            if (!CheckBox(box))
            {
                CheckBoxesLeft(box);
                CheckBoxesRight(box);
            }

            if (BoxGreaterZero()) return true;

            EventBlocking?.Invoke();
            EventGameOver?.Invoke();
            return false;
        }

        bool CheckBox(int box)
        {
            if (_lBoxes.Boxes[box] == 0)
                return false;

            _lBoxes.Boxes[box] = 0;
            _boxesCount--;
            return true;
        }
        void CheckBoxesLeft(int box)
        {
            if (_boxesCount <= 0) return;

            box = _lBoxes.Boxes.Left(box);
            if (!CheckBox(box))
                CheckBoxesLeft(box);
        }
        void CheckBoxesRight(int box)
        {
            if (_boxesCount <= 0) return;

            box = _lBoxes.Boxes.Right(box);
            if (!CheckBox(box))
                CheckBoxesRight(box);
        }

        bool BoxGreaterZero() => _boxesCount > 0;
    }

    private IEnumerator BoxOff(int indexBox, bool isLeft = true, bool isRight = true) 
    {
        Box box = _boxes[indexBox];

        if (box.Type != FigureType.Off)
        {
            yield return StartCoroutine(box.SFX.SelfOff(_dicBoxesSettings[FigureType.Off]));
        }
        else 
        {
            Coroutine pause = null;

            yield return StartCoroutine(box.SFX.LeftRightOff(isLeft, isRight));
            if (isLeft)
                pause = StartCoroutine(BoxOff(_boxes.Left(indexBox), true, false));
            if (isRight)
                pause = StartCoroutine(BoxOff(_boxes.Right(indexBox), false, true));
            yield return pause;
        }
    }

    private bool IsBlocking()
    {
        foreach (var box in _boxes) 
            if(box.IsBlocker) return true;

        return false;
    }
}

