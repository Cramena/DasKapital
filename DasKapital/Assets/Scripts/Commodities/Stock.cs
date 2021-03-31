using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Stock : UIOwner
{
    public List<UITarget> spawnTargets = new List<UITarget>();
    public List<CommoditySO> spawnList = new List<CommoditySO>();

    private void Start() 
    {
        SpawnCommodities();
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

    private void OnDisable()
    {
        foreach (UITarget target in spawnTargets)
        {
            target.DestroyCommodity();
        }
    }
}
