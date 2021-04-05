using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProduceConditionNode : ConditionNode
{
    public List<CommoditySO> commoditiesToProduce = new List<CommoditySO>();

    public override void Activate()
    {
        base.Activate();
        ScenarioService.instance.onProduceRegistered += CheckCommodityProduced;
    }

    public void CheckCommodityProduced(CommoditySO _commodity)
    {
        if (commoditiesToProduce.Contains(_commodity))
        {
            commoditiesToProduce.Remove(_commodity);
            if (commoditiesToProduce.Count == 0)
            {
                ScenarioService.instance.OnNodeStep();
                ScenarioService.instance.onProduceRegistered -= CheckCommodityProduced;
            }
        }
    }
}
