using System;
using UnityEngine;

public class KeyDownInput : IInput
{
    public Action OnInputCalled { get; set; }
    private readonly KeyCode key;

    public KeyDownInput(KeyCode key)
    {
        this.key = key;
    }

    public void Update()
    {
        if (Input.GetKeyDown(key))
            OnInputCalled?.Invoke();
    }
}
