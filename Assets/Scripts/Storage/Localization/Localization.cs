using System;
using System.Collections.Generic;
using UnityEngine;

public partial class Localization : Singleton<Localization>
{
    [SerializeField] private string _path = "Languages";
    [SerializeField] private string _defaultLang = "en";

    private Dictionary<string, string> _language = new();

    private LanguageType[] _languages;
    public LanguageType[] Languages => _languages;

    public int CurrentIdLang { get; private set; } = -1;

    public event Action EventSwitchLanguage;

    public bool LoadFromResources()
    {
        var (result, value) = Storage.LoadFromResources<LanguageType[]>(_path);
        if (result)
        {
            _languages = value;
            return SwitchLanguage(_defaultLang);
        }

        return false;
    }

    public bool TryIdFromCode(string codeISO639_1, out int id)
    {
        id = -1;
        if (string.IsNullOrEmpty(codeISO639_1)) 
            return false;

        foreach (LanguageType language in _languages)
        {
            if (language.CodeISO639_1.ToLowerInvariant() == codeISO639_1.ToLowerInvariant())
            {
                id = language.Id;
                return true;
            }
        }
        return false;  
    }

    public bool SwitchLanguage(string codeISO639_1)
    {
        if (TryIdFromCode(codeISO639_1, out int id))
            return SwitchLanguage(id);

        return false;
    }

    public bool SwitchLanguage(int id)
    {
        if (CurrentIdLang == id) return true;

        foreach (LanguageType language in _languages)
            if (language.Id == id)
                return SetLanguage(language);

        return false;
    }

    public string GetText(string name)
    {
        
        if (_language.TryGetValue(name, out string str))
            return str;

        return "";
    }

    public string GetDescBuff(ItemSubType type)
    {
        if (_language.Count == 0) return "";

        return type switch
        {
            ItemSubType.None => _language["Empty"],
            ItemSubType.Catch_10 => String.Format(_language["BuffCatch"], 10),
            ItemSubType.Catch_20 => String.Format(_language["BuffCatch"], 20),
            ItemSubType.Catch_30 => String.Format(_language["BuffCatch"], 30),
            ItemSubType.Spawn_10 => String.Format(_language["BuffSpawn"], 10),
            ItemSubType.Spawn_20 => String.Format(_language["BuffSpawn"], 20),
            ItemSubType.Spawn_30 => String.Format(_language["BuffSpawn"], 30),
            ItemSubType.SpawnOne_10 => String.Format(_language["BuffSpawnFromOne"], 10),
            ItemSubType.SpawnOne_20 => String.Format(_language["BuffSpawnFromOne"], 20),
            ItemSubType.SpawnOne_30 => String.Format(_language["BuffSpawnFromOne"], 30),
            ItemSubType.RotSpeed_10 => String.Format(_language["BuffRotSpeed"], 10),
            ItemSubType.RotSpeed_20 => String.Format(_language["BuffRotSpeed"], 20),
            ItemSubType.RotSpeed_30 => String.Format(_language["BuffRotSpeed"], 30),
            ItemSubType.Speed_10 => String.Format(_language["BuffSpeed"], 10),
            ItemSubType.Speed_20 => String.Format(_language["BuffSpeed"], 20),
            ItemSubType.Speed_30 => String.Format(_language["BuffSpeed"], 30),
            ItemSubType.MultiSpawn_10 => String.Format(_language["BuffMultiSpawn"], 10),
            ItemSubType.MultiSpawn_20 => String.Format(_language["BuffMultiSpawn"], 20),
            ItemSubType.MultiSpawn_30 => String.Format(_language["BuffMultiSpawn"], 30),
            ItemSubType.Bridge => _language["BuffBridge"],
            ItemSubType.Count => _language["BuffCount"],
            ItemSubType.MultiSpawnThreeOff => _language["BuffMultiSpawnThreeOff"],
            ItemSubType.Sector => _language["BuffSector"],
            ItemSubType.Points_x2 => _language["BuffPoints"],
            ItemSubType.Cell => _language["GoodsCell"],
            ItemSubType.Coin_1 => _language["GoodsCoin"],
            ItemSubType.Coin_10 => String.Format(_language["GoodsCoins"], 10),
#if YSDK
            ItemSubType.Coin_Ads => _language["GoodsCoinsAd"],
            ItemSubType.Coin_Yan => String.Format(_language["GoodsCoinsYan"], YMoney.Inst.Coins, YMoney.Inst.PriceYan, YMoney.Inst.DayOffAd),
#endif

            _ => _language["Error"]
        };
    }

    private bool SetLanguage(LanguageType type)
    {
        var (result, value) = Storage.LoadFromResources<Dictionary<string, string>>(type.File);
        if (result)
        {
            CurrentIdLang = type.Id;
            _language = new(value, new StringComparer());
            EventSwitchLanguage?.Invoke();
        }

        return result;
    }

    public class StringComparer : IEqualityComparer<string>
    {
        public bool Equals(string str1, string str2)
        {
            return str1.ToLowerInvariant() == str2.ToLowerInvariant();
        }
        public int GetHashCode(string str)
        {
            return str.ToLowerInvariant().GetHashCode();
        }

    }

}
