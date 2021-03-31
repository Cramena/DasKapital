using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIService : MonoBehaviour
{
    public static UIService instance;
    [HideInInspector] public GraphicRaycaster graphicRaycatser;
    [HideInInspector] public EventSystem eventSystem;
    public InfoPanel infoPanel;
    public Transform coinsPanel;
    [HideInInspector] public bool infoPanelDisplaying;
    [HideInInspector] public UITarget highlightedTarget;
    private PointerEventData pointerEventData;

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
        graphicRaycatser = GetComponent<GraphicRaycaster>();
    }

    private void Start()
    {
        HideInfoPanel();
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !infoPanel.hovering)
        {
            HideInfoPanel();
        }
    }

    public void DisplayInfoPanel(Commodity _commodity)
    {
        infoPanel.gameObject.SetActive(true);
        infoPanel.Initialize(_commodity);
        infoPanelDisplaying = true;
    }

    public void HideInfoPanel()
    {
        infoPanel.gameObject.SetActive(false);
        infoPanelDisplaying = false;
    }

    private void FixedUpdate()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        UIService.instance.graphicRaycatser.Raycast(pointerEventData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject.CompareTag("Target"))
            {
                UITarget target = result.gameObject.GetComponent<UITarget>();
                if (highlightedTarget != target)
                {
                    if (highlightedTarget != null) highlightedTarget.SetHighlight(false);
                    highlightedTarget = target;
                }
                target.SetHighlight(true);
                return;
            }
        }
        if (highlightedTarget != null) 
        {
            highlightedTarget.SetHighlight(false);
            highlightedTarget = null;
        }
    }
}
