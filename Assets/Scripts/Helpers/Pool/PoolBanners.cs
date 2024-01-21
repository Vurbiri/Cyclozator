using UnityEngine;

public class PoolBanners : Pool
{
    public PoolBanners(PooledObject prefab, Transform container, int size = 0) : base(prefab, container, size)
    {
    }

    public Banner GetObject()
    {
        if (!_pool.TryPop(out PooledObject gameObject))
            gameObject = CreateObject();
        
        return (gameObject as Banner);
    }


}
