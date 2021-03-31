using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradingStock : UIOwner
{
    public void GetCommodities(List<Commodity> _commodities)
    {
        for (var i = 0; i < _commodities.Count; i++)
        {
            _commodities[i].transform.SetParent(transform.GetChild(i));
            _commodities[i].body.LerpTo(Vector2.zero);
        }
    }
}
