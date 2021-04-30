using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectConditionNode : ConditionNode
{
    public List<CommoditySO> commoditiesToInspect = new List<CommoditySO>();

    public override void Activate()
    {
        base.Activate();
        ScenarioService.instance.onCommodityInspected += CheckCommodityInspected;
    }

    public void CheckCommodityInspected(CommoditySO _commodity)
    {
        if (commoditiesToInspect.Contains(_commodity))
        {
            ScenarioService.instance.OnNodeStep();
            ScenarioService.instance.onCommodityInspected -= CheckCommodityInspected;
        }
    }
}
