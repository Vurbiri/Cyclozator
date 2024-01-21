using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HighscoreButton : MonoBehaviour
{
    void Start() =>
        GetComponent<Button>().interactable = YandexSDK.Inst.IsLeaderboard;
}
