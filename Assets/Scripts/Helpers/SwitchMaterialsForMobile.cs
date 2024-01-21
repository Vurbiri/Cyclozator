using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SwitchMaterialsForMobile : MonoBehaviour
{
    [SerializeField] private Material[] _mobileMaterials;

    private void Awake()
    {
        if (SettingsStorage.Inst.IsDesktop) return;

        Renderer thisRenderer = GetComponent<Renderer>();
        List<Material> thisMaterials = new(thisRenderer.materials);
        Material old;

        for(int i = 0;  i < _mobileMaterials.Length; i++)
        {
            old = thisMaterials[i];
            thisMaterials[i] = _mobileMaterials[i];
            Destroy(old);
        }

        thisRenderer.SetSharedMaterials(thisMaterials);
    }
}
