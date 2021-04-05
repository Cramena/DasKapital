using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum CommodityState
{
    Idle,
    Drag,
    Lerp
}

public class Commodity : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite plusValueIcon;
    public Image icon;
    public List<UseWidget> usesIcons = new List<UseWidget>();
    public RectTransform rect;
    public UITarget lastTarget;
    public UITarget target;
    public CommodityProfile profile;
    [HideInInspector] public CommodityState state;
    public int currentStock;
    public CommoditySO type;
    public CommodityBody body;
    public int plusValue;
    public bool canClick;
    public static System.Action<Commodity> onClick;
    public delegate bool OnDurableUsed();
    public OnDurableUsed onDurableUsed;
    public int value;
    public float lerpSnapThreshold = 0.2f;
    public float lerpSpeed = 0.2f;
    public float currentLerpSpeed;
    public float infoPanelTriggerDelay = 0.5f;
    private float infoPanelTriggerTimer;
    [HideInInspector] public bool hovering;
    [HideInInspector] public bool draggable = true;

    private void Awake()
    {
        canClick = true;
        InitializeProfile(type);
    }

    public void InitializeProfile(CommoditySO _type)
    {
        type = _type;
        profile.type = _type;
        profile.commodityName = type.commodityName;
        profile.exchangeValue = type.exchangeValue;
        profile.useValue = type.useValue;
        profile.useValueDescription = type.useValueDescription;
        profile.isDurable = type.isDurable;
        profile.usesAmount = type.usesAmount;
        profile.initialUsesAmount = type.usesAmount;
        profile.valuePerUse = profile.usesAmount != 0 ? profile.exchangeValue / profile.usesAmount : 0;
        profile.icon = type.icon;
        icon.sprite = profile.icon;
        profile.sizeModifier = type.sizeModifier;
        icon.transform.localScale = Vector3.one * profile.sizeModifier;
        profile.color = type.color;
        
        if (profile.isDurable) SetUsesUI();
    }

    private void OnEnable()
    {
        if (profile.isDurable) SetUsesUI();
    }

    public void SetUsesUI()
    {
        for (var i = 0; i < profile.initialUsesAmount; i++)
        {
            usesIcons[i].gameObject.SetActive(true);
            if (i < profile.usesAmount)
            {
                usesIcons[i].InitializeUse(true, profile.valuePerUse);
            }
            else
            {
                usesIcons[i].InitializeUse(false, profile.valuePerUse);
            }
        }
    }

    public void TransferComponentsValue(List<CommodityProfile> _componentsProfiles)
    {
        profile.exchangeValue = 0;
        foreach (CommodityProfile _profile in _componentsProfiles)
        {
            profile.components.Add(new CommodityProfile(_profile));
            profile.exchangeValue += _profile.usesAmount != 0 ? _profile.exchangeValue / _profile.usesAmount : _profile.exchangeValue;
            if (_profile.useValue == UseValue.AddValue)
            {
                profile.components.Add(new CommodityProfile(CommoditiesService.instance.plusValue));
                profile.exchangeValue += plusValue;
            }
        }
        profile.valuePerUse = profile.isDurable ? profile.exchangeValue / profile.usesAmount : 0;
        value = profile.exchangeValue;
    }

    private void Update()
    {
        if (hovering && !UIService.instance.infoPanelDisplaying && state != CommodityState.Drag)
        {
            infoPanelTriggerTimer += Time.deltaTime;
            if (infoPanelTriggerTimer >= infoPanelTriggerDelay)
            {
                UIService.instance.DisplayInfoPanel(this);
                infoPanelTriggerTimer = 0;
            }
        }
        if (target == null) return;
        
        switch (state)
        {
            case CommodityState.Idle:
                if (rect.position != target.rect.position) rect.position = target.rect.position; 
                break;
            case CommodityState.Lerp:
                LerpToTarget();
                break;
            default:
                break;
        }
    }

    void LerpToTarget()
    {
        if (Vector2.Distance(rect.position, target.rect.position) > lerpSnapThreshold)
        {
            rect.position = Vector2.Lerp(rect.position, target.rect.position, currentLerpSpeed * Time.deltaTime);
        }
        else
        {
            rect.position = target.rect.position;
            state = CommodityState.Idle;
        }
    }

    public void StartLerp()
    {
        currentLerpSpeed = lerpSpeed;
        state = CommodityState.Lerp;
        transform.SetAsLastSibling();
    }

    public void StartLerp(float _speed)
    {
        currentLerpSpeed = _speed;
        state = CommodityState.Lerp;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData pointerEventData)
    {
        if (!draggable) return;
        rect.position = pointerEventData.position;
        transform.SetAsLastSibling();
    }

    public void OnBeginDrag(PointerEventData pointerEventData)
    {
        if (!draggable) return;
        state = CommodityState.Drag;
    }

    public void OnEndDrag(PointerEventData pointerEventData)
    {
        if (!draggable) return;
        List<RaycastResult> results = new List<RaycastResult>();

        UIService.instance.graphicRaycatser.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Target"))
            {
                UITarget targetScript = result.gameObject.GetComponent<UITarget>();
                Stock stockCast = targetScript.owner as Stock;
                Stock currentStockCast = target.owner as Stock;
                if (!ScenarioService.instance.inProductionPhase &&
                    //No direct movement from one base stock to another base stock
                    ((targetScript.stockID != target.stockID && stockCast != null && currentStockCast != null) || 
                    //No direct movement from left to right or from right to left
                    Mathf.Sign(target.stockID) != Mathf.Sign(targetScript.stockID) ||
                    //No direct movement from trade stock to a base stock from which the commodity doesn't orginate
                    (currentStockCast == null && stockCast != null && targetScript.stockID != lastTarget.stockID) ||
                    //No direct movement from base stock to trade stock if another base stock
                    //already has at least one commodity of its own inside
                    (currentStockCast != null && stockCast == null && target.stockID >= 0 &&
                    target.stockID != ExchangeService.instance.otherStockIndex && ExchangeService.instance.otherStockIndex != -1)))
                    break;
                if (targetScript.OnCommodityPlaced(this)) break;
            }
        }
        StartLerp();
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hovering = false;
        infoPanelTriggerTimer = 0;
    }

    public bool OnUsed()
    {
        if (type.isDurable)
        {
            return onDurableUsed.Invoke();
        }
        else
        {
            Destroy(gameObject);
            return false;
        }
    }

    private void OnDestroy()
    {
        CommoditiesService.instance.CheckWorkforceEmptied(type);
    }
}
