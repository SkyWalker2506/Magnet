using System;
using UnityEngine;

public class QuitController : MonoBehaviour
{
    [SerializeField] private float topInterval = 1;
    [SerializeField] private int topCountToQuit = 2;
    private int currentTopCount;
    private DateTime lastTapTime;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (DateTime.Now.Subtract(lastTapTime).Seconds > topInterval)
            {
                currentTopCount = 0;
            }
            currentTopCount++;
            lastTapTime = DateTime.Now;
            if (currentTopCount >= topCountToQuit)
            {
                Application.Quit();
            }
        }
    }
}