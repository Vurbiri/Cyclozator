using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class ObjectAnimation : MonoBehaviour
{
    protected Animator _thisAnimator;

    private void Awake() => _thisAnimator = GetComponent<Animator>();

    public virtual void Play() => _thisAnimator.SetBool("isOpen", true);

    public virtual void PlayRevers() => _thisAnimator.SetBool("isOpen", false);
}
