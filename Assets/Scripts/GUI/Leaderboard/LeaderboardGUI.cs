using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardGUI : MenuNavigation
{
    [Space]
    [SerializeField] private LeaderboardRecordGUI _record;
    [SerializeField] private GameObject _recordSpace;
    [Space]
    [SerializeField] private ScrollRect _rect;
    [SerializeField] private GameObject _closeMenu;
    [Space]
    [SerializeField] private int[] _coinsReward;
    [Space]
    [Range(1, 20), SerializeField] private int _maxTop = 20;
    [Range(1, 10), SerializeField] private int _maxAround = 10;
    [SerializeField] private string _lbName = "lbCyclozator";

    protected override void OnEnable()
    {
        base.OnEnable();
        if(_closeMenu != null)
            _closeMenu.SetActive(false);
    }

    public void Show()
    {
        if (_rect.content.childCount <= 0)
            Initialize().Forget();
        else
            gameObject.SetActive(true);
    }
    public void Hide() => gameObject.SetActive(false);

    private async UniTaskVoid Initialize()
    {
        if (!YandexSDK.Inst.IsLeaderboard) return;

        int UserRank = 0;
        bool playerInTable = false;
        var (result, value) = await YandexSDK.Inst.GetPlayerResult(_lbName);
        if (result)
        {
            UserRank = value.Rank;
            playerInTable = UserRank > 0;
        }
        if (playerInTable)
            if (UserRank <= (_maxTop - _maxAround))
                playerInTable = false;

        var (res, table) = await YandexSDK.Inst.GetLeaderboard(_lbName, _maxTop, playerInTable, _maxAround);
        if (!res) return;
        if (playerInTable) UserRank = table.UserRank;

        int preRank = 0;
        bool isPlayer;
        RectTransform recordTransform = null;
        RectTransform content = _rect.content;
        LeaderboardRecordGUI recordGUI;

        foreach (var record in table.Table)
        {
            if (record.Rank - preRank > 1)
                Instantiate(_recordSpace, content);
            preRank = record.Rank;

            isPlayer = record.Rank == UserRank;
            recordGUI = Instantiate(_record, _rect.content);
            recordGUI.Setup(record, isPlayer);
            if (isPlayer)
                recordTransform = recordGUI.GetComponent<RectTransform>();
        }

        gameObject.SetActive(true);

        if (recordTransform != null)
        {
            RectTransform viewport = _rect.viewport;
            Canvas.ForceUpdateCanvases();

            float maxOffset = content.rect.height - viewport.rect.height;
            float offset = - viewport.rect.height / 2f - recordTransform.localPosition.y;

            if (offset < 0) offset = 0;
            else if (offset > maxOffset) offset = maxOffset;

            content.localPosition = new Vector2(0, offset);
        }
    }

    public async UniTask<bool> TrySetScoreAndReward(int points, bool isSaveHard)
    {
        if(points <= 0) return false;
        if (!YandexSDK.Inst.IsLeaderboard) return false;

        int maxPoints = 0;
        var (rs, value) = await YandexSDK.Inst.GetPlayerResult(_lbName);
        if (rs)
            maxPoints = value.Score;
        if (maxPoints >= points) return false;

        RectTransform content = _rect.content;
        for (int i = 0; i < content.childCount; i++)
            Destroy(content.GetChild(i).gameObject);

        if (!await YandexSDK.Inst.SetScore(_lbName, (int)points)) return false;

        if(!await Reward())
            Message.Banner(Localization.Inst.GetText("PersonalRecord"));

        return true;

        async UniTask<bool> Reward()
        {
            await UniTask.Delay(250, true);

            var (r, v) = await YandexSDK.Inst.GetPlayerResult(_lbName);
            if (!r) return false;
            if (v.Rank > _coinsReward.Length || v.Rank <= 0) return false;

            int reward = _coinsReward[v.Rank - 1];
            Inventory.Inst.TotalScore.AddCoins(reward);
            Inventory.Inst.Save(isSaveHard, (b) => { if (b) Message.AddCoins(reward, "Prize"); });

            return true;
        }
    }
}
