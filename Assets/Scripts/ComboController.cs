using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboController : MonoBehaviour
{
    [SerializeField]
    Text comboText;
    int combo;
    float lastScoredTime;
    [SerializeField]
    [Range(0.1f, 1)]
    float scoreInterval;

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

    void ShowCombo()
    {
        comboText.gameObject.SetActive(true);
        comboText.color= new Color().RandomColor();
        comboText.text = "x" + combo.ToString();
        comboText.transform.DOShakeRotation(1,45,5,45).OnComplete(()=>comboText.gameObject.SetActive(false));
    }

}
