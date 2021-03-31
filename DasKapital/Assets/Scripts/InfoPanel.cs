using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InfoPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    private Commodity commodity;
    public JaugeSegment segmentPrefab;

    public Text title;
    public Image icon;
    public Text exchangeValueText;
    public Text useValueText;
    public GameObject durablePanel;
    public Text usesText;
    public List<UseWidget> uses = new List<UseWidget>();
    public RectTransform jauge;

    public List<Color> jaugeColors = new List<Color>();
    public float horizontalOffset = 50;
    public float hideDelay = 0.5f;
    private float hideTimer;
    public bool hovering;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        CheckHover();
    }

    private void OnDisable()
    {
        hideTimer = 0;
    }

    void CheckHover()
    {
        if (!hovering && !commodity.hovering)
        {
            hideTimer += Time.deltaTime;
            if (hideTimer >= hideDelay)
            {
                UIService.instance.HideInfoPanel();
            }
        }
        else if (hideTimer != 0)
        {
            hideTimer = 0;
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        hovering = false;
    }

    public void Initialize(Commodity _commodity)
    {
        commodity = _commodity;
        Vector3 pos = rect.position;
        pos.x = commodity.rect.position.x > Screen.width / 2 ?
                commodity.rect.position.x - (Screen.width*horizontalOffset) :
                commodity.rect.position.x + (Screen.width*horizontalOffset);
        rect.position = pos;

        title.text = commodity.profile.commodityName;
        icon.sprite = commodity.profile.icon;
        exchangeValueText.text = $"Value: {commodity.profile.exchangeValue.ToString()}";
        useValueText.text = commodity.profile.useValueDescription;
        if (commodity.profile.isDurable)
        {
            durablePanel.SetActive(true);
            usesText.text = commodity.profile.usesAmount == 1 ? "1 use left:" : $"{commodity.profile.usesAmount} uses left:";
            foreach (UseWidget use in uses)
            {
                use.gameObject.SetActive(false);
            }
            for (var i = 0; i < commodity.profile.initialUsesAmount; i++)
            {
                uses[i].gameObject.SetActive(true);
                if (i < commodity.profile.usesAmount)
                {
                    uses[i].InitializeUse(true, commodity.profile.valuePerUse);
                }
                else
                {
                    uses[i].InitializeUse(false, commodity.profile.valuePerUse);
                }
            }
        }
        else
        {
            durablePanel.SetActive(false);
        }

        float currentHeight = 0;
        Vector3 segmentPos = Vector3.zero;
        Vector3 segmentScale = Vector3.zero;
        if (commodity.profile.components.Count == 0)
        {
            JaugeSegment segment = Instantiate(segmentPrefab, jauge);
            segment.InitializeSegment(commodity.profile.valuePerUse * commodity.profile.usesAmount, commodity.profile.icon,
                                      commodity.profile.commodityName, commodity.profile.color, commodity.profile.sizeModifier);
        }
        else
        {
            for (var i = 0; i < commodity.profile.components.Count; i++)
            {
                JaugeSegment segment = Instantiate(segmentPrefab, jauge);
                CommodityProfile component = commodity.profile.components[i];

                string nameText = "";
                if (component.isDurable)
                {
                    nameText = $"({component.valuePerUse}) {component.commodityName} use";
                }
                else
                {
                    nameText = $"({component.exchangeValue}) {component.commodityName}";
                }
                int value = component.isDurable ? component.valuePerUse : component.exchangeValue;
                segment.InitializeSegment(value, component.icon, nameText, component.color, component.sizeModifier);
            }
            print($"currentHeight {currentHeight}");
        }
    }
}
