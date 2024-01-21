using UnityEngine;

[RequireComponent(typeof(SectorSound))]
public class Sector : MonoBehaviour
{
    [SerializeField] private SectorAnimation _sectorAnimation;
    [SerializeField] private Turn _selfTurn;
    private SectorSound _sectorSound;
    public int Id => _selfTurn.ToInt();
    public SectorSound Sound => _sectorSound;

    private bool _isOpen = false;

    private void Awake() => _sectorSound = GetComponent<SectorSound>();

    public void Open()
    {
        if (_isOpen) return; 
        
        _sectorAnimation.Play();
        _isOpen = true;
    }

    public void Close()
    {
        if (!_isOpen) return;

        _sectorAnimation.PlayRevers();
        _isOpen = false;
    }

    public void PauseSound(bool paused) => _sectorSound.Pause(paused);
}
