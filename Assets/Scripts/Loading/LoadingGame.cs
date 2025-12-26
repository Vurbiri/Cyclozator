#if YSDK
using Cysharp.Threading.Tasks;
#endif

public sealed class LoadingGame : LoadingScreen
{
#if !YSDK
    private void Start()
    {
        StartLoadScene();
        EndLoadScene();
    }
#else
    private void Start() => Loading().Forget();

    private async UniTaskVoid Loading()
    {
        StartLoadScene();

        await YMoney.Inst.ShowFullscreenAdv();

        EndLoadScene();
    }
#endif
}
