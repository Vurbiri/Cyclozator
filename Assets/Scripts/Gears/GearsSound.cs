
public class GearsSound : SoundControl
{
    public override void Play()
    {
        base.Play();

        _thisAudioSource.Play();
    }

}
