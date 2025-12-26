using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HighscoreButton : MonoBehaviour
{
    void Start()
    {
#if YSDK
        GetComponent<Button>().interactable = YandexSDK.Inst.IsLeaderboard;
#else
        GetComponent<Button>().interactable = false;
#endif
    }
       
}
