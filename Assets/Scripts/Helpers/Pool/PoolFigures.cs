using System.Collections.Generic;
using UnityEngine;

public class PoolFigures : Pool
{
    public PoolFigures(PooledObject prefab, Transform container, int size = 0) : base(prefab, container, size)
    {
    }

    public PooledObject GetObject(Vector3 position)
    {
        if (!_pool.TryPop(out PooledObject gameObject))
            gameObject = CreateObject();

        gameObject.ThisTransform.position = position;
        gameObject.Activate();

        return gameObject;
    }

}
