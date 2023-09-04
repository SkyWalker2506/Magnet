using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player CurrentPlayer;
    void Awake()
    {
        CurrentPlayer = this;
    }

}