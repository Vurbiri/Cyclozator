#if YSDK

using Cysharp.Threading.Tasks;
using System;

public partial class YMoney
{
    private UniTaskCompletionSource<bool> taskEndShowFullscreenAdv;
    private UniTaskCompletionSource<bool> taskRewardRewardedVideo;
    private UniTaskCompletionSource<bool> taskCloseRewardedVideo;
    private UniTaskCompletionSource<bool> taskEndInitPayments;
    private UniTaskCompletionSource<string> taskEndPurchase;
    private UniTaskCompletionSource<string> taskEndGetPurchases;
    private UniTaskCompletionSource<bool> taskEndConsumePurchase;

    public void OnEndShowFullscreenAdv(int result) => taskEndShowFullscreenAdv?.TrySetResult(Convert.ToBoolean(result));
    public void OnRewardRewardedVideo(int result) => taskRewardRewardedVideo?.TrySetResult(Convert.ToBoolean(result));
    public void OnCloseRewardedVideo(int result) => taskCloseRewardedVideo?.TrySetResult(Convert.ToBoolean(result));
    public void OnEndInitPayments(int result) => taskEndInitPayments?.TrySetResult(Convert.ToBoolean(result));
    public void OnEndPurchase(string value) => taskEndPurchase?.TrySetResult(value);
    public void OnEndGetPurchases(string value) => taskEndGetPurchases?.TrySetResult(value);
    public void OnEndConsumePurchase(int result) => taskEndConsumePurchase?.TrySetResult(Convert.ToBoolean(result));
}
#endif