using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum Condition
{
    None,
    Button,
    Produce,
    Info
}
public class ScenarioService : MonoBehaviour
{
    public static ScenarioService instance;
    public Text scenarioText;
    public GameObject continueButton;
    public Button transactionButton;
    public Button productionButton;
    public Button distributionButton;
    public List<ScenarioNode> nodes = new List<ScenarioNode>();
    private int currentNodeIndex;
    public List<UnityEvent> scenarioEvents = new List<UnityEvent>();
    public Condition currentCondition;
    public bool delayedDistribution;
    public bool valueJaugeActive;
    public bool inProductionPhase;
    public bool allowBaseCommoditiesAutoSell;
    public System.Action<CommoditySO> onProduceRegistered;
    public System.Action<CommoditySO> onAutoSell;
    public System.Action onDistribution;
    public System.Action<CommoditySO> onCommodityInspected;

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
        SetContinueButtonActive(true);
    }

    public void OnNodeStep()
    {
        nodes[currentNodeIndex]?.OnNodeLeft();
        currentCondition = Condition.None;
        SetContinueButtonActive(true);
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

    public void SetContinueButtonActive(bool _active)
    {
        continueButton.SetActive(_active);
    }

    public void SetBaseCommoditiesAutoSellAllowed(bool _allowed)
    {
        allowBaseCommoditiesAutoSell = _allowed;
    }

    public void RegisterProduce(CommoditySO _type)
    {
        onProduceRegistered?.Invoke(_type);
    }

    public void OnAutoSell(CommoditySO _commodity)
    {
        onAutoSell?.Invoke(_commodity);
    }

    public void OnCommodityInspected(CommoditySO _type)
    {
        onCommodityInspected?.Invoke(_type);
    }

    public void LaunchCommodityEdit()
    {
        CommoditiesService.instance.EditCommodities();
    }

    public void SetTransactionInteractive(bool _active)
    {
        if (transactionButton.gameObject == null) return;
        transactionButton.interactable = _active;
    }

    public void SetProductionInteractive(bool _active)
    {
        if (productionButton.gameObject == null) return;
        productionButton.interactable = _active;
    }

    public void SetDistributionInteractive(bool _active)
    {
        if (distributionButton.gameObject == null) return;
        distributionButton.interactable = _active;
    }
}
