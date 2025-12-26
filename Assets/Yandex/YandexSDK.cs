#if YSDK

using Cysharp.Threading.Tasks;
using System;

public partial class YandexSDK
{
    private static readonly YandexSDK s_instance = new();

	public static YandexSDK Inst => s_instance;

	private YandexSDK() { }

#if !UNITY_EDITOR
	public bool IsInitialize => IsInitializeJS();
	public bool IsPlayer => IsPlayerJS();
	public bool IsLogOn => IsLogOnJS();
	public bool IsLeaderboard => IsLeaderboardJS();
	public bool IsDesktop => IsDesktopJS();
	public bool IsMobile => IsMobileJS();

	public string PlayerName => GetPlayerNameJS();
	public UniTask<(bool result, Texture texture)> GetPlayerAvatar(AvatarSize size)
	{
		string url = GetPlayerAvatarURLJS(size.ToString().ToLower());
		return Storage.TryLoadTextureWeb(url);
	}
	public string PlayerId => GetPlayerIdJS();
	public string Lang => GetLangJS();

	public UniTask<bool> InitYsdk() => WaitTask(ref taskEndInitYsdk, InitYsdkJS);
	public void LoadingAPI_Ready() => ReadyJS();
	public UniTask<bool> InitPlayer() => WaitTask(ref taskEndInitPlayer, InitPlayerJS);
	public UniTask<bool> LogOn() => WaitTask(ref taskEndLogOn, LogOnJS);

	public UniTask<bool> InitLeaderboards() => WaitTask(ref taskEndInitLeaderboards, InitLeaderboardsJS);
	public async UniTask<(bool result, LeaderboardResult lbResult)> GetPlayerResult(string lbName) 
	{
		string json = await WaitTask(ref taskEndGetPlayerResult, GetPlayerResultJS, lbName);
		taskEndGetPlayerResult = null;
		if (string.IsNullOrEmpty(json))
			return (false, null);
		else
			return Storage.Deserialize<LeaderboardResult>(json);
	}
	public UniTask<bool> SetScore(string lbName, int score) => WaitTask(ref taskEndSetScore, SetScoreJS, lbName, score); 
	public async UniTask<(bool result, Leaderboard table)> GetLeaderboard(string lbName, int quantityTop, bool includeUser = false, int quantityAround = 1, AvatarSize size = AvatarSize.Small)
	{
		taskEndGetLeaderboard.TrySetResult(default);
		taskEndGetLeaderboard = new();
		GetLeaderboardJS(lbName, quantityTop, includeUser, quantityAround, size.ToString().ToLower());
		string json = await taskEndGetLeaderboard.Task;
		taskEndGetLeaderboard = null;
		return Storage.Deserialize<Leaderboard>(json);
	}

	public UniTask<bool> Save(string key, string data) => WaitTask(ref taskEndSave, SaveJS, key, data); 
	public UniTask<string> Load(string key) => WaitTask(ref taskEndLoad, LoadJS, key);

	public UniTask<bool> CanReview() => WaitTask(ref taskEndCanReview, CanReviewJS);
	public UniTask<bool> RequestReview() => WaitTask(ref taskEndRequestReview, RequestReviewJS);

	public UniTask<bool> CanShortcut() => WaitTask(ref taskEndCanShortcut, CanShortcutJS);
	public UniTask<bool> CreateShortcut() => WaitTask(ref taskEndCreateShortcut, CreateShortcutJS);
#endif
    public static UniTask<T> WaitTask<T>(ref UniTaskCompletionSource<T> taskCompletion, Action action)
	{
		taskCompletion?.TrySetResult(default);
		taskCompletion = new();
		action();
		return taskCompletion.Task;
	}
	public static UniTask<T> WaitTask<T, U>(ref UniTaskCompletionSource<T> taskCompletion, Action<U> action, U value)
	{
		taskCompletion?.TrySetResult(default);
		taskCompletion = new();
		action(value);
		return taskCompletion.Task;
	}
	public static UniTask<T> WaitTask<T, U, V>(ref UniTaskCompletionSource<T> taskCompletion, Action<U, V> action, U value1, V value2)
	{
		taskCompletion?.TrySetResult(default);
		taskCompletion = new();
		action(value1, value2);
		return taskCompletion.Task;
	}
}
#endif