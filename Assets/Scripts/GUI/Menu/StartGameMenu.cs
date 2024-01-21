using Cysharp.Threading.Tasks;
using UnityEngine;

public class StartGameMenu : LoadSceneShading
{
    [Space]
    [SerializeField] private LeaderboardGUI _leaderboard;

    public void StartNewGame()
    {
        RewardAndReset().Forget();

        async UniTaskVoid RewardAndReset()
        {
            int points = 0;
            var game = GameStorage.Inst;
            if (game.ModeStart != GameModeStart.New && game.SimpleSave != null)
                points = (int)(new ScoreGame(game.SimpleSave.A, game.SimpleSave.B)).Points;

            BuffStorage.Inst.ResetBuffs(false);
            bool isRecord = await _leaderboard.TrySetScoreAndReward(points, false);
            game.ResetContinueGame();
            if (isRecord) _leaderboard.Show();
        }
    }

}
