
[System.Serializable]
public class GUIAnimation : ObjectAnimation
{
    public virtual void SetMotionTime(float time) => _thisAnimator.speed = 1f / time;
}
