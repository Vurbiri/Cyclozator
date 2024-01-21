using System.Collections;
using UnityEngine;

public class PanelsStartEndAnimation : MonoBehaviour
{
    [SerializeField] GUIAnimation[] _animPanels;
    [Space]
    [SerializeField] private float _motionTime = 2f;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(Game.Inst.PauseBeforeStart);

        foreach (var animPanel in _animPanels)
        {
            animPanel.SetMotionTime(_motionTime);
            animPanel.Play();
        }

    }

    public void AllPlayRevers()
    {
        foreach (var animPanel in _animPanels)
            animPanel.PlayRevers();
    }

}
