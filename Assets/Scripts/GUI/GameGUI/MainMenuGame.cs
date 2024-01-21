using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuGame : LoadSceneShading
{
    [SerializeField] private Button _saveButton;
    private bool _isSaving = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        _saveButton.interactable = Game.Inst.Mode != GameMode.None;
    }

    public void OnSave()
    {
        OnSaveAsync().Forget();

        async UniTaskVoid OnSaveAsync()
        {
            _saveButton.interactable = false;
            _saveButton.interactable = !await AllSave();
        }
    }

    public void OnToMenu()
    {
        OnToMenuAsync().Forget();

        async UniTaskVoid OnToMenuAsync()
        {
            await AllSave();
            StartLoadScene();
            Time.timeScale = 1f;
        }
    }

    private async UniTask<bool> AllSave()
    {
        if(_isSaving || Game.Inst.Mode == GameMode.None) 
            return true;

        _isSaving = true;
        bool result = true;
        UniTaskCompletionSource<bool> taskSave;
        await StorageSave(Game.Inst.Save);
        await StorageSave(BuffStorage.Inst.Save);
        await StorageSave(SettingsStorage.Inst.Save);
        result = await Inventory.Inst.SaveAsync() && result;

        Message.Saving("GoodSave", result);
        _isSaving = false;

        return result;

        async UniTask StorageSave(Action<bool, Action<bool>> save)
        {
            taskSave = new();
            save(false, (b) => taskSave.TrySetResult(b));
            result = await taskSave.Task && result;
        }
    }
}