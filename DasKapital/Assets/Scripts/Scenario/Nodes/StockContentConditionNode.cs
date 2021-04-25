using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StockContentConditionNode : ConditionNode
{
    public Stock stock;
    public List<CommoditySO> commoditiesToStock = new List<CommoditySO>();

    public override void Activate()
    {
        base.Activate();
        ExchangeService.instance.onTransaction += CheckStockContent;
    }

    public void CheckStockContent()
    {
        List<CommoditySO> tempCommodities = (from type in commoditiesToStock
                                            select type).ToList();
        foreach (CommoditySO commodity in stock.Commodities)
        {
            if (tempCommodities.Contains(commodity))
            {
                tempCommodities.Remove(commodity);
            }
        }
        if (tempCommodities.Count == 0)
        {
            ScenarioService.instance.OnNodeStep();
            ExchangeService.instance.onTransaction -= CheckStockContent;
        }
    }
}
