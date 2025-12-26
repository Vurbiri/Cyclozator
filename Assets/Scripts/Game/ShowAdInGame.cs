#if YSDK
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;

public class ShowAdInGame : MonoBehaviour
{
#if !YSDK
    private void Start() => Destroy(this);
#else
    [Range(2, 11)]
    [SerializeField] private int _showPerLevel = 4;
    [Space]
    [SerializeField] private int _timeBeforeShow = 3600;
    [SerializeField] private int _step = 3;

    private int _nextLevel = 0;

    private void Start() => Game.Inst.EventNewLevel += OnNewLevel;

    private void OnNewLevel(int level)
    {

        if (_nextLevel == 0)
        {
            SetNextLevel(level);
            return;
        }

        if (_nextLevel > level)
            return;

        if (YMoney.Inst.IsTimeAdFull)
        {
            _nextLevel++;
            return;
        }

        SetNextLevel();
        ShowAd().Forget();

        async UniTaskVoid ShowAd()
        {
            bool isNotBlock = !YMoney.Inst.IsBlockAd;

            if (isNotBlock)
                await Msg();

            bool isPause = !Game.Inst.IsPause && isNotBlock;

            if (isPause)
                Game.Inst.Pause(true);

            await YMoney.Inst.ShowFullscreenAdv();

            if (isPause)
                Game.Inst.Pause(false);
        }

        async UniTask Msg()
        {
            Message.Banner(Localization.Inst.GetText("ShowAds"), MessageType.Warning, _timeBeforeShow);

            int pause = _timeBeforeShow / _step;
            for (int i = _step; i > 0; i--)
            {
                Message.Banner(i.ToString(), MessageType.Warning, pause);
                await UniTask.Delay(pause, true);
            }
        }

        void SetNextLevel(int lvl = 0)
        {
            _nextLevel += Random.Range(_showPerLevel - 1, _showPerLevel + 2) + lvl;
        }
    }
#endif
}
