using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private int _nextScene = 3;
    [Space]
    [SerializeField] private Transform[] _figures;

    private readonly LoadScene _loadScene = new();
    protected void EndLoadScene() => _loadScene.End();

    protected void StartLoadScene()
    {
        _loadScene.Start(_nextScene);

        Random.InitState(Helper.Seed);
        if (_figures?.Length > 0)
            Instantiate(_figures[Random.Range(0, _figures.Length)], transform).GetComponentInChildren<RandomRotation>().Setup(3f, 9f);
    }
}
