using Cysharp.Threading.Tasks;

public sealed class LoadingGame : LoadingScreen
{

    private void Start() => Loading().Forget();

    private async UniTaskVoid Loading()
    {
        StartLoadScene();

        await YMoney.Inst.ShowFullscreenAdv();

        EndLoadScene();
    }
}
