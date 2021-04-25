using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Stock : UIOwner
{
    public List<UITarget> spawnTargets = new List<UITarget>();
    public List<CommoditySO> spawnList = new List<CommoditySO>();
    private int myVar;
    public List<CommoditySO> Commodities
    {
        get 
        {
            List<CommoditySO> commodities = new List<CommoditySO>();
            foreach (UITarget target in spawnTargets)
            {
                if (target.loadedCommodity == null) continue;
                commodities.Add(target.loadedCommodity.type);
            } 
            return commodities;
        }
    }
    

    private void Start() 
    {
        SpawnCommodities();
    }

    private void OnEnable()
    {
        if (spawnList.Count > 0) return;
        foreach (UITarget target in spawnTargets)
        {
            if (target.loadedCommodity != null) 
            {
                SetContentEnabled(true);
                return;
            }
        }
        SpawnRandomCommodities();
    }

    void SpawnRandomCommodities()
    {
        int commoditiesAmount = Random.Range(1, spawnTargets.Count);
        for (var i = 0; i < commoditiesAmount; i++)
        {
            Commodity commodityInstance = CommoditiesService.instance.SpawnCommodity(CommoditiesService.instance.GetRandomCommodity());
            commodityInstance.rect.position = spawnTargets[i].rect.position;
            spawnTargets[i].OnCommodityPlaced(commodityInstance, true);
        }
    }

    public void RandomizeContent()
    {
        DestroyContent();
        SpawnRandomCommodities();
    }

    void SpawnCommodities()
    {
        for (var i = 0; i < spawnList.Count; i++)
        {   
            if (spawnList[i] == null) continue;
            Commodity commodityInstance = CommoditiesService.instance.SpawnCommodity(spawnList[i]);
            commodityInstance.rect.position = spawnTargets[i].rect.position;
            spawnTargets[i].OnCommodityPlaced(commodityInstance);
        }
        spawnList.Clear();
    }

    public void GetCommodities(List<Commodity> _commodities)
    {
        List<Commodity> copies = new List<Commodity>(_commodities);
        List<UITarget> freeSlots = (from slot in spawnTargets
                                    where slot.loadedCommodity == null
                                    select slot).ToList();
        for (var i = 0; i < copies.Count; i++)
        {
            copies[i].StartLerp();
            freeSlots[i].OnCommodityPlaced(copies[i]);
        }
    }

    public int GetFreeSlotsAmount()
    {
        IEnumerable<UITarget> freeSlots = from slot in spawnTargets
                                          where slot.loadedCommodity == null
                                          select slot;
        return freeSlots.Count();
    }

    private void OnDisable()
    {
        SetContentEnabled(false);
    }

    void SetContentEnabled(bool _enabled)
    {
        foreach (UITarget target in spawnTargets)
        {
            if (target.loadedCommodity == null) continue;
            target.loadedCommodity.gameObject.SetActive(_enabled);
        }
    }

    public void DestroyContent()
    {
        foreach (UITarget target in spawnTargets)
        {
            target.DestroyCommodity();
        }
    }

    public override bool CheckCanLoad(Commodity _commodity)
    {
        if (_commodity.type.index == 1)
            return false;
        else
            return true;
    }
}
