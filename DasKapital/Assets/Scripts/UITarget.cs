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
        if (!force && owner != null && !owner.available) return false;

        if (loadedCommodity != null)
        {
            if (!force) return false;
            UnloadCommodity();
        }
        if (_commodity.target != null) _commodity.target.UnloadCommodity();
        _commodity.lastTarget = _commodity.target;
        _commodity.target = this;
        loadedCommodity = _commodity;
        onCommodityPlaced?.Invoke(loadedCommodity);
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
}
