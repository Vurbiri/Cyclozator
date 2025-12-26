using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class JsonToLocalStorage : JsonTo
{
    private string _key;

#if UNITY_EDITOR
    public override bool IsValid => false;
#else
    public override bool IsValid => UnityJS.IsStorage();
#endif

    public async override UniTask<bool> Initialize(string key)
    {
        _key = key;
        string json;

        await UniTask.Delay(0, true);

        try
        {
            json = UnityJS.GetStorage(_key);
        }
        catch (Exception ex)
        {
            json = null;
            Message.Log(ex.Message);
        }

        if (!string.IsNullOrEmpty(json))
        {
            var (result, value) = Deserialize<Dictionary<string, string>>(json);

            if (result)
            {
                _saved = value;
                return true;
            }
        }

        _saved = new();
        return false;
    }

    public override void Save(string key, object data, bool isSaveHard, Action<bool> callback)
    {
        bool result;
        if (!(result = SaveSoft(key, data)) || !isSaveHard)
        {
            callback?.Invoke(result);
            return;
        }

        try
        {
            string json = JsonConvert.SerializeObject(_saved);
            result = UnityJS.SetStorage(_key, json);

        }
        catch (Exception ex)
        {
            result = false;
            Message.Log(ex.Message);
        }
        finally
        {
            callback?.Invoke(result);
        }

    }
}
