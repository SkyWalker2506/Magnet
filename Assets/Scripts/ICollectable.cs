using System;

public interface ICollectable
{

    Action OnCollected { get; } 
    void Collect();
    
}