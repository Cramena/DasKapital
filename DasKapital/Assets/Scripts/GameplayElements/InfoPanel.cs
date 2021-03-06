using System.Collections;
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

    public Animator lockAnimator;
    public Text title;
    public Image icon;
    public Text exchangeValueText;
    public Text useValueText;
    public GameObject durablePanel;
    public Text usesText;
    public List<UseWidget> uses = new List<UseWidget>();
    public RectTransform jauge;
    public Appearable appearable;

    public List<Color> jaugeColors = new List<Color>();
    public float horizontalOffset = 50;
    public float hideDelay = 0.5f;
    private float hideTimer;
    public bool hovering;
    public List<CommodityProfile> tempProfiles;
    private bool disablePending;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        appearable = GetComponent<Appearable>();
    }

    private void Update()
    {
        CheckHover();
    }

    private void OnDisable()
    {
        hideTimer = 0;
        disablePending = false;
    }

    void CheckHover()
    {
        if (disablePending || UIService.instance.infoPanelLocked) return;

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

    public void LaunchDisable()
    {
        appearable.LaunchDisappear();
        if (commodity != null)
        {
            commodity.ResetInfoPanelTimer();
        }
        disablePending = true;
    }

    public void LaunchLockAnim()
    {
        //Set lock animation
        if (UIService.instance.infoPanelLocked)
        {
            lockAnimator.SetTrigger("Lock");
        }
    }

    public void Initialize(Commodity _commodity)
    {
        commodity = _commodity;
        // GetComponent<Animator>().SetTrigger("OnEnable");
        ScenarioService.instance.OnCommodityInspected(_commodity.type);

        //Position
        Vector3 pos = rect.position;
        pos.x = commodity.rect.position.x > Screen.width / 2 ?
                commodity.rect.position.x - (Screen.width*horizontalOffset) :
                commodity.rect.position.x + (Screen.width*horizontalOffset);
        rect.position = pos;

        //Main info panel
        string comIndex = commodity.type.index.ToString("000");
        title.text = LocalisationService.instance.Translate($"COMNAME_{comIndex}"); //commodity.profile.commodityName;
        icon.sprite = commodity.profile.icon;
        icon.preserveAspect = true;
        exchangeValueText.text = $"{LocalisationService.instance.Translate("UI_EXCHVAL")} {commodity.profile.exchangeValue.ToString()}";
        useValueText.text = LocalisationService.instance.Translate($"COMDESC_{comIndex}");//commodity.profile.useValueDescription;

        //Durable panel
        if (commodity.profile.isDurable)
        {
            durablePanel.SetActive(true);
            usesText.text = LocalisationService.instance.Translate("UI_USENB");
            // usesText.text = commodity.profile.usesAmount == 1 ? "1 emploi restant:" : $"{commodity.profile.usesAmount} emplois restants:";
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
            int currentIndex = tempProfiles[0].type.index;
            int toSpawnAmount = 0;
            for (var i = 0; i < tempProfiles.Count; i++)
            {
                toSpawnAmount++;
                if ((i+1 < tempProfiles.Count && tempProfiles[i+1].type.index != currentIndex) || i+1 == tempProfiles.Count)
                {
                    if (i+1 < tempProfiles.Count) 
                    {
                        currentIndex = tempProfiles[i+1].type.index;
                    }

                    JaugeSegment segment = Instantiate(segmentPrefab, jauge);
                    string nameText = "";
                    string pluralAnnex = toSpawnAmount == 1 || (tempProfiles[i].type.index == 0 || tempProfiles[i].type.index == 106) ? "" : "s";
                    int value = tempProfiles[i].isDurable ? tempProfiles[i].valuePerUse * toSpawnAmount : tempProfiles[i].exchangeValue * toSpawnAmount;
                    string tempComIndex = tempProfiles[i].type.index.ToString("000");
                    string tempName = LocalisationService.instance.Translate($"COMNAME_{tempComIndex}");
                    string of = LocalisationService.instance.Translate("UI_OF");
                    string from = LocalisationService.instance.Translate("UI_FROM");
                    string use = LocalisationService.instance.Translate("UI_USE");
                    if (tempProfiles[i].isDurable)
                    {
                        nameText = $"{value} {from} ({toSpawnAmount}) {use}{pluralAnnex} {of} {tempName}";
                    }
                    else
                    {
                        nameText = $"{value} {from} ({toSpawnAmount}) {tempName}{pluralAnnex}";
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
