using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;

public enum Condition
{
    None,
    Button,
    Produce,
    Info
}
public class ScenarioService : MonoBehaviour
{
    public static ScenarioService instance;
    private Animator animator;
    public List<TMP_Text> scenarioTexts = new List<TMP_Text>();
    public Appearable continueButton;
    public Button transactionButton;
    public Button productionButton;
    public Button previousButton;
    public Animator interlocutorAnimator;
    public AutoSellStock autoSellStock;
    public AsteriskPanel asteriskPanel;
    public List<ScenarioNode> nodes = new List<ScenarioNode>();
    private int maxProgress;
    private int currentNodeIndex;
    public List<UnityEvent> scenarioEvents = new List<UnityEvent>();
    public Condition currentCondition;
    public bool delayedDistribution;
    public bool valueJaugeActive;
    public bool inProductionPhase;
    public bool allowBaseCommoditiesAutoSell;
    public System.Action<CommoditySO> onProduceRegistered;
    public System.Action<CommoditySO> onAutoSell;
    public System.Action onDistribution;
    public System.Action<CommoditySO> onCommodityInspected;
    public System.Action onSandboxStart;
    // private int scenarioIndex;
    public bool sandboxActive;
    public bool manualProgress;
    public bool workforceIntroduced;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            throw new System.Exception($"Too many {this} instances");
        }
    }

    private void Start()
    {
        // scenarioIndex = 0;
        animator = GetComponent<Animator>();
        nodes[maxProgress]?.OnNodeEntered(false);
        SetContinueButtonActive(true);
    }

    private void Update()
    {
        CheckAsterisk();
        if (Input.GetKeyDown(KeyCode.Space) && manualProgress)
        {
            OnNodeStep();
        }
    }

    private void SetRewindButton(bool _activate)
    {
        if (_activate)
        {
            previousButton.gameObject.SetActive(true);
            previousButton.interactable = true;
        }
        else
        {
            previousButton.GetComponent<Appearable>().LaunchDisappear();
            previousButton.interactable = false;
        }
    }

    public void OnNodeStep()
    {
        //Regular progression
        if (currentNodeIndex == maxProgress)
        {
            currentNodeIndex++;
            maxProgress = currentNodeIndex;
            if (nodes[currentNodeIndex].hasCondition)
            {
                SetContinueButtonActive(false);
            }
            else
            {
                SetContinueButtonActive(true);
            }
            nodes[currentNodeIndex]?.OnNodeEntered(false);
        }
        //Catching up
        else
        {
            currentNodeIndex++;
            //Just caught up
            if (currentNodeIndex == maxProgress)
            {
                if (nodes[currentNodeIndex].hasCondition)
                {
                    SetContinueButtonActive(false);
                }
                else
                {
                    SetContinueButtonActive(true);
                }
            }
            else
            {
                SetContinueButtonActive(true);
            }
            nodes[currentNodeIndex]?.OnNodeEntered(true);
        }


        // if (currentNodeIndex == maxProgress)
        // {
        //     // nodes[maxProgress]?.OnNodeLeft();
        //     currentCondition = Condition.None;
        //     // SetContinueButtonActive(true);
        //     maxProgress++;
        //     currentNodeIndex++;
        //     if (maxProgress < nodes.Count)
        //     {
        //         nodes[maxProgress]?.OnNodeEntered(false);
        //     }
        //     else 
        //     {
        //         previousButton.GetComponent<Appearable>().LaunchDisappear();
        //         previousButton.interactable = false;
        //     }
        // }
        // else
        // {
        //     currentNodeIndex++;
        //     if (maxProgress < nodes.Count)
        //     {
        //         nodes[currentNodeIndex]?.OnNodeEntered(true);
        //     }
        // }
    }

    public void OnNodeRewind()
    {
        if (currentNodeIndex > 1)
        {
            if (currentNodeIndex == 2)
            {
                SetRewindButton(false);
                // previousButton.GetComponent<Appearable>().LaunchDisappear();
                // previousButton.interactable = false;
            }
            else
            {
                SetRewindButton(true);
            }
            currentNodeIndex--;
            nodes[currentNodeIndex]?.OnNodeRewind();
            SetContinueButtonActive(true);
        }
        // if (/*scenarioIndex*/currentNodeIndex-1 <= 1)
        // {
        //     previousButton.GetComponent<Appearable>().LaunchDisappear();
        //     previousButton.interactable = false;
        // }
    }

    public void OnSandboxStart()
    {
        onSandboxStart?.Invoke();
        sandboxActive = true;
    }

    void CheckAsterisk()
    {
        foreach (TMP_Text scenarioText in scenarioTexts)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(scenarioText, Input.mousePosition, Camera.main);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = scenarioText.textInfo.linkInfo[linkIndex];
                string key = linkInfo.GetLinkID();
                if (!asteriskPanel.gameObject.activeSelf || key != asteriskPanel.currentKey)
                {
                    asteriskPanel.gameObject.SetActive(true);
                    asteriskPanel.InitializePanel(key);
                }
                return;
            }
        }
        if (asteriskPanel.appearable.displayed)
        {
            asteriskPanel.appearable.LaunchDisappear();
        }
    }

    public void PreviousLine(string _key)
    {
        // scenarioIndex--;
        string key = "SCE_" + (currentNodeIndex).ToString("000");
        if (currentNodeIndex % 2 == 0)
        {
            scenarioTexts[0].text = LocalisationService.instance.Translate(key);
            animator.SetTrigger("TwoSwipeUp");
        }
        else
        {
            scenarioTexts[1].text = LocalisationService.instance.Translate(key);
            animator.SetTrigger("OneSwipeUp");
        }
        interlocutorAnimator.SetTrigger("Talk");
    }

    public void NextLine(string _key)
    {
        if (currentNodeIndex-1 > 0)
        {
            previousButton.gameObject.SetActive(true);
            previousButton.interactable = true;
        }
        // scenarioIndex++;
        string key = "SCE_" + (currentNodeIndex).ToString("000");
        if (currentNodeIndex % 2 == 0)
        {
            print("Key is" + key);
            scenarioTexts[0].text = LocalisationService.instance.Translate(key);
            animator.SetTrigger("OneSwipeDown");
        }
        else
        {
            print("Key is" + key);
            scenarioTexts[1].text = LocalisationService.instance.Translate(key);
            animator.SetTrigger("TwoSwipeDown");
        }
        interlocutorAnimator.SetTrigger("Talk");
    }

    public void LaunchEvent(int _index)
    {
        scenarioEvents[_index].Invoke();
    }

    public void SetDelayedDistribution(bool _active)
    {
        delayedDistribution = _active;
    }

    public void SetValueJaugeActive(bool _active)
    {
        valueJaugeActive = _active;
    }

    public void SetStockToStockMovement(bool _active)
    {
        inProductionPhase = _active;
    }

    public void SetContinueButtonActive(bool _active)
    {
        if (_active)
        {
            continueButton.gameObject.SetActive(_active);
            continueButton.GetComponent<Button>().interactable = true;
        }
        else
        {
            continueButton.GetComponent<Button>().interactable = false;
            continueButton.LaunchDisappear();
        }
        manualProgress = _active;
    }

    public void SetBaseCommoditiesAutoSellAllowed(bool _allowed)
    {
        allowBaseCommoditiesAutoSell = _allowed;
    }

    public void RegisterProduce(CommoditySO _type)
    {
        onProduceRegistered?.Invoke(_type);
    }

    public void OnAutoSell(CommoditySO _commodity)
    {
        onAutoSell?.Invoke(_commodity);
    }

    public void OnCommodityInspected(CommoditySO _type)
    {
        onCommodityInspected?.Invoke(_type);
    }

    public void LaunchCommodityEdit()
    {
        CommoditiesService.instance.EditCommodities();
    }

    public void SetTransactionInteractive(bool _active)
    {
        if (transactionButton.gameObject == null) return;
        transactionButton.interactable = _active;
    }

    public void SetProductionInteractive(bool _active)
    {
        if (productionButton.gameObject == null) return;
        productionButton.interactable = _active;
    }

    public void SetDistributionInteractive(bool _active)
    {
        autoSellStock.ActivateDistributionButton(_active);
    }

    public void IntroduceWorkforce()
    {
        workforceIntroduced = true;
    }

    public void HideSelf()
    {
        GetComponent<Image>().enabled = false;
        foreach (TMP_Text text in scenarioTexts)
        {
            text.enabled = false;
        }
        SetContinueButtonActive(false);
    }
}
