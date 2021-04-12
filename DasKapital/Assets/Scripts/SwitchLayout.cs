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
        if (onProductionLayout)
        {
            foreach (GameObject go in productionObjects)
            {
                go.SetActive(true);
            }
            foreach (GameObject go in exchangeObjects)
            {
                go.SetActive(false);
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
                go.SetActive(false);
            }
        }
    }
}
