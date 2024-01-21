using UnityEngine;
using UnityEngine.UI;

public class BoardsScore : MonoBehaviour
{
    [SerializeField] private Text _textCountPoints;
    [SerializeField] private Text _textCountCoins;
    [Space]
    [SerializeField] private string _maxOneValue = "9999999";
    [SerializeField] private string _maxTwoValue = "9999999";

    private Score _score;

    protected void BoardsBind(Score score)
    {
        _score = score;

        _textCountPoints.text = GetOneValue(score.Points.ToString());
        _textCountCoins.text = GetTwoValue(score.Coins.ToString());

        _score.EventPointsChange += SetPoints;
        _score.EventCoinsChange += SetCoins;
    }

    protected virtual void OnDestroy()
    {
        if( _score != null)
        {
            _score.EventPointsChange -= SetPoints;
            _score.EventCoinsChange -= SetCoins;
        }
    }

    private void SetPoints(string points) => _textCountPoints.text = GetOneValue(points);
    private void SetCoins(string coins) => _textCountCoins.text = GetTwoValue(coins);

    protected string GetOneValue(string value) => value.Length > _maxOneValue.Length ? _maxOneValue : value;
    protected string GetTwoValue(string value) => value.Length > _maxTwoValue.Length ? _maxTwoValue : value;
}
