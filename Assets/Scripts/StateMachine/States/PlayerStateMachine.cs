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
    public bool RunScene => runScene;

    protected virtual void Awake()
    {
        // Dapatkan komponen yang dijamin ada oleh RequireComponent
        Controller = GetComponent<CharacterController>();
        InputHandler = GetComponent<InputHandler>();
        PlayerAnimator = GetComponent<Animator>();

        if (playerHealthManager == null)
        {
            playerHealthManager = GetComponent<PlayerHealthManager>();
            if (playerHealthManager == null)
            {
                Debug.LogError("PlayerHealthManager tidak ditemukan pada player!");
            }
        }
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
    }

    protected override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorLocked = !isCursorLocked;
            SetCursorLock(isCursorLocked);
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
}