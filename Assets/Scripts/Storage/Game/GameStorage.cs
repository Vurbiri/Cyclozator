using System;

public partial class GameStorage : Singleton<GameStorage>, ILoading
{
    private const string modeStartKey = "trts";

    public GameModeStart ModeStart { get; private set; } = GameModeStart.New;

    public GameSave Save { get; private set; }
    public GameSimpleSave SimpleSave { get; private set; }

    public void SimpleSaveGame(Score score, int level, bool isSaveHard = true, Action<bool> callback = null)
    {
        bool result = true;
        GameSimpleSave simpleSave = new(score, level);
        Storage.Save(GameSimpleSave.Key, simpleSave, false, (b) => result = result && b);
        SimpleSave = simpleSave;
        ModeStart = GameModeStart.ContinueSimple;
        SaveMode(isSaveHard, (b) => callback?.Invoke(result && b));
    }


    public void SaveGame(Score score, int level, FigureType[] types, Level.LBridges bridges, int figures, int[] boxes, bool isSaveHard = true, Action<bool> callback = null)
    {
        bool result = true;
        SimpleSaveGame(score, level, false, (b) => result = result && b);
        GameSave save = new(types, bridges, figures, boxes);
        Storage.Save(GameSave.Key, save, false, (b) => result = result && b);
        Save = save;
        ModeStart = GameModeStart.Continue;
        SaveMode(isSaveHard, (b) => callback?.Invoke(result && b));
    }

    private void SaveMode(bool isSaveHard = true, Action<bool> callback = null) => Storage.Save(modeStartKey, ModeStart, isSaveHard, callback);
    public bool Load()
    {
        var (result, value) = Storage.Load<GameModeStart>(modeStartKey);
        if (!result)
            return false;

        ModeStart = value;

        if (ModeStart == GameModeStart.ContinueSimple)
            return SimpleLoadGame();
        else if (ModeStart == GameModeStart.Continue)
            return LoadGame();

        return true;
    }

    public void Create()
    {
        ModeStart = GameModeStart.New;
        SimpleSave = null;
        Save = null;
    }

    private bool SimpleLoadGame()
    {
        var(result, value) = Storage.Load<GameSimpleSave>(GameSimpleSave.Key);
        SimpleSave = value;
        return result && SimpleSave?.Level > 0;
    }

    private bool LoadGame()
    {
        bool r =  SimpleLoadGame();
        if (!r) return false;

        var(result, value) =  Storage.Load<GameSave>(GameSave.Key);
        Save = value;
        return result;
    }

    public void ResetGame(ScoreGame score, bool isSaveHard = true)
    {
        ModeStart = GameModeStart.New;
        bool scoreNull = score == null;
        SaveMode(scoreNull);

        if (scoreNull) return;

        Inventory.Inst.TotalScore.DumpScore(score);
        Storage.Save(GameSimpleSave.Key, new GameSimpleSave(), false);
        Storage.Save(GameSave.Key, new GameSave(), false);
        Inventory.Inst.Save(isSaveHard);
        SimpleSave = null;
    }

    public void ResetContinueGame(bool isSaveHard = true)
    {
        if (SimpleSave == null)
            ResetGame(null, isSaveHard);
        else
            ResetGame(new(SimpleSave.A, SimpleSave.B), isSaveHard);
    }

}
