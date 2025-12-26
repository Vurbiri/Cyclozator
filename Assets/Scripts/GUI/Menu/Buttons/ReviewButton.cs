#if YSDK
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
#endif
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ReviewButton : MonoBehaviour
{
#if !YSDK
    private void Start() => Destroy(gameObject);
#else
    [SerializeField] private string _message = "ReviewReward";
    [SerializeField] private int _coins = 25;
    [Space]
    [SerializeField] private float _secPauseMsg = 2.5f;

    private void Start() => Initialize().Forget();

    private async UniTaskVoid Initialize()
    {
        Button shortcutButton = GetComponent<Button>();
        if (YMoney.Inst.IsFirstStart || !await YandexSDK.Inst.CanReview())
        {
            shortcutButton.interactable = false;
            return;
        }

        AddListener();
        StartCoroutine(MessageReview());
        
        IEnumerator MessageReview()
        {
            yield return new WaitForSeconds(_secPauseMsg);
            Message.Banner(String.Format(Localization.Inst.GetText(_message), _coins), time: 7750, isThrough: false);
        }

        void AddListener()
        {
            shortcutButton.onClick.AddListener(UniTask.UnityAction(async () =>
                {
                    shortcutButton.interactable = false;
                    if (await YandexSDK.Inst.RequestReview())
                    {
                        Inventory.Inst.TotalScore.AddCoins(_coins);
                        Inventory.Inst.Save(true, (b) => { if (b) Message.AddCoins(_coins, "Review"); });
                    }
                }));
        }
    }
#endif
}
