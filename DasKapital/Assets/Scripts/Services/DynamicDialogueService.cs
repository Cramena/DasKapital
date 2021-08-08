using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DynamicTextBubble
{
    OwnerLeft = 0,
    OwnerRight = 1,
    WorkerLeft = 2,
    WorkerRight = 3
}
public class DynamicDialogueService : MonoBehaviour
{
    public static DynamicDialogueService instance;
    [System.Serializable]
    public class DynamicLine
    {
        public DynamicTextBubble bubble;
        public string text;
    }
    [System.Serializable]
    public class DynamicDialogue
    {
        public bool waitForCommodityCreation;
        public List<DynamicLine> lines = new List<DynamicLine>();
    }

    public List<DynamicBubble> bubbles = new List<DynamicBubble>();

    public List<DynamicDialogue> dynamicDialogues = new List<DynamicDialogue>();
    private DynamicDialogue currentDialogue;
    private int lineIndex;
    public float dynamicDialogueTimeInterval = 0.5f;
    public float dynamicDialogueInitialWaitTime = 0.5f;
    private float dynamicDialogueTimer;
    private bool timingTimeInterval;
    private bool timingWaitTime;

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
        ScenarioService.instance.onNodeStep += Clearbubbles;
    }

    void Update()
    {
        if (timingTimeInterval)
        {
            if (dynamicDialogueTimer < dynamicDialogueTimeInterval)
            {
                dynamicDialogueTimer += Time.deltaTime;
            }
            else
            {
                dynamicDialogueTimer = 0;
                timingTimeInterval = false;
                LaunchDynamicLine();
            }
        }
        if (timingWaitTime)
        {
            if (dynamicDialogueTimer < dynamicDialogueInitialWaitTime)
            {
                dynamicDialogueTimer += Time.deltaTime;
            }
            else
            {
                dynamicDialogueTimer = 0;
                timingWaitTime = false;
                LaunchDynamicLine();
            }
        }
    }

    public void Clearbubbles()
    {
        foreach (DynamicBubble bubble in bubbles)
        {
            if (bubble.gameObject.activeSelf)
            {
                bubble.Die();
            }
        }
        if (timingTimeInterval) timingTimeInterval = false;
        if (timingWaitTime) timingWaitTime = false;
    }
    
    public void LaunchDynamicDialogue(int _index)
    {
        currentDialogue = dynamicDialogues[_index];
        lineIndex = 0;
        dynamicDialogueTimer = 0;
        foreach (DynamicBubble bubble in bubbles)
        {
            if (bubble.gameObject.activeSelf)
            {
                Clearbubbles();
                timingTimeInterval = true;
                return;
            }
        }
        timingWaitTime = true;
        // LaunchDynamicLine();
    }

    void LaunchDynamicLine()
    {
        DynamicBubble _bubble = bubbles[(int)currentDialogue.lines[lineIndex].bubble];
        _bubble.gameObject.SetActive(true);
        _bubble.InitializeBubble(currentDialogue.lines[lineIndex].text);

        lineIndex++;
        if (lineIndex < currentDialogue.lines.Count)
        {
            timingTimeInterval = true;
        }
    }

    public void CheckCommodityDialogue(CommoditySO _type)
    {
        if (_type.commodityName == "Punch")
        {
            LaunchDynamicDialogue(5);
        }
    }
}
