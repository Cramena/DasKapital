using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UseValue
{
    None,
    AddValue,
    Spicy,
    Sour,
    Acidic,
    Sweet,
    Salty
}

public enum CommodityGroup
{
    None,
    Fruit,
    Tranche,
    Jus,
    Smoothie,
    Salade,
    Assiette,
    Compote,
    Cocktail
}

[CreateAssetMenu(fileName = "CommoditySO", menuName = "DasKapital/CommoditySO")]
public class CommoditySO : ScriptableObject
{
    public int index;
    public string commodityName;
    public int exchangeValue;
    public UseValue useValue;
    public string useValueDescription;
    public bool isDurable;
    public int usesAmount;
    public List<CommoditySO> components;
    public Sprite icon;
    public float sizeModifier = 1;
    public Color color;
    public int probabilityWeight = 1;
    public CommodityGroup group;
}
