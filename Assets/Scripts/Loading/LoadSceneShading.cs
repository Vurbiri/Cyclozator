using UnityEngine;

public class LoadSceneShading : MenuNavigation
{
    [Space]
    [SerializeField] protected int _nextScene = 2;
    [Space]
    [SerializeField] protected float _speedShading = 0.4f;

    private readonly LoadScene _loadScene = new();

    public void StartLoadScene()
    {
        Shading.Inst.Appear(_speedShading, _loadScene.End);
        SmoothStartEndMusic.Inst.Disappear(_speedShading);

        _loadScene.Start(_nextScene);
    }
}
