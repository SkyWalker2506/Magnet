using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ComboController : MonoBehaviour
{
    [SerializeField]
    private Text comboText;
    private int combo;
    private float lastScoredTime;
    [Range(0.1f, 5)]
    [SerializeField]
    private float scoreInterval;
    [Range(0.5f, 5)]
    [SerializeField]
    private float screenTime = 1.5f;
    private void Awake()
    {
        comboText.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        MagnetGameActionSystem.ObjectCollected += OnScored;
    }

    private void OnDisable()
    {
        MagnetGameActionSystem.ObjectCollected -= OnScored;
    }

    void OnScored(int a)
    {
        if (lastScoredTime + scoreInterval > Time.realtimeSinceStartup)
        {
            combo++;
            ShowCombo();
        }
        else
            combo = 1;
        lastScoredTime = Time.realtimeSinceStartup;
    }

    async void ShowCombo()
    {
        comboText.gameObject.SetActive(true);
        comboText.color= new Color().RandomColor();
        comboText.text = $"x{combo}";
        comboText.transform.DOShakeRotation(screenTime, 45, 5, 45);
        await UniTask.WaitUntil(()=>lastScoredTime + screenTime < Time.realtimeSinceStartup);
        comboText.gameObject.SetActive(false);
    }

}
