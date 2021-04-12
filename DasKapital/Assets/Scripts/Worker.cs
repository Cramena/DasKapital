using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Worker : MonoBehaviour
{
    private RectTransform rect;
    public MeanOfProduction meanOfProduction;
    public Commodity workforcePrefab;
    public UITarget workforceSlot;
    public float workforcePopSpeed = 5;
    private Queue<Commodity> workforces = new Queue<Commodity>();

    private void Start() 
    {
        rect = GetComponent<RectTransform>();
        ReplenishPool();
        PopWorkForce();
    }

    void ReplenishPool()
    {
        for (var i = 0; i < 20; i++)
        {
            Commodity newWorkforce = Instantiate(workforcePrefab, CommoditiesService.instance.transform);
            newWorkforce.rect.position = rect.position;
            newWorkforce.gameObject.SetActive(false);
            workforces.Enqueue(newWorkforce);
        }
    }

    public void PopWorkForce()
    {
        Commodity spawnedWorkforce = workforces.Dequeue();
        spawnedWorkforce.gameObject.SetActive(true);
        if (workforces.Count < 5) ReplenishPool();

        spawnedWorkforce.rect.position = rect.position;
        spawnedWorkforce.StartLerp(workforcePopSpeed);
        workforceSlot.OnCommodityPlaced(spawnedWorkforce);
    }
    private void OnEnable()
    {
        SetContentEnabled(true);
    }

    private void OnDisable()
    {
        SetContentEnabled(false);
    }

    void SetContentEnabled(bool _enabled)
    {
        if (workforceSlot.loadedCommodity != null) 
        {
            workforceSlot.loadedCommodity.gameObject.SetActive(_enabled);
        }
    }
}
