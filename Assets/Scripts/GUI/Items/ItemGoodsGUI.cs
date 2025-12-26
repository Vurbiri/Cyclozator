using UnityEngine;
using UnityEngine.UI;

public class ItemGoodsGUI : ItemGUI
{
	[Space]
	[SerializeField] private Image _pricePanel;
	[SerializeField] private TextLocalization _textCaption;
	[SerializeField] private Text _textPrice;
 
	public override void Setup(Item item)
	{
		base.Setup(item);
		_textCaption.Setup(_item.Currency.ToString());
		_textPrice.text = _item.Price.ToString();
		_countPanel.gameObject.SetActive(_item.IsBuff);

		if (_item.IsBuff)
		{
			SetText(_item.Count);
			_item.EventCountChange += SetText;
		}
		else if (_item.SubType == ItemSubType.Cell) 
		{
			OnCountChange();
			_item.EventCountChange += OnCountChange;
		}
#if YSDK
		else if (_item.SubType == ItemSubType.Coin_Yan)
		{
			SetInteractable(YMoney.Inst.IsPayments);
			YMoney.Inst.EventChangedPayments += SetInteractable;
		}
#endif
	}
	
	protected override void SetInteractable(bool isOn)
	{
		if (isOn == _isOn) return;

		base.SetInteractable(isOn);
		
		Color currentColor = isOn ? _colorOn : _colorOff;
		_pricePanel.color = currentColor;
		_textCaption.Text.color = currentColor;
	}

	public void OnAdvertisement(string price, bool isOn)
	{
		_textPrice.text = price; 
		SetInteractable(isOn);
	}

	private void OnDestroy()
	{
		if (_item.IsBuff)
			_item.EventCountChange -= SetText;
		else if (_item.SubType == ItemSubType.Cell)
			_item.EventCountChange -= OnCountChange;
#if YSDK
		else if (_item.SubType == ItemSubType.Coin_Yan)
			YMoney.Inst.EventChangedPayments -= SetInteractable;
#endif

	}

	private void SetText(int i) => _textCount.text = i.ToString();
	private void OnCountChange(int i = 0) => SetInteractable(!BuffStorage.Inst.IsMaxCellBuff);

}
