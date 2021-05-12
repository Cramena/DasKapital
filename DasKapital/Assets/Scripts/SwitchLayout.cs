using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchLayout : MonoBehaviour
{
    public List<GameObject> exchangeObjects = new List<GameObject>();
    public List<GameObject> productionObjects = new List<GameObject>();
    public bool onProductionLayout = true;

    public void OnSwitch()
    {
        onProductionLayout = !onProductionLayout;
        ScenarioService.instance.inProductionPhase = onProductionLayout;
        if (onProductionLayout)
        {
            foreach (GameObject go in productionObjects)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in exchangeObjects)
            {
                Appearable appearable = go.GetComponent<Appearable>();
                if (appearable == null)
                {
                    go.SetActive(false);
                }
                else
                {
                    appearable.LaunchDisappear();
                }
            }
        }
        else
        {
            foreach (GameObject go in exchangeObjects)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in productionObjects)
            {
                Appearable appearable = go.GetComponent<Appearable>();
                if (appearable == null)
                {
                    go.SetActive(false);
                }
                else
                {
                    appearable.LaunchDisappear();
                }
            }
        }
    }
}
