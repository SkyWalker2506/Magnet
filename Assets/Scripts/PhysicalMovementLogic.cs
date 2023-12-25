using UnityEngine;

public class PhysicalMovementLogic :IMovementLogic
{
    public bool IsMovable { get; set; }
    public Vector3 MoveDirection { get; set; }
    public float MovementSpeed { get; set; }
    Rigidbody rigidbody;

    public PhysicalMovementLogic(float movementSpeed, Rigidbody rigidbody)
    {
        MovementSpeed = movementSpeed;
        this.rigidbody = rigidbody;
    }
    
    public void Move()
    {
        if (!IsMovable)
        {
            rigidbody.velocity = Vector3.zero;
        }
        else
        {
            rigidbody.velocity = MoveDirection * (Time.fixedDeltaTime * MovementSpeed);
        }
    }


}