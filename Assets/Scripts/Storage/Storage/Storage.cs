using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.Networking.UnityWebRequest;

public static class Storage
{
    private const string keyGlobalSave = "CZR";
    
    private static ISaveLoad service;

    public static Type TypeStorage => service?.GetType();

    public static bool Create<T>() where T : ISaveLoad, new()
    {
        if (typeof(T) == service?.GetType())
            return true; 

        service = new T();
        return service.IsValid;
    }
   
    public static void Save(string key, object data, bool isSaveHard = true, Action<bool> callback = null) => service.Save(key, data, isSaveHard, callback);
    public static UniTask<bool> SaveAsync(string key, object data, bool isSaveHard = true)
    {
        UniTaskCompletionSource<bool> taskSave = new();
        service.Save(key, data, isSaveHard, (b) => taskSave.TrySetResult(b));
        return taskSave.Task;
    }
    public static (bool result, T value) Load<T>(string key) => service.Load<T>(key);

    public static (bool result, T value) LoadFromResources<T>(string path)
    {
        string text = Resources.Load<TextAsset>(path).text;

        return Deserialize<T>(text);
    }

    public static (bool result, T value) Deserialize<T>(string json)
    {
        (bool, T) result = (false, default);

        try
        {
            result.Item2 = JsonConvert.DeserializeObject<T>(json);
            result.Item1 = true;
        }
        catch (Exception ex)
        {
            Message.Log(ex.Message);
        }

        return result;
    }

    public static async UniTask<(bool result, Texture texture)> TryLoadTextureWeb(string url)
    {
        if (string.IsNullOrEmpty(url))
            return (false, null);

        using var request = UnityWebRequestTexture.GetTexture(url);
        await request.SendWebRequest();

        if (request.result != Result.Success)
            return (false, null);

        return (true, ((DownloadHandlerTexture)request.downloadHandler).texture);
    }

    public static bool StoragesCreate()
    {
#if YSDK
        if (Create<JsonToYandex>())
            return true;
#endif

        if (Create<JsonToLocalStorage>())
            return true;

        if (Create<JsonToCookies>())
            return true;

#if UNITY_EDITOR
        if (Create<JsonToFile>())
            return true;

        if (Create<JsonToPlayerPrefs>())
            return true;
#endif

        Create<EmptyStorage>();
        return false;
    }

    public async static UniTask<bool> Initialize(string key = null)
    {
        if (await service.Initialize(string.IsNullOrEmpty(key) ? keyGlobalSave : key))
        {
            Message.Log("Storage Initialize");

            if (Load())
            {
                Message.Banner(Localization.Inst.GetText("GoodLoad"), time: 2200);
                return true;
            }
            else
            {
                Message.Banner(Localization.Inst.GetText("ErrorLoad"), MessageType.Error);
                return false;
            }
        }

        Message.Log("Storage Not Initialize");

        Inventory.Inst.Create();
        BuffStorage.Inst.Create();
        SettingsStorage.Inst.Create();
        GameStorage.Inst.Create();
#if YSDK
        YMoney.Inst.Create();
#endif

        return false;

        //============== local func ===============================
        static bool Load()
        {
            bool result = false;

            result |= Loading(Inventory.Inst);
            result |= Loading(SettingsStorage.Inst);
            result |= Loading(BuffStorage.Inst);
            result |= Loading(GameStorage.Inst);
#if YSDK
            result |= Loading(YMoney.Inst);
#endif

            return result;
        }

        static bool Loading(ILoading instantiate)
        {
            if (instantiate.Load())
                return true;
            
            instantiate.Create();
            return false;
        }
    }

    //public async static UniTask<(bool result, T value)> LoadFromResourcesAsync<T>(string path)
    //{
    //    TextAsset textAsset = await Resources.LoadAsync<TextAsset>(path) as TextAsset;

    //    return await UniTask.RunOnThreadPool(() => service.Deserialize<T>(textAsset.text)); ;
    //}

    //public static bool Create(Type typeStorage)
    //{
    //    if (typeStorage == TypeStorage)
    //        return true;

    //    object obj = Activator.CreateInstance(typeStorage);
    //    service = obj as ISaveLoad;
    //    if (service == null)
    //    {
    //        return false;
    //    }
    //    return service.IsValid;
    //}
}
