using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchLayout : MonoBehaviour
{
    private Button button;
    public List<GameObject> exchangeObjects = new List<GameObject>();
    public List<GameObject> productionObjects = new List<GameObject>();
    public bool onProductionLayout = true;
    public float buttonCooldown = 0.25f;
    private float buttonTimer;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Update()
    {
        if (!button.interactable)
        {
            buttonTimer += Time.deltaTime;
            if (buttonTimer > buttonCooldown)
            {
                button.interactable = true;
                buttonTimer = 0;
            }
        }
    }

    public void OnSwitch()
    {
        onProductionLayout = !onProductionLayout;
        ScenarioService.instance.inProductionPhase = onProductionLayout;
        button.interactable = false;
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
