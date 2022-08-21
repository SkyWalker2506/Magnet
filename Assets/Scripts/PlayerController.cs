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
    [SerializeField]
    float movementSpeed = 200;//dokunulan magnetin hedef dokunuşa ulaşma hızı
    bool isPlayerMovable;//magnet hareket edebilir mi
    Transform player;//raycast ışınının vurduğu noktanın transformu
    Rigidbody playerRB;//hareket ettirilmek istenen magnetin rigidbody si
    Vector3 screenToFloorPosition;
    bool isPlayerMoveable;
    Plane floorPlane = new Plane(Vector3.up, Vector3.zero);
    void Start()
    {
    }
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
        Invoke("SetPlayer", 1);
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

        isPlayerMovable = Input.GetMouseButton(0);//hareket ettirilebilir değişkeni true olur
        if (!isPlayerMovable)
        {
            playerRB.velocity = Vector3.zero;
            return;
        }
        if (!camera)
        {
            // Kamera bulunnana kadar alttaki kodu atla
            camera = Camera.main;
            if (!camera) return;
        }
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        // dogru ile yuzey kesismesinden dokunulan noktayi bul
        if (floorPlane.Raycast(ray, out float enter))
        {
            screenToFloorPosition = ray.GetPoint(enter);
        }

        var direction = (screenToFloorPosition - playerRB.position);
        direction.y = 0;
        direction = direction.normalized;

        playerRB.velocity = direction * Time.fixedDeltaTime * movementSpeed;
    }

}