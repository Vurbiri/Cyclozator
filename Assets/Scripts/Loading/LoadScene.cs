using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene 
{
    private AsyncOperation _asyncOperation = null;

    public void Start(int nextScene)
    {
        _asyncOperation = SceneManager.LoadSceneAsync(nextScene);
        _asyncOperation.allowSceneActivation = false;
    }

    public void End()
    {
        if (_asyncOperation == null) return;

        _asyncOperation.allowSceneActivation = true;
    }
}
