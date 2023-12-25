using System;

public interface IInput
{
    public Action OnInputCalled { get; set; }
    public void Update();
}