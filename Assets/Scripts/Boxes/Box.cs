using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxSFX))]
public class Box : MonoBehaviour
{
    public FigureType Type => SFX.Type;

    [SerializeField] private BoxTriggerFar _triggerFar;
    [SerializeField] private BoxTriggerNear _triggerNear;
    public BoxSFX SFX { get; private set; }
    public bool IsBlocker { get; set; } = false;

    public event Action<Box> EventBlocking;
    public event EventHandler<CatchingEventArgs> EventCatching;


    private readonly Dictionary<int, Figure> _figuresFar = new();
    private Figure _figureNear;
    private bool _isStop = true;

    private void Awake()
    {
        SFX = GetComponent<BoxSFX>();

        _triggerFar.EventTriggerEnter += AddFigureFar;
        _triggerFar.EventTriggerExit += RemoveFigureFar;

        _triggerNear.EventTriggerEnter += AddFigureNear;
        _triggerNear.EventTriggerExit += RemoveFigureNear;
    }

    public void OnStartRotation(DirectRotation d) => _isStop = false;
    public void OnStopRotation()
    {
        _isStop = true;

        CatchFarFigures();
        CatchNearFigures();
    }

    public void Setup(Vector3 offset) => _triggerFar.Offset(offset);

    public void BoxOn()
    {
        _isStop = true;
        IsBlocker = false;
        _triggerFar.Activate();
    }

    private void AddFigureFar(Figure figure)
    {
        if (Type != figure.Type) return;

        if (_isStop)
            PreCatchFiguresForFar(figure);
        else
            _figuresFar[figure.GetInstanceID()] = figure;
    }

    private void RemoveFigureFar(Figure figure)
    {
        _figuresFar.Remove(figure.GetInstanceID());
        IsBlocker = false;
    }

    private void AddFigureNear(Figure figure)
    {
        if (Type == figure.Type) return;

        EventBlocking?.Invoke(this);
        _triggerFar.Deactivate();

        if (_isStop)
            CatchFigure(figure, false);
        else
            _figureNear = figure;
    }

    private void RemoveFigureNear(Figure figure)
    {
        _figureNear = null;
        IsBlocker = false;
        _triggerFar.Activate();
    }

    private void CatchFarFigures()
    {
        foreach (var f in _figuresFar.Values)
            PreCatchFiguresForFar(f);
        
        _figuresFar.Clear();
    }

    private void CatchNearFigures()
    {
        if (_figureNear == null) return;
        
        CatchFigure(_figureNear, false);
        _figureNear = null;
    }

    private void PreCatchFiguresForFar(Figure figure)
    {
        EventBlocking?.Invoke(this);
        CatchFigure(figure, true);
    }

    private void CatchFigure(Figure figure, bool isAptly)
    {
        StartCoroutine(figure.Catch(isAptly));
        SFX.CatchStart(figure.ParticleForceField, isAptly);
        figure.EventDeactivate += RemoveFigure;
    }

    private void RemoveFigure(PooledObject _pObj)
    {
        Figure figure = _pObj as Figure;
        bool isAptly = Type == figure.Type;
        
        if (isAptly)
            SFX.ScoreUP();

        SFX.CatchEnd(figure.ParticleForceField);
        figure.EventDeactivate -= RemoveFigure;
        IsBlocker = false;
        EventCatching?.Invoke(this, new (isAptly, figure.IsCoin));
    }

    public override bool Equals(object other) => this.GetInstanceID() == (other as Box).GetInstanceID();
    public static bool operator == (Box box1, Box box2) => box1.Equals(box2);
    public static bool operator !=(Box box1, Box box2) => !box1.Equals(box2);
    public override int GetHashCode() => this.GetInstanceID();
}
