﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

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
    public List<CommodityProfile> tempProfiles;

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
        ScenarioService.instance.OnCommodityInspected(_commodity.type);

        //Position
        Vector3 pos = rect.position;
        pos.x = commodity.rect.position.x > Screen.width / 2 ?
                commodity.rect.position.x - (Screen.width*horizontalOffset) :
                commodity.rect.position.x + (Screen.width*horizontalOffset);
        rect.position = pos;

        //Main info panel
        title.text = commodity.profile.commodityName;
        icon.sprite = commodity.profile.icon;
        exchangeValueText.text = $"Value: {commodity.profile.exchangeValue.ToString()}";
        useValueText.text = commodity.profile.useValueDescription;

        //Durable panel
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

        //Value jauge
        if (!ScenarioService.instance.valueJaugeActive)
        {
            return;
        }
        if (commodity.profile.components.Count == 0)
        {
            JaugeSegment segment = Instantiate(segmentPrefab, jauge);
            segment.InitializeSegment(commodity.profile.valuePerUse * commodity.profile.usesAmount, commodity.profile.icon, 1,
                                      commodity.profile.commodityName, commodity.profile.color, commodity.profile.sizeModifier);
        }
        else
        {
            tempProfiles.Clear();
            for (var i = 0; i < commodity.profile.components.Count; i++)
            {
                GetValueFromCommodity(commodity.profile.components[i]);
            }
            tempProfiles = tempProfiles.OrderBy(x => x.type.index).ToList();
            int currentIndex = 0;
            int toSpawnAmount = 0;
            for (var i = 0; i < tempProfiles.Count; i++)
            {
                toSpawnAmount++;
                if ((i+1 < tempProfiles.Count &&tempProfiles[i+1].type.index != currentIndex) || i+1 == tempProfiles.Count)
                {
                    if (i+1 < tempProfiles.Count) currentIndex = tempProfiles[i+1].type.index;
                    JaugeSegment segment = Instantiate(segmentPrefab, jauge);
                    string nameText = "";
                    string pluralAnnex = toSpawnAmount == 1 && tempProfiles[i].type.index == 0 ? "" : "s";
                    int value = tempProfiles[i].isDurable ? tempProfiles[i].valuePerUse * toSpawnAmount : tempProfiles[i].exchangeValue * toSpawnAmount;
                    if (tempProfiles[i].isDurable)
                    {
                        nameText = $"{value} from ({toSpawnAmount}) {tempProfiles[i].commodityName} use{pluralAnnex}";
                    }
                    else
                    {
                        nameText = $"{value} from ({toSpawnAmount}) {tempProfiles[i].commodityName}{pluralAnnex}";
                    }
                    segment.InitializeSegment(value, tempProfiles[i].icon, toSpawnAmount, nameText, tempProfiles[i].color, tempProfiles[i].sizeModifier);
                    toSpawnAmount = 0;
                }
            }
        }
    }

    void GetValueFromCommodity(CommodityProfile _profile)
    {
        if (_profile.components.Count == 0)
        {
            tempProfiles.Add(_profile);
        }
        else
        {
            for (var i = 0; i < _profile.components.Count; i++)
            {
                GetValueFromCommodity(_profile.components[i]);
            }
        }
    }
}
