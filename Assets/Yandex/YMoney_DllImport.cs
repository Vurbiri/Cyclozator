using System.Runtime.InteropServices;

public partial class YMoney
{
    [DllImport("__Internal")]
    private static extern bool IsPaymentsJS();

    [DllImport("__Internal")]
    private static extern void ShowFullscreenAdvJS();
    [DllImport("__Internal")]
    private static extern void ShowRewardedVideoJS();

    [DllImport("__Internal")]
    private static extern void InitPaymentsJS();
    [DllImport("__Internal")]
    private static extern void PurchaseJS(string pId, string devPayload);
    [DllImport("__Internal")]
    private static extern void GetPurchasesJS();
    [DllImport("__Internal")]
    private static extern void ConsumePurchaseJS(string token);

}
