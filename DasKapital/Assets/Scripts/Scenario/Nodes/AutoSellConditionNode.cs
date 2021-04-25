using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoSellConditionNode : ConditionNode
{
    public CommoditySO typeToSell;
    public override void Activate()
    {
        base.Activate();
        ScenarioService.instance.onAutoSell += CheckAutoSell;
    }

    public void CheckAutoSell(CommoditySO _commodity)
    {
        if (typeToSell == _commodity)
        {
            ScenarioService.instance.OnNodeStep();
            ScenarioService.instance.onAutoSell -= CheckAutoSell;
        }
    }
}
