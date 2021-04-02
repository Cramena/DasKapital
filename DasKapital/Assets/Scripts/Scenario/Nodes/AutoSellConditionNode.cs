using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSellConditionNode : ConditionNode
{
    public override void Activate()
    {
        ScenarioService.instance.onAutoSell += CheckAutoSell;
    }

    public void CheckAutoSell()
    {
        ScenarioService.instance.OnNodeStep();
        ScenarioService.instance.onAutoSell -= CheckAutoSell;
    }
}
