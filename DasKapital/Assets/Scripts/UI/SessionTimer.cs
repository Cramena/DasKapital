using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SessionTimer : MonoBehaviour
{
    public Text timerText;
    public float timerFactor = 1;
    private float timer;
    private bool running;

    private void Awake()
    {
        HideTimer();
        // running = true;
    }

    private void Update()
    {
        if (running)
        {
            timer += Time.deltaTime * timerFactor;
            // DisplayTimer();
        }
    }

    public void StartTimer()
    {
        running = true;
    }

    public void StopTimer()
    {
        running = false;
    }

    public void DisplayTimer()
    {
        timerText.enabled = true;
        string minutes = timer/60 < 1 ? "0" : (timer/60).ToString("#");
        string seconds = timer%60 < 1 || timer%60 == 60 ? "0" : (timer%60).ToString("#");
        timerText.text = $"Temps de session : {minutes}mn{seconds}s";
    }

    public void HideTimer()
    {
        timerText.enabled = false;
    }
}
