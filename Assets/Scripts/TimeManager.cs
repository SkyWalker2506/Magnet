using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TimeManager :MonoBehaviour
{
    [SerializeField] private int levelTime = 90;
    private int leftTime;
    public static Action<int> OnGameCountDownChanged;
    private UniTaskVoid countDown; 

    private void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += (x) => StartCountdown();
    }

    private void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= (x) => StartCountdown();
    }


    private void StartCountdown()
    {
        leftTime = levelTime;
        OnGameCountDownChanged?.Invoke(leftTime);
        UniTaskVoid task = Countdown();
    }

    private async UniTaskVoid Countdown()
    {
        while (leftTime > 0)
        {
            await UniTask.Delay(1000);
            leftTime--;
            OnGameCountDownChanged?.Invoke(leftTime);
        }
        MagnetGameActionSystem.OnLevelFailed?.Invoke();
    }
}