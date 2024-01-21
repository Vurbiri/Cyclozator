using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Dealer))]
public class ScrollViewInventory : ScrollViewPanel<ItemGoodsGUI>
{
    [SerializeField] private Button _buttonBuy;
    [SerializeField] private string _keyTextBuy = "Buy";
    [SerializeField] private string _keyTextAd = "View";
    [SerializeField] private string _keyTextYanErr = "Complete";

    [Space]
    [SerializeField] private InputEvents _inputEvents;

    private Dealer _dealer;
    private Text _textButtonBuy;
    private string _textBuy;
    private string _textAd;
    private string _textYanErr;

    protected override void Awake()
    {
        _dealer = GetComponent<Dealer>();
        _textButtonBuy = _buttonBuy.targetGraphic as Text;
        _buttonBuy.interactable = false;

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        _buttonBuy.onClick.AddListener(Buy);

        foreach (var g in Inventory.Inst.Goods.Values)
            CreateItem(g);
        foreach (var b in Inventory.Inst.Buffs.Values)
            CreateItem(b);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetText(_dealer.TargetItem);
        _inputEvents.EventOk += ButtonOnClick;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _inputEvents.EventOk -= ButtonOnClick;
    }

    private void Buy()
    {
        _buttonBuy.interactable = false;
        BuyAsync().Forget();

        async UniTaskVoid BuyAsync()
        {
            await _dealer.Buy();
            SetText(_dealer.TargetItem);
            _buttonBuy.interactable = _dealer.IsPossibleBuy();
        }
    }

    protected override ItemGoodsGUI CreateItem(Item item)
    {
        ItemGoodsGUI itemGUI = base.CreateItem(item);
        if (item.Currency == Currency.Ad)
        {
            bool init = YandexSDK.Inst.IsInitialize;
            itemGUI.OnAdvertisement(init ? "1" : "0", init);
            _dealer.EventAdvertisement += itemGUI.OnAdvertisement;
        }

        return itemGUI;
    }

    protected override void OnItemSelected(Item sender) => SetElements(sender, sender.Description);

    protected override void OnItemDeselected(Item sender)
    {
        if (_dealer.TargetItem != sender) return;

        SetElements(null, _hintTextDefault);
    }

    private void SetElements(Item item, string hintText)
    {
        _dealer.TargetItem = item;
        _hint.text = hintText;
        _buttonBuy.interactable = _dealer.IsPossibleBuy();
        SetText(_dealer.TargetItem);
    }

    private void ButtonOnClick()
    {
        if (_buttonBuy.interactable)
            _buttonBuy.onClick?.Invoke();
    }

    protected override void GetText()
    {
        base.GetText();
        _textBuy = Localization.Inst.GetText(_keyTextBuy);
        _textAd = Localization.Inst.GetText(_keyTextAd);
        _textYanErr = Localization.Inst.GetText(_keyTextYanErr);

        SetText(_dealer.TargetItem);
    }

    private void SetText(Item item)
    {
        if (item != null)
        {
            _hint.text = item.Description;
            _textButtonBuy.text = item.Currency switch
            {
                Currency.Ad => _textAd,
                Currency.Yan => TextButtonYan(),
                _ => _textBuy,
            };
        }
        else
        {
            _hint.text = _hintTextDefault;
            _textButtonBuy.text = _textBuy;
        }

        string TextButtonYan()
        {
            if (!YandexSDK.Inst.IsLogOn)
                Message.Banner(Localization.Inst.GetText("LogonPurchase"), MessageType.Warning, 6750);

            return YMoney.Inst.Status == StatusPurchase.Ready ? _textBuy : _textYanErr;
        }
    }
}
