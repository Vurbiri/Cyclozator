using System.Collections;
using UnityEngine;

public class BuffsGamePanel : MonoBehaviour
{
    [SerializeField] private BuffIcon _buffIcon;

    private IEnumerator Start()
    {
        yield return null;

        Transform thisTransform = gameObject.transform;
        foreach (var b in BuffStorage.Inst.Buffs)
            Instantiate(_buffIcon, thisTransform).Icon = b.Sprite;
    }
}
