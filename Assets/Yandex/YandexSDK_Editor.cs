#if UNITY_EDITOR && YSDK

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public partial class YandexSDK
{
	public bool IsInitialize => true;
	public bool IsPlayer => true;  
	public bool IsLeaderboard => true;
	public string PlayerName => "";
	public string PlayerId => "tOpLpSh7i8QG8Voh/SuPbeS4NKTj1OxATCTKQF92H4c=";
	public bool IsLogOn { set; get; } = false;
	public string Lang => "ru";

	public bool IsDesktop => false;
	public bool IsMobile => true;

	public UniTask<bool> InitYsdk() => UniTask.RunOnThreadPool(() => true);
	public void LoadingAPI_Ready() { }
	public UniTask<bool> InitPlayer() => UniTask.RunOnThreadPool(() => true);
	public async UniTask<bool> LogOn()
	{
		await UniTask.Delay(1000, true);
		IsLogOn = true;
		return true;
	}
	public UniTask<bool> InitLeaderboards() => UniTask.RunOnThreadPool(() => false);
	public UniTask<(bool result, Texture texture)> GetPlayerAvatar(AvatarSize size) => UniTask.RunOnThreadPool<(bool result, Texture texture)>(() => (false, null));

	public UniTask<(bool result, LeaderboardResult lbResult)> GetPlayerResult(string lbName) => UniTask.RunOnThreadPool(() => (true, new LeaderboardResult(2,3010)));
	public UniTask<bool> SetScore(string lbName, int score) => UniTask.RunOnThreadPool(() => true);
	public UniTask<(bool result, Leaderboard table)> GetLeaderboard(string lbName, int quantityTop, bool includeUser = false, int quantityAround = 0, AvatarSize size = AvatarSize.Small)
	{
		List<LeaderboardRecord> list = new()
		{
			new(1, 1100, "Седов Герман", ""),
			new(2, 1000, "Журавлев Тимофей", ""),
			new(3, 900, "Крылов Богдан", ""),
			new(4, 800, "Панов Фёдор", ""),
			new(5, 600, "Зайцев Илья", ""),
			new(6, 550, "Лебедева Алёна", ""),
			new(8, 500, "", ""),
			new(9, 400, "Муравьев Егор", ""),
			new(10, 300, "Казанцев Алексей", ""),
			new(11, 200, "Баженов Борис", ""),
			new(12, 100, "Крылова Таня", "")
		};

		Leaderboard l = new(2, list.ToArray());

		return UniTask.RunOnThreadPool(() => (true, l));
	}

	public UniTask<bool> Save(string key, string data) => UniTask.RunOnThreadPool(() => false);
	public UniTask<string> Load(string key) => UniTask.RunOnThreadPool(() => string.Empty);

	public UniTask<bool> CanReview() => UniTask.RunOnThreadPool(() => false);
	public UniTask<bool> RequestReview() => UniTask.RunOnThreadPool(() => false);

	public UniTask<bool> CanShortcut() => UniTask.RunOnThreadPool(() => true);
	public UniTask<bool> CreateShortcut() => UniTask.RunOnThreadPool(() => true);

}
#endif
