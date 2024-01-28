using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private PlayerView playerView;
    [Tooltip("dokunulan magnetin hedef dokunuşa ulaşma hızı")]
    [SerializeField] float movementSpeed = 200;
    [Tooltip("dead zone yüzdeliği")]
    [SerializeField] [Range(0,100)] float touchDeadPercentage = 10;
    Vector3 screenToFloorPosition;
    private Vector2 touchStartPos;
    private bool listenMovement=true;
    private Magnet magnet;
    Camera mainCamera;
    private IMovementLogic movementLogic;
    private IInput magnetismToggleInput;
        
    void Awake()
    {
        magnet = GetComponent<Magnet>();
        movementLogic = new PhysicalMovementLogic(movementSpeed, GetComponent<Rigidbody>());
        listenMovement = true;
        magnetismToggleInput = new MultiClick(2,.5f);
    }

    private void Start()
    {
        SetMagnetism(true);
    }

    void OnEnable()
    {
        MagnetGameActionSystem.OnLevelFailed += DisablePlayerController;
        magnetismToggleInput.OnInputCalled += ToggleMagnet;
    }

    void OnDisable()
    {
        MagnetGameActionSystem.OnLevelFailed -= DisablePlayerController;
        magnetismToggleInput.OnInputCalled -= ToggleMagnet;
    }
    
    void DisablePlayerController()
    {
        listenMovement = false;
        movementLogic.IsMovable = false;
    }

    void Update()
    {
        magnetismToggleInput.Update();
        if (!listenMovement) return;
        if (!mainCamera)
        {
            mainCamera = Camera.main;
            if (!mainCamera) return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            movementLogic.IsMovable = true;
            touchStartPos =  mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }
        
        if (Input.GetMouseButton(0))
        {
            movementLogic.IsMovable = true;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            movementLogic.IsMovable = false;
        }

        if (!movementLogic.IsMovable)
        {
            return;
        }
        
        Vector2 moveVector = 100*((Vector2)mainCamera.ScreenToViewportPoint(Input.mousePosition) - touchStartPos);
        Debug.Log(Input.mousePosition);
        Debug.Log(touchStartPos);
        movementLogic.IsMovable = !(moveVector.magnitude < touchDeadPercentage);

        if (movementLogic.IsMovable)
        {
            movementLogic.MoveDirection = new Vector3(-moveVector.normalized.y,0,moveVector.normalized.x);
        }
    }

    private void FixedUpdate()
    {
        if (!listenMovement) return;
            movementLogic.Move();
    }


    private void ToggleMagnet()
    {
        SetMagnetism(!magnet.enabled);
    }

    private void SetMagnetism(bool value)
    {
        magnet.enabled = value;
        playerView.TogglePlayerVisual(value);
    }
}