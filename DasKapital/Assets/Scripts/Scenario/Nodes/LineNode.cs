using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineNode : ScenarioNode
{
    public string lockey;

    public override void OnNodeEntered(bool _rewind)
    {
        ScenarioService.instance.NextLine(lockey);
        base.OnNodeEntered(_rewind);
    }

    public override void OnNodeRewind()
    {
        base.OnNodeRewind();
        ScenarioService.instance.PreviousLine(lockey);
    }
}
