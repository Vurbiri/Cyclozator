using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Box_", menuName = "Circlizator/Box", order = 51)]
public class BoxSettings : ScriptableObject
{
    [SerializeField] private FigureType _type;
    [SerializeField] private Material _materialDisplay;
    [SerializeField] private Material _materialDisplayMobile;
    [SerializeField] private Material _materialChange;

    public FigureType Type => _type;
    public int Id => Convert.ToInt32(_type);
    public Material MaterialDisplay => _materialDisplay;
    public Material MaterialDisplayMobile => _materialDisplayMobile;
    public Material MaterialChange => _materialChange;
    public Color Color => _materialDisplay != null ? _materialDisplay.color : default;

}
