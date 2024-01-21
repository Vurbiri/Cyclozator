using UnityEngine;

public class Sectors : MonoBehaviour
{
    Sector[] _sectors;

    private void Awake() => _sectors = GetComponentsInChildren<Sector>();
    public void InitializeLevel(bool[] sectorsOpen)
    {
        int volumeScale = 0;

        foreach(var s in sectorsOpen)
            if(s) volumeScale++;

        bool isOpen;
        foreach (var sector in _sectors)
        {
            sector.Sound.GetVolume(1f/(float)volumeScale);
            isOpen = sectorsOpen[sector.Id];
            if (isOpen)
                sector.Open();
            else
                sector.Close();

        }
    }
}
