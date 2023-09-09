using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeManager :MonoBehaviour
{
    [SerializeField] private int levelTime = 120;
    private float leftTime;
    public static Action<int> OnGameCountDownChanged;
    private bool isCountdownOn;
    private int lastSentCountdown;
    private void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += (x) => StartCountdown();
        MagnetGameActionSystem.OnLevelCompleted +=  StopCountdown;
        MagnetGameActionSystem.OnLevelFailed +=  StopCountdown;
    }

    private void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= (x) => StartCountdown();
        MagnetGameActionSystem.OnLevelCompleted -=  StopCountdown;
        MagnetGameActionSystem.OnLevelFailed -=  StopCountdown;
    }

    private void Update()
    {
        if (!isCountdownOn)
        {
            return;
        }

        leftTime -= Time.deltaTime;
        if (lastSentCountdown > leftTime)
        {
            SendGameCountdownChanged();            
        }

        if (leftTime <= 0)
        {
            isCountdownOn = false;
            MagnetGameActionSystem.OnLevelFailed?.Invoke();
        }
    }

    private void StartCountdown()
    {
        leftTime = levelTime;
        SendGameCountdownChanged();
        isCountdownOn = true;
    }

    void SendGameCountdownChanged()
    {
        lastSentCountdown = (int)leftTime;
        OnGameCountDownChanged?.Invoke(lastSentCountdown);
    }

    private void StopCountdown()
    {
        isCountdownOn = false;
    }
}