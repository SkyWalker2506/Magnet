using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementLogic
{
    bool IsMovable { get; set; }
    Vector3 MoveDirection { get; set; }
    float MovementSpeed { get; set; }
    void Move();

}