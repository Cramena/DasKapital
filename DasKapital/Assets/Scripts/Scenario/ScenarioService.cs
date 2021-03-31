using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioService : MonoBehaviour
{
    public static ScenarioService instance;
    public Text scenarioText;
    public List<ScenarioNode> nodes = new List<ScenarioNode>();
    private int currentNodeIndex;

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
        nodes[currentNodeIndex].OnNodeEntered();
    }

    public void OnNodeStep()
    {
        currentNodeIndex++;
        if (currentNodeIndex < nodes.Count)
        {
            nodes[currentNodeIndex].OnNodeEntered();
        }
    }

    public void DisplayLine(string _key)
    {
        scenarioText.text = LocalisationService.instance.Translate(_key);
    }
}
