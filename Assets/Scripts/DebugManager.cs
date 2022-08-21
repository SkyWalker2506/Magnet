using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugManager : Singleton<DebugManager>
{
    [SerializeField]
    Text FPS;
    public Toggle IsMagnetismOn;


    void Update()
    {
        FPS.text = (1f / Time.deltaTime).ToString("F0");
    }
}
