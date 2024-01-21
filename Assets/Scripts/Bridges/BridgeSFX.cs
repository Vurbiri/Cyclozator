using UnityEngine;

public class BridgeSFX : MonoBehaviour
{
    [SerializeField] private ObjectAnimation _bridgeAnimation;
    [SerializeField] private ChangeLightBridge _bridgeLight;

    public void Open()
    {
        _bridgeAnimation.Play();
        _bridgeLight.LightOpen();
    }

    public void Close()
    {
        _bridgeAnimation.PlayRevers();
        _bridgeLight.LightClose();
    }

}

