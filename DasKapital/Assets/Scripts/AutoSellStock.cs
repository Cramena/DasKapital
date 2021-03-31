using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AutoSellStock : UIOwner
{
    public DistributionCommodity distributionPrefab;
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
    private bool distributing;
    private CoinType currentStep;
    // public bool tuto;

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
        spawnedCoins.Clear();
        available = false;
        int coinsToDistribute = _commodity.profile.exchangeValue;
        foreach (CommodityProfile profile in _commodity.profile.components)
        {
            int value = profile.isDurable ? profile.valuePerUse : profile.exchangeValue;
            if (profile.type == CommoditiesService.instance.workforce)
            {
                SpawnCoins(CoinType.Salary, value);
            } 
            else if (profile.type == CommoditiesService.instance.plusValue)
            {
                SpawnCoins(CoinType.Profit, value);
            } 
            else
            {
                SpawnCoins(CoinType.Material, value);
            }
        }
        spawnedCoins = spawnedCoins.OrderBy(x => x.type).ToList();

        Vector3 _spawnPos = _commodity.rect.position;
        Destroy(_commodity.gameObject);

        for (var i = 0; i < spawnedCoins.Count; i++)
        {
            spawnedCoins[i].commodity.rect.position = _spawnPos;
            spawnedCoins[i].commodity.StartLerp(coinsPopSpeed);
            targets[i].OnCommodityPlaced(spawnedCoins[i].commodity, true);
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
                    if (ScenarioService.instance.delayedDistribution) distributing = false;
                    else currentStep = CoinType.Salary;
                } 
                break;
            case CoinType.Salary :
                salaryQueue.Dequeue().GetDistributedTo(distributionTargets[1]);
                if (salaryQueue.Count <= 0)
                {
                    if (ScenarioService.instance.delayedDistribution) distributing = false;
                    else currentStep = CoinType.Profit;
                } 
                break;
            case CoinType.Profit :
                profitQueue.Dequeue().GetDistributedTo(distributionTargets[2]);
                if (profitQueue.Count <= 0)
                {
                    currentStep = CoinType.None;
                    distributing = false;
                    available = true;
                } 
                break;
            default:
                break;
        }
    }

    public void ProcessDistribution()
    {
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
}
