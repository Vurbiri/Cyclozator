using System;
using UnityEngine;

public static class Message
{
    public static void Log(string msg)
    {
#if UNITY_EDITOR
        Debug.Log(msg);
#elif UNITY_WEBGL
        UnityJS.Log(msg);
#endif
    }

    public static void Banner(string message, MessageType type = MessageType.Normal, int time = 5000, bool isThrough = true) => Banners.Inst.Message(message, type, time, isThrough);

    public static void Saving(string goodMSG, bool isSaving) 
    {
        if (isSaving)
            Banner(Localization.Inst.GetText(goodMSG), time: 1500);
        else
            Banner(Localization.Inst.GetText("ErrorSave"), MessageType.Error, 2500);
    }

    public static void AddCoins(int coins, string msg = null, bool isTotal = false, int time = 5000)
    {
        if(!string.IsNullOrEmpty(msg))
            Banner(Localization.Inst.GetText(msg), time: time);

        if (coins == 1)
            msg = Localization.Inst.GetText("GoodsCoin");
        else
            msg = String.Format(Localization.Inst.GetText("GoodsCoins"), coins);

        if(isTotal)
            msg += Localization.Inst.GetText("ToTotal");
        Banner(msg, time: time);
    }


    //    public static void Error(string msg)
    //    {
    //#if UNITY_EDITOR
    //        Debug.LogError(msg);
    //#elif UNITY_WEBGL
    //        UnityJS.Error(msg);
    //#endif
    //    }

}
