using Cysharp.Threading.Tasks;
using System;

public class EmptyStorage : ISaveLoad
{
    public bool IsValid => true;

    public async UniTask<bool> Initialize(string key)
    {
        await UniTask.Delay(0, true);
        return false;
    }

    public (bool result, T value) Load<T>(string key) => (false, default);
   public void Save(string key, object data, bool isSaveHard, Action<bool> callback) => callback?.Invoke(false);

}
