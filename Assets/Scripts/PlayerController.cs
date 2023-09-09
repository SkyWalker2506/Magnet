using System;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    Camera mainCamera;

    [SerializeField] float movementSpeed = 200;//dokunulan magnetin hedef dokunuşa ulaşma hızı
    [SerializeField] [Range(0,100)] float touchDeadPercentage = 1;//dokunulan magnetin hedef dokunuşa ulaşma hızı
    bool isPlayerMovable;//magnet hareket edebilir mi
    Transform player;//raycast ışınının vurduğu noktanın transformu
    Rigidbody playerRB;//hareket ettirilmek istenen magnetin rigidbody si
    Vector3 screenToFloorPosition;
    bool isPlayerMoveable;
    private Vector2 touchStartPos;
    private bool listenMovement;
    private Vector3 moveDirection; 
    void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += SetLevel;
        MagnetGameActionSystem.OnLevelFailed += DisablePlayerController;
    }

    void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= SetLevel;
        MagnetGameActionSystem.OnLevelFailed -= DisablePlayerController;
    }

    void DisablePlayerController()
    {
        listenMovement = false;
        isPlayerMovable = false;
    }

    void SetLevel(int level)
    {
        listenMovement = true;
        Invoke(nameof(SetPlayer), .5f);
    }

    void SetPlayer()
    {
        player = Player.CurrentPlayer.transform;
        playerRB = player.GetComponent<Rigidbody>();
        isPlayerMoveable = true;
    }
    
    void Update()
    {
        if (!player) return;
        if (!listenMovement) return;
        if (!mainCamera)
        {
            mainCamera = Camera.main;
            if (!mainCamera) return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            isPlayerMovable = true;
            touchStartPos =  mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isPlayerMovable = false;
        }
 
        Vector2 moveVector = 100*((Vector2)mainCamera.ScreenToViewportPoint(Input.mousePosition) - touchStartPos);
        if (moveVector.magnitude < touchDeadPercentage)
        {
            return;
        }
        
        moveDirection = new Vector3(-moveVector.normalized.y,0,moveVector.normalized.x);
    }


    private void FixedUpdate()
    {
        if (!player) return;
        if (!listenMovement) return;
        if (!isPlayerMovable)
        {
            playerRB.velocity = Vector3.zero;
            return;
        }
        playerRB.velocity = moveDirection * (Time.fixedDeltaTime * movementSpeed);


    }
}