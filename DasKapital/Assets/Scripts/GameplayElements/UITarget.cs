using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UITarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int stockID;
    public UIOwner owner;
    public System.Action<Commodity> onCommodityPlaced;
    public System.Action<Commodity> onCommodityUnloaded;
    [HideInInspector] public RectTransform rect;
    [HideInInspector] public Commodity loadedCommodity;
    private Image image;
    public bool highlighted = true;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        if (highlighted) image.color = new Color(255, 255, 255, 0);
    }

    public bool OnCommodityPlaced(Commodity _commodity, bool force = false)
    {
        if (owner != null && (/*(!force && !owner.available) || */!owner.CheckCanLoad(_commodity))) return false;

        UITarget otherTarget = _commodity.target;
        if (otherTarget != null) otherTarget.UnloadCommodity();
        if (loadedCommodity != null)
        {
            Commodity lastLoadedCommodity = loadedCommodity;
            UnloadCommodity();
            if (!force && otherTarget != null)
            {
                otherTarget.OnCommodityPlaced(lastLoadedCommodity);
            }
        }
        if (_commodity.lastTarget == null || stockID != _commodity.target.stockID)
            _commodity.lastTarget = otherTarget;
        _commodity.target = this;
        loadedCommodity = _commodity;
        onCommodityPlaced?.Invoke(loadedCommodity);
        CommoditiesService.instance.RegisterCommodityPlacement();
        return true;
    }

    public void UnloadCommodity()
    {
        onCommodityUnloaded?.Invoke(loadedCommodity);
        loadedCommodity = null;
    }

    public void SetHighlight(bool _active)
    {
        if (!highlighted) return;
        if (_active)
        {
            image.color = new Color(255, 255, 255, 1);
        }
        else
        {
            image.color = new Color(255, 255, 255, 0);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        SetHighlight(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        SetHighlight(false);
    }
    
    public void DestroyCommodity()
    {
        if (loadedCommodity != null)
        {
            Destroy(loadedCommodity.gameObject);
        }
    }
}
