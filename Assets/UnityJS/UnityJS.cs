using System.Runtime.InteropServices;

public static class UnityJS
{
    public static bool IsMobile => IsMobileUnityJS();

    public static void Log(string message) => LogJS(message);
    public static void Error(string message) => ErrorJS(message);

    public static bool SetStorage(string key, string data) => SetStorageJS(key, data);
    public static string GetStorage(string key) => GetStorageJS(key);
    public static bool IsStorage() => IsStorageJS();

    public static bool SetCookies(string key, string data) => SetCookiesJS(key, data);
    public static string GetCookies(string key) => GetCookiesJS(key);
    public static bool IsCookies() => IsCookiesJS();


    [DllImport("__Internal")]
    private static extern bool IsMobileUnityJS();
    [DllImport("__Internal")]
    private static extern void LogJS(string msg);
    [DllImport("__Internal")]
    private static extern void ErrorJS(string msg);
    [DllImport("__Internal")]
    private static extern bool SetStorageJS(string key, string data);
    [DllImport("__Internal")]
    private static extern string GetStorageJS(string key);
    [DllImport("__Internal")]
    private static extern bool IsStorageJS();
    [DllImport("__Internal")]
    private static extern bool SetCookiesJS(string key, string data);
    [DllImport("__Internal")]
    private static extern string GetCookiesJS(string key);
    [DllImport("__Internal")]
    private static extern bool IsCookiesJS();

}
