using Cysharp.Threading.Tasks;
using System;

public interface ISaveLoad
{
    public bool IsValid { get; }
    public UniTask<bool> Initialize(string key);
    public void Save(string key, object data, bool isSaveHard, Action<bool> callback);
    public (bool result, T value) Load<T>(string key);
}
