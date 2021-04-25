using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommodityPlacementConditionNode : ConditionNode
{

    public override void Activate()
    {
        base.Activate();
        CommoditiesService.instance.onCommodityPlacementRegistered += CheckCommodityPlacement;
    }

    public void CheckCommodityPlacement()
    {
        ScenarioService.instance.OnNodeStep();
        CommoditiesService.instance.onCommodityPlacementRegistered -= CheckCommodityPlacement;
    }
}
