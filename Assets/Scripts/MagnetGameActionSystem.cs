using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetGameActionSystem 
{
    public static  Action OnLevelCompleted;
    public static Action<int> LevelStarted;
    public static Action LevelUnloadedStarted;
    public static Action LevelUnloadedEnded;
    public static Action OnLevelFailed;
    public static Action<int> ObjectCollected;
    public static Action<Metal> OnMetalCollected;

}
