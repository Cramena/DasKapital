using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockPillars : MonoBehaviour
{
    private Animator animator;
    public Appearable mockMarketPillar;
    public Appearable mockPrivatePillar;
    private int currentStep;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void NextTargetStep()
    {
        currentStep++;
        StartAnimWithIndex(currentStep);
    }

    private void StartAnimWithIndex(int _index)
    {
        switch (_index)
        {
            case 1 :
                mockMarketPillar.gameObject.SetActive(true);
                mockPrivatePillar.gameObject.SetActive(true);
                break;
            case 2 :
                mockPrivatePillar.LaunchDisappear();
                animator.SetTrigger("CenterOnMockMarket");
                break;
            case 3 :
                mockMarketPillar.LaunchDisappear();
                break;
            default:
                throw new System.Exception($"Error: Start unknown anim n°{_index}");
        }
    }
}
