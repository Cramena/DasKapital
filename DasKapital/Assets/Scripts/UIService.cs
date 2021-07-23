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
    public RecipesPanel recipesPanel;
    public bool infoPanelDisplaying;
    public bool recipePanelDisplaying;
    [HideInInspector] public UITarget highlightedTarget;
    private PointerEventData pointerEventData;
    public bool infoPanelLocked;
    public System.Action onInfoPanelLocked;

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
        onInfoPanelLocked += infoPanel.LaunchLockAnim;
    }

    private void Start()
    {
        HideInfoPanel();
    }

    private void Update()
    {
        if (infoPanelDisplaying &&
            ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) 
            && !infoPanel.hovering && !infoPanelLocked))
        {
            HideInfoPanel();
        }
    }

    public void DisplayInfoPanel(Commodity _commodity)
    {
        // if (infoPanelDisplaying) return;
        infoPanel.gameObject.SetActive(true);
        infoPanel.Initialize(_commodity);
        infoPanelDisplaying = true;
    }

    public void HideInfoPanel()
    {
        infoPanel.LaunchDisable();
        // infoPanel.gameObject.SetActive(false);
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

    public void ShowRecipe(Recipe _recipe)
    {
        recipesPanel.gameObject.SetActive(true);
        recipesPanel.InitializePanel(_recipe);
        Canvas.ForceUpdateCanvases();
        recipePanelDisplaying = true;
    }

    public void HideRecipe()
    {
        recipesPanel.appearable.LaunchDisappear();
        // recipesPanel.gameObject.SetActive(false);
        recipePanelDisplaying = false;
    }

    public void SetInfoPanelLocked(bool _lock)
    {
        infoPanelLocked = _lock;
        if (_lock)
        {
            onInfoPanelLocked.Invoke();
        }
    }
}
