using System;
using UnityEngine;

public class MultiClick : IInput
{
    public Action OnInputCalled { get; set; }
    private float lastClicked;
    private readonly float interval;
    private readonly int neededCount;
    private int clickCount;

    public MultiClick(int clickForAction = 2, float clickInterval=1)
    {
        neededCount = clickForAction;
        interval = clickInterval;
    }
    
    
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Click();
        }
        
    }

    private void Click()
    {
        if (lastClicked + interval > Time.time)
        {
            clickCount++;
        }
        else
        {
            clickCount = 1;
        }
        lastClicked = Time.time;

        if (neededCount == clickCount)
        {
            OnInputCalled?.Invoke();
            clickCount = 0;
        }
    }
}