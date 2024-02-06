using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeManager :MonoBehaviour
{
    public static TimeManager Instance; 

    [SerializeField] private int levelTime = 120;
    public float LeftTime;
    public static Action<int> OnGameCountDownChanged;
    private bool isCountdownOn;
    private int lastSentCountdown;

    private void Awake()
    {
        Instance = this;
    }
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

        LeftTime -= Time.deltaTime;
        if (lastSentCountdown > LeftTime)
        {
            SendGameCountdownChanged();            
        }

        if (LeftTime <= 0)
        {
            isCountdownOn = false;
            MagnetGameActionSystem.OnLevelFailed?.Invoke();
        }
    }

    private void StartCountdown()
    {
        LeftTime = levelTime;
        SendGameCountdownChanged();
        isCountdownOn = true;
    }

    void SendGameCountdownChanged()
    {
        lastSentCountdown = (int)LeftTime;
        OnGameCountDownChanged?.Invoke(lastSentCountdown);
    }

    private void StopCountdown()
    {
        isCountdownOn = false;
    }
}