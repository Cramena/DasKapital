using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CommodityProfile
{
    public CommodityProfile()
    {
        commodityName = "";
        exchangeValue = -1;
        useValue = UseValue.None;
        isDurable = false;
        usesAmount = -1;
        components = null;
    }
    
    public CommodityProfile(string _name, int _value, UseValue _useValue, string _useValueDescription,bool _isDurable,
                            int _usesAmount, int _valuePerUse, List<CommodityProfile> _components, Sprite _icon, Color _color)
    {
        commodityName = _name;
        exchangeValue = _value;
        useValue = _useValue;
        useValueDescription = _useValueDescription;
        isDurable = _isDurable;
        usesAmount = _usesAmount;
        valuePerUse = _valuePerUse;
        components = _components;
        icon = _icon;
        color = _color;
    }
    public CommodityProfile(CommodityProfile _profile)
    {
        type = _profile.type;
        commodityName = _profile.commodityName;
        exchangeValue = _profile.exchangeValue;
        useValue = _profile.useValue;
        useValueDescription = _profile.useValueDescription;
        isDurable = _profile.isDurable;
        usesAmount = _profile.usesAmount;
        valuePerUse = _profile.valuePerUse;
        components = _profile.components;
        icon = _profile.icon;
        sizeModifier = _profile.sizeModifier;
        color = _profile.color;
        components = _profile.components;
    }
    public CommodityProfile(CommoditySO _type)
    {
        type = _type;
        commodityName = _type.commodityName;
        exchangeValue = _type.exchangeValue;
        useValue = _type.useValue;
        useValueDescription = _type.useValueDescription;
        isDurable = _type.isDurable;
        usesAmount = _type.usesAmount;
        initialUsesAmount = _type.usesAmount;
        valuePerUse = usesAmount != 0 ? exchangeValue / usesAmount : 0;
        icon = _type.icon;
        sizeModifier = _type.sizeModifier;
        color = _type.color;
        components = new List<CommodityProfile>();
    }

    public CommoditySO type;
    public string commodityName;
    public int exchangeValue;
    public UseValue useValue;
    public string useValueDescription;
    public bool isDurable;
    public int usesAmount;
    public int initialUsesAmount;
    public int valuePerUse;
    public List<CommodityProfile> components;
    public Sprite icon;
    public float sizeModifier = 1;
    public Color color;

}
