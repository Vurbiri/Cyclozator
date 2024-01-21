#if UNITY_EDITOR

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
        List<LeaderboardRecord> list = new();
        list.Add(new(1, 1100, "����� ������", ""));
        list.Add(new(2, 1000, "�������� �������", ""));
        list.Add(new(3, 900, "������ ������", ""));
        list.Add(new(4, 800, "����� Ը���", ""));
        list.Add(new(5, 600, "������ ����", ""));
        list.Add(new(6, 550, "�������� ����", ""));
        list.Add(new(8, 500, "", ""));
        list.Add(new(9, 400, "�������� ����", ""));
        list.Add(new(10, 300, "�������� �������", ""));
        list.Add(new(11, 200, "������� �����", ""));
        list.Add(new(12, 100, "������� ����", ""));

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
