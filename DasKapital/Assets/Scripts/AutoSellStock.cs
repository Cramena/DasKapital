using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class AutoSellStock : UIOwner
{
    public DistributionCommodity distributionPrefab;
    public Button distributionButton;
    public Stock personalStock;
    public List<CommoditySO> coinTypes = new List<CommoditySO>();
    public List<UITarget> targets = new List<UITarget>();
    public List<UITarget> distributionTargets = new List<UITarget>();
    public List<Commodity> loadedCommodities = new List<Commodity>();
    private List<DistributionCommodity> spawnedCoins = new List<DistributionCommodity>();
    private Queue<DistributionCommodity> materialQueue = new Queue<DistributionCommodity>();
    private Queue<DistributionCommodity> salaryQueue = new Queue<DistributionCommodity>();
    private Queue<DistributionCommodity> profitQueue = new Queue<DistributionCommodity>();
    public float coinsPopSpeed = 10;
    public float distributionDelay = 0.15f;
    private float distributionTimer;
    public bool distributing;
    public bool buttonActive;
    private CoinType currentStep;
    private int stockValue;

    void Start()
    {
        available = true;
        foreach (UITarget target in targets)
        {
            target.onCommodityPlaced += OnCommodityPlaced;
            target.onCommodityUnloaded += (Commodity _commodity) =>
            {
                if (loadedCommodities.Contains(_commodity))
                {
                    loadedCommodities.Remove(_commodity);
                }
            };
        }
    }

    public override bool CheckCanLoad(Commodity _commodity)
    {
        if ((!ScenarioService.instance.allowBaseCommoditiesAutoSell && _commodity.profile.components.Count == 0 &&
            _commodity.GetComponent<DistributionCommodity>() == null) ||
            _commodity.type.index == 1)
            return false;
        else
            return true;
    }

    public void OnCommodityPlaced(Commodity _commodity)
    {
        DistributionCommodity distributionItem = _commodity.GetComponent<DistributionCommodity>();
        if (distributionItem == null && available)
        {
            AutoSellCommodity(_commodity);
        }
    }

    void AutoSellCommodity(Commodity _commodity)
    {
        ScenarioService.instance.OnAutoSell(_commodity.type);
        spawnedCoins.Clear();
        available = false;
        int coinsToDistribute = _commodity.profile.exchangeValue;

        GetTypeByProfile(_commodity.profile);
        spawnedCoins = spawnedCoins.OrderBy(x => x.type).ToList();

        Vector3 _spawnPos = _commodity.rect.position;
        _commodity.animator.SetTrigger("Disappear");
        // Destroy(_commodity.gameObject);

        for (var i = 0; i < spawnedCoins.Count; i++)
        {
            spawnedCoins[i].commodity.rect.position = _spawnPos;
            spawnedCoins[i].commodity.StartLerp(coinsPopSpeed);
            targets[i].OnCommodityPlaced(spawnedCoins[i].commodity, true);
        }
    }

    void GetTypeByProfile(CommodityProfile _profile)
    {
        if (_profile.components.Count > 0)
        {
            foreach (CommodityProfile component in _profile.components)
            {
                GetTypeByProfile(component);
            }
        }
        else
        {
            int value = _profile.isDurable ? _profile.valuePerUse : _profile.exchangeValue;
            if (_profile.type == CommoditiesService.instance.workforce)
            {
                SpawnCoins(CoinType.Salary, value);
            }
            else if (_profile.type == CommoditiesService.instance.plusValue)
            {
                SpawnCoins(CoinType.Profit, value);
            }
            else
            {
                SpawnCoins(CoinType.Material, value);
            }
        }
    }

    void SpawnCoins(CoinType _type, int _amount)
    {
        for (var i = 0; i < _amount; i++)
        {
            DistributionCommodity instance = Instantiate(distributionPrefab, CommoditiesService.instance.transform);
            instance.Initialize(_type);
            spawnedCoins.Add(instance);
            switch(_type)
            {
                case CoinType.Material :
                    stockValue++;
                    materialQueue.Enqueue(instance);
                    break;
                case CoinType.Salary :
                    salaryQueue.Enqueue(instance);
                    break;
                case CoinType.Profit :
                    profitQueue.Enqueue(instance);
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        ManageDistributionTimer();
    }

    void ManageDistributionTimer()
    {
        if (distributing)
        {
            if (distributionTimer < distributionDelay)
            {
                distributionTimer += Time.deltaTime;
            }
            else
            {
                distributionTimer = 0;
                DistributeCoin();
            }
        }
    }

    void DistributeCoin()
    {
        switch(currentStep)
        {
            case CoinType.Material :
                materialQueue.Dequeue().GetDistributedTo(distributionTargets[0]);
                if (materialQueue.Count <= 0)
                {
                    if (ScenarioService.instance.delayedDistribution)
                    {
                        StartCoroutine(SpawnStockCoins());
                        distributing = false;
                        distributionButton.interactable = buttonActive;
                    }
                    else currentStep = CoinType.Salary;
                }
                break;
            case CoinType.Salary :
                if (salaryQueue.Count > 0) salaryQueue.Dequeue().GetDistributedTo(distributionTargets[1]);
                if (salaryQueue.Count <= 0)
                {
                    if (ScenarioService.instance.delayedDistribution)
                    {
                        distributing = false;
                        distributionButton.interactable = buttonActive;
                    }
                    else currentStep = CoinType.Profit;
                }
                break;
            case CoinType.Profit :
                if (profitQueue.Count > 0) profitQueue.Dequeue().GetDistributedTo(distributionTargets[2]);
                if (profitQueue.Count <= 0)
                {
                    currentStep = CoinType.None;
                    distributing = false;
                    available = true;
                    distributionButton.interactable = buttonActive;
                    if (!ScenarioService.instance.delayedDistribution)
                    {
                        StartCoroutine(SpawnStockCoins());
                    }
                }
                break;
            default:
                break;
        }
    }

    IEnumerator SpawnStockCoins()
    {
        yield return new WaitForSeconds(0.5f);

        while (stockValue >= 50)
        {
            stockValue -= 50;
            personalStock.AddCommodity(coinTypes[5]);
            yield return new WaitForSeconds(0.05f);
        }
        while (stockValue >= 20)
        {
            stockValue -= 20;
            personalStock.AddCommodity(coinTypes[4]);
            yield return new WaitForSeconds(0.05f);
        }
        while (stockValue >= 10)
        {
            stockValue -= 10;
            personalStock.AddCommodity(coinTypes[3]);
            yield return new WaitForSeconds(0.05f);
        }
        while (stockValue >= 5)
        {
            stockValue -= 5;
            personalStock.AddCommodity(coinTypes[2]);
            yield return new WaitForSeconds(0.05f);
        }
        while (stockValue >= 2)
        {
            stockValue -= 2;
            personalStock.AddCommodity(coinTypes[1]);
            yield return new WaitForSeconds(0.05f);
        }
        while (stockValue >= 1)
        {
            stockValue -= 1;
            personalStock.AddCommodity(coinTypes[0]);
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void ProcessDistribution()
    {
        ScenarioService.instance.onDistribution?.Invoke();
        if (!ScenarioService.instance.delayedDistribution)
        {
            distributing = true;
            currentStep = CoinType.Material;
            DistributeCoin();
        }
        else
        {
            distributing = true;
            switch(currentStep)
            {
                case CoinType.None:
                    currentStep = CoinType.Material;
                    break;
                case CoinType.Material:
                    currentStep = CoinType.Salary;
                    break;
                case CoinType.Salary:
                    currentStep = CoinType.Profit;
                    break;
            }
            DistributeCoin();
        }
    }

    public void ActivateDistributionButton(bool _interactable)
    {
        buttonActive = _interactable;
        if (!distributing)
        {
            distributionButton.interactable = buttonActive;
        }
    }
}
