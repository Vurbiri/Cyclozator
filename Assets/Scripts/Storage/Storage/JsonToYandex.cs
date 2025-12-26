#if YSDK

using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public class JsonToYandex : JsonTo
{
    private string _key;

#if UNITY_EDITOR
    public override bool IsValid => false;
#else
    public override bool IsValid => YandexSDK.Inst.IsLogOn;
#endif

    public async override UniTask<bool> Initialize(string key)
    {
        _key = key;
        string json;

        try
        {
            json = await YandexSDK.Inst.Load(_key);
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
        if (!((result = SaveSoft(key, data)) && isSaveHard && _dictModified))
        {
            callback?.Invoke(result);
            return;
        }

        SaveToFileAsync(callback).Forget();
    }

    public async UniTaskVoid SaveToFileAsync(Action<bool> callback)
    {
        bool result = true;
        try
        {
            string json = JsonConvert.SerializeObject(_saved);
            await YandexSDK.Inst.Save(_key, json);
            _dictModified = false;
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
#endif