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
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float slowRunSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float gravity = -25f;
    [SerializeField] private float airControl = 3f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private bool walkScene = false;
    [SerializeField] private bool runScene = true;

    [Header("Referensi")]
    [SerializeField] private Camera playerCamera;

    [Header("Referensi Health")]
    [SerializeField] private PlayerHealthManager playerHealthManager;
    public PlayerHealthManager PlayerHealthManager => playerHealthManager;
    
    [Header("Weapon Settings")]
    [SerializeField] private int weaponDamage = 20;
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float raycastMaxDistance = 100f;
    [SerializeField] private LayerMask raycastLayerMask = -1; // Default to all layers
    
    // Properties
    public CharacterController Controller { get; private set; }
    public InputHandler InputHandler { get; private set; }
    public Camera PlayerCamera => playerCamera;
    public Animator PlayerAnimator { get; private set; }
    
    // Variabel state
    public float CurrentSpeed { get; set; }
    public Vector3 MovementInput { get; set; }
    public float VerticalVelocity { get; set; }
    public bool IsRunning { get; set; }
    public bool JumpTriggered { get; set; }
    public bool IsAiming { get; set; }
    public bool IsHealing { get; set; }
    private bool isCursorLocked = true;
      // Gravity Control
    public bool IsGravityEnabled { get; private set; } = true;
    private float originalGravity;
    
    // Climbing reference
    private PlayerClimbFixed playerClimb;
    public bool IsClimbing => playerClimb != null && playerClimb.IsClimbing();
    
    // Weapon State
    private float lastFireTime = 0f;
    public bool CanFire => Time.time >= lastFireTime + fireRate;
    
    // Momentum saat lompat
    public Vector3 JumpMomentum { get; set; }
    public float JumpMomentumMultiplier { get; set; } = 1.2f;
    
    // Nilai konfigurasi yang terekspos
    public float WalkSpeed => walkSpeed;
    public float SlowRunSpeed => slowRunSpeed;
    public float RunSpeed => runSpeed;
    public float JumpForce => jumpForce;
    public float Gravity => gravity;
    public float AirControl => airControl;
    public float Acceleration => acceleration;
    public bool WalkScene => walkScene;
    public bool RunScene => runScene;    protected virtual void Awake()
    {
        // Dapatkan komponen yang dijamin ada oleh RequireComponent
        Controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<InputHandler>();
        PlayerAnimator = GetComponent<Animator>();
        playerClimb = GetComponent<PlayerClimbFixed>();

        if (playerHealthManager == null)
        {
            playerHealthManager = GetComponent<PlayerHealthManager>();
            if (playerHealthManager == null)
            {
                Debug.LogError("PlayerHealthManager tidak ditemukan pada player!");
            }
        }
        
        // Initialize raycast layer mask if not set
        if (raycastLayerMask == -1)
        {
            raycastLayerMask = Physics.DefaultRaycastLayers;
        }
        
        // Store original gravity value
        originalGravity = gravity;
    }
    
    private void Start()
    {
        // Gunakan kamera utama jika tidak ada yang disediakan
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError("Tidak ada kamera yang ditemukan. Silakan tetapkan referensi kamera.");
            }
        }
        
        SetCursorLock(true);
        // Mulai dengan state idle
        ChangeState(new IdleState(this));
    }    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            SetCursorLock(isCursorLocked);
        }
        
        // Kontrol gravitasi untuk testing (G key)
        if (Input.GetKeyDown(KeyCode.G))
        {
            ToggleGravity();
            Debug.Log($"Gravity {(IsGravityEnabled ? "Enabled" : "Disabled")}");
        }
        
        base.Update();
    }

    private void SetCursorLock(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }

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
    
    /// <summary>
    /// Menembakkan raycast dari tengah kamera/layar.
    /// </summary>
    /// <returns>True jika berhasil menembak, false jika belum bisa menembak (cooldown)</returns>
    public bool FireWeapon()
    {
        if (!CanFire) return false;
        
        ShootRaycastFromCamera();
        lastFireTime = Time.time;
        return true;
    }
    
    /// <summary>
    /// Menembakkan raycast dari tengah kamera/layar.
    /// </summary>
    private void ShootRaycastFromCamera()
    {
        if (playerCamera == null) return;
        
        // Mendapatkan posisi tengah layar (0.5, 0.5) dalam normalized viewport coordinates
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
        // Visualisasi raycast dengan debug line
        Debug.DrawRay(ray.origin, ray.direction * raycastMaxDistance, Color.red, 0.5f);
        
        // Efek suara tembakan bisa ditambahkan di sini
        // AudioManager.PlaySound("GunShot");
        
        // Animasi tembakan bisa ditambahkan di sini
        // PlayerAnimator.SetTrigger("Fire");
        
        // Melakukan raycast dan mencatat hit result
        RaycastHit hitInfo;
        bool hitSomething = Physics.Raycast(ray, out hitInfo, raycastMaxDistance, raycastLayerMask);
        
        if (hitSomething)
        {
            // Debug informasi hit
            Debug.Log($"Raycast hit: {hitInfo.collider.gameObject.name} at distance {hitInfo.distance}");
            
            // Opsional: Visualisasi titik hit
            Debug.DrawLine(ray.origin, hitInfo.point, Color.green, 0.5f);
            
            // Periksa apakah objek yang terkena memiliki HealthSystem
            HealthSystem targetHealth = hitInfo.collider.GetComponent<HealthSystem>();
            if (targetHealth != null)
            {
                // Memberikan damage langsung ke HealthSystem
                int damageDealt = targetHealth.TakeDamage(weaponDamage, this.gameObject);
                Debug.Log($"Hit dealt {damageDealt} damage to {hitInfo.collider.gameObject.name}");
                
                // Efek hit bisa ditambahkan di sini (darah, partikel, dll)
                // SpawnHitEffect(hitInfo.point, hitInfo.normal);
            }
            else
            {
                // Coba cari HealthSystem di parent atau children
                targetHealth = hitInfo.collider.GetComponentInParent<HealthSystem>();
                if (targetHealth != null)
                {
                    int damageDealt = targetHealth.TakeDamage(weaponDamage, this.gameObject);
                    Debug.Log($"Hit dealt {damageDealt} damage to parent of {hitInfo.collider.gameObject.name}");
                }
                else
                {
                    // Jika tidak ada HealthSystem, bisa dicoba gunakan Damager kalau ada (misal untuk hit box khusus)
                    Damager damager = hitInfo.collider.GetComponent<Damager>();
                    if (damager != null)
                    {
                        Debug.Log($"Hit a Damager on {hitInfo.collider.gameObject.name}");
                        // Kamu bisa mengimplementasikan logika khusus jika menembak Damager
                    }
                    else
                    {
                        Debug.Log($"Hit object has no health system: {hitInfo.collider.gameObject.name}");
                    }
                }
            }
            
            // Efek impact di surface (decal, partikel berdasarkan material)
            // InstantiateImpactEffect(hitInfo);
        }
    }
}