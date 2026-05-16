using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static event Action OnPlayerFirstMoved;

    [SerializeField] private PlayerView playerView;
    [Tooltip("dokunulan magnetin hedef dokunuşa ulaşma hızı")]
    [SerializeField] float movementSpeed = 200;
    [Tooltip("dead zone yüzdeliği")]
    [SerializeField] [Range(0,100)] float touchDeadPercentage = 10;
    Vector3 screenToFloorPosition;
    private Vector2 touchStartPos;
    private bool listenMovement=true;
    private bool firstMoveFired;
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
            touchStartPos = mainCamera.ScreenToViewportPoint(Input.mousePosition);
        }

        float kh = Input.GetAxisRaw("Horizontal");
        float kv = Input.GetAxisRaw("Vertical");
        bool keyboardActive = Mathf.Abs(kh) > 0.01f || Mathf.Abs(kv) > 0.01f;
        if (keyboardActive)
        {
            Vector2 dir = new Vector2(kh, kv).normalized;
            movementLogic.IsMovable = true;
            movementLogic.MoveDirection = new Vector3(-dir.y, 0f, dir.x);
            NotifyFirstMove();
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector2 moveVector = 100 * ((Vector2)mainCamera.ScreenToViewportPoint(Input.mousePosition) - touchStartPos);
            if (moveVector.magnitude < touchDeadPercentage)
            {
                movementLogic.IsMovable = false;
                return;
            }
            movementLogic.IsMovable = true;
            movementLogic.MoveDirection = new Vector3(-moveVector.normalized.y, 0f, moveVector.normalized.x);
            NotifyFirstMove();
            return;
        }

        movementLogic.IsMovable = false;
    }

    private void NotifyFirstMove()
    {
        if (firstMoveFired) return;
        firstMoveFired = true;
        OnPlayerFirstMoved?.Invoke();
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