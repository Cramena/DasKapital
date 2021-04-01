using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ScenarioService : MonoBehaviour
{
    public static ScenarioService instance;
    public Text scenarioText;
    public List<ScenarioNode> nodes = new List<ScenarioNode>();
    private int currentNodeIndex;
    public List<UnityEvent> scenarioEvents = new List<UnityEvent>();
    public bool delayedDistribution;
    public bool valueJaugeActive;
    public bool inProductionPhase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception($"Too many {this} instances");
        }
    }

    private void Start()
    {
        nodes[currentNodeIndex]?.OnNodeEntered();
    }

    public void OnNodeStep()
    {
        nodes[currentNodeIndex]?.OnNodeLeft();
        currentNodeIndex++;
        if (currentNodeIndex < nodes.Count)
        {
            nodes[currentNodeIndex]?.OnNodeEntered();
        }
    }

    public void DisplayLine(string _key)
    {
        scenarioText.text = LocalisationService.instance.Translate(_key);
    }

    public void LaunchEvent(int _index)
    {
        scenarioEvents[_index].Invoke();
    }

    public void SetDelayedDistribution(bool _active)
    {
        delayedDistribution = _active;
    }

    public void SetValueJaugeActive(bool _active)
    {
        valueJaugeActive = _active;
    }

    public void SetStockToStockMovement(bool _active)
    {
        inProductionPhase = _active;
    }
}
