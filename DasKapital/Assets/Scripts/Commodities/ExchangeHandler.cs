using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool CanExchangeCommodities(CommoditySO firstCommodity, CommoditySO secondCommodity)
    {
        return firstCommodity.exchangeValue == secondCommodity.exchangeValue;
    }

    void ExchangeCommodities(CommoditySO firstCommodity, CommoditySO secondCommodity)
    {
        if (CanExchangeCommodities(firstCommodity, secondCommodity))
        {
            
        }
    }
}
