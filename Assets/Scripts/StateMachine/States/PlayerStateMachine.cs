using System;
using UnityEngine;

/// <summary>
/// State machine untuk karakter pemain.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]
public class PlayerStateMachine : StateMachine
{
    [Header("Pengaturan Gerakan")]
    [SerializeField]
    private float walkSpeed = 2f;

    [SerializeField]
    private float slowRunSpeed = 4f;

    [SerializeField]
    private float runSpeed = 7f;

    [SerializeField]
    private float jumpForce = 12f;

    [SerializeField]
    private float gravity = -25f;

    [SerializeField]
    private float airControl = 3f;

    [SerializeField]
    private float acceleration = 5f;

    [SerializeField]
    private bool walkScene = false;

    [SerializeField]
    private bool runScene = true;

    [Header("Referensi")]
    [SerializeField]
    private Camera playerCamera;

    [Header("Referensi Health")]
    [SerializeField]
    private PlayerHealthManager playerHealthManager;
    public PlayerHealthManager PlayerHealthManager => playerHealthManager;

    [Header("Weapon Settings")]
    [SerializeField]
    private int weaponDamage = 20;

    [SerializeField]
    private float fireRate = 0.5f;

    [SerializeField]
    private float raycastMaxDistance = 100f;

    [SerializeField]
    private LayerMask raycastLayerMask = -1; // Default to all layers

    [Header("Climbing Settings")]
    [SerializeField]
    private float groundCheckBuffer = 0.2f;
    private float timeSinceGrounded = 0f;
    public float climbDistance = 1f;

    [SerializeField]
    public LayerMask climbableLayerMask;

    // Core Components
    public CharacterController Controller { get; private set; }
    public InputHandler InputHandler { get; private set; }
    public Camera PlayerCamera => playerCamera;
    public Animator PlayerAnimator { get; private set; }

    // State Variables
    public float CurrentSpeed { get; set; }
    public Vector3 MovementInput { get; set; }
    public float VerticalVelocity { get; set; }
    public bool IsRunning { get; set; }
    public bool JumpTriggered { get; set; }
    public bool IsAiming { get; set; }
    public bool IsHealing { get; set; }

    // Cursor Control
    private bool isCursorLocked = true;

    // Gravity Control
    public bool IsGravityEnabled = true;
    private float originalGravity;

    // Climbing reference
    private PlayerClimbFixed playerClimb;
    public bool IsClimbing => playerClimb != null && playerClimb.IsClimbing();

    // Weapon State
    private float lastFireTime = 0f;
    public bool CanFire => Time.time >= lastFireTime + fireRate;

    // Jump Momentum
    public Vector3 JumpMomentum { get; set; }
    public float JumpMomentumMultiplier { get; set; } = 1.2f;

    // Movement Configuration Properties
    public float WalkSpeed => walkSpeed;
    public float SlowRunSpeed => slowRunSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float Gravity => gravity;
    public float AirControl => airControl;
    public float Acceleration => acceleration;
    public bool WalkScene => walkScene;
    public bool RunScene => runScene;

    protected virtual void Awake()
    {
        InitializeComponents();
        InitializeHealth();
        InitializeWeapon();
        InitializeGravity();
    }

    private void InitializeComponents()
    {
        // Dapatkan komponen yang dijamin ada oleh RequireComponent
        Controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<InputHandler>();
        PlayerAnimator = GetComponent<Animator>();
        playerClimb = GetComponent<PlayerClimbFixed>();
    }

    private void InitializeHealth()
    {
        if (playerHealthManager == null)
        {
            playerHealthManager = GetComponent<PlayerHealthManager>();
            if (playerHealthManager == null)
            {
                Debug.LogError("PlayerHealthManager tidak ditemukan pada player!");
            }
        }
    }

    private void InitializeWeapon()
    {
        // Initialize raycast layer mask if not set
        if (raycastLayerMask == -1)
        {
            raycastLayerMask = Physics.DefaultRaycastLayers;
        }
    }

    private void InitializeGravity()
    {
        // Store original gravity value
        originalGravity = gravity;
    }

    private void Start()
    {
        InitializeCamera();
        InitializeCursor();
        StartStateMachine();
    }

    private void InitializeCamera()
    {
        // Gunakan kamera utama jika tidak ada yang disediakan
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError(
                    "Tidak ada kamera yang ditemukan. Silakan tetapkan referensi kamera."
                );
            }
        }
    }

    private void InitializeCursor()
    {
        SetCursorLock(true);
    }

    private void StartStateMachine()
    {
        // Mulai dengan state idle
        ChangeState(new IdleState(this));
    }

    protected override void Update()
    {
        HandleInput();
        base.Update();
    }

    private void HandleInput()
    {
        HandleCursorToggle();
        HandleGravityToggle();
    }

    private void HandleCursorToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            SetCursorLock(isCursorLocked);
        }
    }

    private void HandleGravityToggle()
    {
        // Kontrol gravitasi untuk testing (G key)
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleGravity();
            Debug.Log($"Gravity {(IsGravityEnabled ? "Enabled" : "Disabled")}");
        }
    }

    private void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

    #region State Management Methods

    /// <summary>
    /// Mengatur ulang pemicu lompat setelah dikonsumsi.
    /// </summary>
    public void ConsumeJumpTrigger()
    {
        JumpTriggered = false;
    }

    /// <summary>
    /// Memperbarui vektor input gerakan.
    /// </summary>
    /// <param name="input">Vektor input gerakan baru.</param>
    public void SetMovementInput(Vector3 input)
    {
        MovementInput = input;
    }

    #endregion

    #region Gravity Control Methods

    /// <summary>
    /// Mengaktifkan atau mematikan gravitasi pada player.
    /// </summary>
    /// <param name="enabled">True untuk mengaktifkan gravitasi, false untuk mematikan</param>
    public void SetGravityEnabled(bool enabled)
    {
        IsGravityEnabled = enabled;
        if (!enabled)
        {
            // Jika gravitasi dimatikan, hentikan velocity vertikal
            VerticalVelocity = 0f;
        }
    }

    /// <summary>
    /// Toggle gravitasi on/off.
    /// </summary>
    public void ToggleGravity()
    {
        SetGravityEnabled(!IsGravityEnabled);
    }

    /// <summary>
    /// Mendapatkan nilai gravitasi yang sedang aktif.
    /// </summary>
    /// <returns>Nilai gravitasi saat ini (0 jika dimatikan, nilai asli jika diaktifkan)</returns>
    public float GetCurrentGravity()
    {
        return IsGravityEnabled ? originalGravity : 0f;
    }

    /// <summary>
    /// Mengembalikan gravitasi ke nilai asli dan mengaktifkannya.
    /// </summary>
    public void ResetGravity()
    {
        gravity = originalGravity;
        SetGravityEnabled(true);
    }

    #endregion
    #region Weapon System Methods

    /// <summary>
    /// Menembakkan raycast dari tengah kamera/layar.
    /// </summary>
    /// <returns>True jika berhasil menembak, false jika belum bisa menembak (cooldown)</returns>
    public bool FireWeapon()
    {
        if (!CanFire)
            return false;

        ShootRaycastFromCamera();
        lastFireTime = Time.time;
        return true;
    }

    /// <summary>
    /// Menembakkan raycast dari tengah kamera/layar.
    /// </summary>
    private void ShootRaycastFromCamera()
    {
        if (playerCamera == null)
            return;

        Ray ray = CreateCameraRay();
        DrawRaycastVisualization(ray);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, raycastMaxDistance, raycastLayerMask))
        {
            ProcessWeaponHit(hitInfo, ray);
        }
    }

    /// <summary>
    /// Membuat ray dari tengah kamera.
    /// </summary>
    private Ray CreateCameraRay()
    {
        return playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    /// <summary>
    /// Menggambar visualisasi raycast untuk debugging.
    /// </summary>
    private void DrawRaycastVisualization(Ray ray)
    {
        Debug.DrawRay(ray.origin, ray.direction * raycastMaxDistance, Color.red, 0.5f);
    }

    /// <summary>
    /// Memproses hasil hit dari weapon raycast.
    /// </summary>
    private void ProcessWeaponHit(RaycastHit hitInfo, Ray ray)
    {
        Debug.Log(
            $"Raycast hit: {hitInfo.collider.gameObject.name} at distance {hitInfo.distance}"
        );
        Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 0.5f);

        HealthSystem targetHealth = FindTargetHealthSystem(hitInfo.collider);

        if (targetHealth != null)
        {
            DealDamageToTarget(targetHealth, hitInfo.collider.gameObject.name);
        }
        else
        {
            HandleNonHealthTarget(hitInfo.collider);
        }
    }

    /// <summary>
    /// Mencari HealthSystem pada target atau parent-nya.
    /// </summary>
    private HealthSystem FindTargetHealthSystem(Collider collider)
    {
        // Coba langsung pada collider
        HealthSystem health = collider.GetComponent<HealthSystem>();
        if (health != null)
            return health;

        // Coba pada parent
        return collider.GetComponentInParent<HealthSystem>();
    }

    /// <summary>
    /// Memberikan damage pada target yang memiliki HealthSystem.
    /// </summary>
    private void DealDamageToTarget(HealthSystem targetHealth, string targetName)
    {
        int damageDealt = targetHealth.TakeDamage(weaponDamage, this.gameObject);
        Debug.Log($"Hit dealt {damageDealt} damage to {targetName}");
    }

    /// <summary>
    /// Menangani target yang tidak memiliki HealthSystem.
    /// </summary>
    private void HandleNonHealthTarget(Collider collider)
    {
        Damager damager = collider.GetComponent<Damager>();
        if (damager != null)
        {
            Debug.Log($"Hit a Damager on {collider.gameObject.name}");
        }
        else
        {
            Debug.Log($"Hit object has no health system: {collider.gameObject.name}");
        }
    }

    #endregion
}
