using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InfoBoardsGame : BoardsScore
{
    [Space]
    [SerializeField] private Text _textCountFigures;
    [SerializeField] private Text _textLevel;
    

    private IEnumerator Start()
    {
        yield return null;
        
        BoardsBind(Game.Inst.Score);

        Game.Inst.EventFigureChange += (f) => _textCountFigures.text = GetTwoValue(f.ToString());
        Game.Inst.EventNewLevel += (l) => _textLevel.text = GetOneValue(l.ToString());
    }
}
