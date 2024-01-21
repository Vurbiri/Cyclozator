using System.Collections.Generic;
using UnityEngine;

public abstract class Pool
{
    protected readonly Stack<PooledObject> _pool = new();
    private readonly PooledObject _prefab;
    protected readonly Transform _container;

    public Pool(PooledObject prefab, Transform container, int size = 0)
    {
        _prefab = prefab;
        _container = container;
        for (int i = 0; i < size; i++)
            CreateObject().Deactivate();
    }


    private void OnDeactivate(PooledObject gameObject) => _pool.Push(gameObject);

    protected virtual PooledObject CreateObject()
    {
        PooledObject gameObject;
        gameObject = GameObject.Instantiate<PooledObject>(_prefab, _container);
        gameObject.EventDeactivate += OnDeactivate;

        return gameObject;
    }
}
