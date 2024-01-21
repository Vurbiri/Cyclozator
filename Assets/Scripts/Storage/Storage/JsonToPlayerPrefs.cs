#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonToPlayerPrefs : JsonTo
{
    private string _key;

    public override bool IsValid => true;

    public async override UniTask<bool> Initialize(string key)
    {
        _key = key;

        if (PlayerPrefs.HasKey(_key))
        {
            string json = PlayerPrefs.GetString(_key);

            if (json != null)
            {
                var (result, value) = Deserialize<Dictionary<string, string>>(json);

                if (result)
                {
                    _saved = value;
                    return await UniTask.RunOnThreadPool(() => true);
                }
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
            PlayerPrefs.SetString(_key, json);
            PlayerPrefs.Save();

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