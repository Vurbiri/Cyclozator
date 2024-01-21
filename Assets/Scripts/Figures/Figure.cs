using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystemForceField))]
public class Figure : PooledObject
{
    [SerializeField] private FigureType _type;
    [SerializeField] private CoinSFX _coin;
    [SerializeField] private float _speedCatch = 1f;
    [SerializeField] private float _pauseCatch = 0.2f;

    private ParticleSystemForceField _psForceField;
    public ParticleSystemForceField ParticleForceField => _psForceField;
    public FigureType Type => _type;

    private bool _isBlockMove = false;
    private bool _isCatch = false;
    private float _speed;
    public bool IsCoin { get; private set; }


    protected override void Awake()
    {
        base.Awake();
        _psForceField = GetComponent<ParticleSystemForceField>();
    }

    public void MoveIt(Vector3 endPoint, float speed, bool isCoin = false)
    {
        _speed = speed;
        _coin.gameObject.SetActive(isCoin);
        IsCoin = isCoin;
        StartCoroutine(Move(endPoint));
    }

    public IEnumerator Catch(bool isAptly)
    {

        if (IsCoin)
        {
            CoinRemove(isAptly);
            yield return new WaitForSeconds(_pauseCatch);
        }

        yield return null;

        _speed = _speedCatch;
        _isCatch = true;
    }

    private void CoinRemove(bool isAptly)
    {
        if (isAptly) _coin.Catch();
        else _coin.Breaking();
    }

    public void PauseMove() => _isBlockMove = true;
    public void ContinueMove() => _isBlockMove = false;

    private IEnumerator Move(Vector3 endPoint)
    {
        Vector3 current = ThisTransform.position;

        while (current != endPoint) 
        {
            if (!_isBlockMove || _isCatch)
                ThisTransform.position = current.MoveTowards(endPoint, _speed * Time.deltaTime);
            yield return null;
        }

        _isCatch = false;
        _isBlockMove = false;
        Deactivate();
    }

}
