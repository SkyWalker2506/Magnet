using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerController : Singleton<PlayerController>
{
    Camera camera;

    public LayerMask playerLayerMask;//magnetin layer ı dragtargets
    public LayerMask floorLayerMask;//magnetin layer ı dragtargets
    [SerializeField] float movementSpeed = 200;//dokunulan magnetin hedef dokunuşa ulaşma hızı
    [SerializeField] [Range(0,100)] float touchDeadPercentage = 1;//dokunulan magnetin hedef dokunuşa ulaşma hızı
    bool isPlayerMovable;//magnet hareket edebilir mi
    Transform player;//raycast ışınının vurduğu noktanın transformu
    Rigidbody playerRB;//hareket ettirilmek istenen magnetin rigidbody si
    Vector3 screenToFloorPosition;
    bool isPlayerMoveable;
    Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
    private Vector2 touchStartPos;
    
    
    void OnEnable()
    {
        MagnetGameActionSystem.LevelStarted += SetLevel;
        MagnetGameActionSystem.OnLevelFailed += DisablePlayer;
    }

    void OnDisable()
    {
        MagnetGameActionSystem.LevelStarted -= SetLevel;
        MagnetGameActionSystem.OnLevelFailed -= DisablePlayer;
    }

    void DisablePlayer()
    {
        isPlayerMoveable = false;
    }

    void SetLevel(int level)
    {
        Invoke(nameof(SetPlayer), .5f);
    }

    void SetPlayer()
    {
        player = Player.CurrentPlayer.transform;
        playerRB = player.GetComponent<Rigidbody>();
        isPlayerMoveable = true;
    }
    void FixedUpdate()
    {
        if (!player) return;

        if (!camera)
        {
            camera = Camera.main;
            if (!camera) return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            isPlayerMovable = true;
            touchStartPos =  camera.ScreenToViewportPoint(Input.mousePosition);
            Debug.Log(touchStartPos);
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isPlayerMovable = false;
        }

        if (!isPlayerMovable)
        {
            playerRB.velocity = Vector3.zero;
            return;
        }

        Vector2 moveVector = 100*((Vector2)camera.ScreenToViewportPoint(Input.mousePosition) - touchStartPos);
        if (moveVector.magnitude < touchDeadPercentage)
        {
            return;
        }


        Vector3 direction = new Vector3(-moveVector.normalized.y,0,moveVector.normalized.x);

        playerRB.velocity = direction * Time.fixedDeltaTime * movementSpeed;
    }

}