using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistributionConditionNode : ConditionNode
{
    public override void Activate()
    {
        base.Activate();
        ScenarioService.instance.onDistribution += CheckDistribution;
    }

    public void CheckDistribution()
    {
        ScenarioService.instance.OnNodeStep();
        ScenarioService.instance.onDistribution -= CheckDistribution;
    }
}
