using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]
[RequireComponent(typeof(Animator))]
public class PlayerStateMachine : StateMachine
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _slowRunSpeed = 4f;
    [SerializeField] private float _runSpeed = 8f;
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _gravity = -25f;
    [SerializeField] private float _airControl = 3f;
    [SerializeField] private float _maxSlopeAngle = 45f;
    
    [Header("Scene Settings")]
    [SerializeField] private bool _walkScene = false;
    [SerializeField] private bool _runScene = true;
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask _groundLayer;

    [Header("References")]
    public CharacterController controller;
    public InputHandler inputHandler;
    public Camera playerCamera;
    public Animator animator;

    // State
    public PlayerStateFactory StateFactory { get; private set; }
    public float currentSpeed;
    public Vector3 movementInput;
    public bool jumpTriggered;
    public float verticalVelocity;
    public bool isRunning;
    
    // Properties
    public float WalkSpeed => _walkSpeed;
    public float SlowRunSpeed => _slowRunSpeed;
    public float RunSpeed => _runSpeed;
    public float Acceleration => _acceleration;
    public float JumpForce => _jumpForce;
    public float Gravity => _gravity;
    public float AirControl => _airControl;
    public bool WalkScene => _walkScene;
    public bool RunScene => _runScene;
    public float MaxSlopeAngle => _maxSlopeAngle;
    public LayerMask groundLayer => _groundLayer;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();
        animator = GetComponent<Animator>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        StateFactory = new PlayerStateFactory(this);
    }
    
    private void Start()
    {
        ChangeState(StateFactory.Idle());
        
        // Subscribe to events from InputHandler
        if (inputHandler != null)
        {
            inputHandler.OnJumpPressed += HandleJumpInput;
            inputHandler.OnRunPressed += HandleRunInput;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (inputHandler != null)
        {
            inputHandler.OnJumpPressed -= HandleJumpInput;
            inputHandler.OnRunPressed -= HandleRunInput;
        }
    }
    
    protected override void Update()
    {
        // Process input
        if (inputHandler != null)
        {
            movementInput = inputHandler.MovementValue;
        }
        
        base.Update();
    }
    
    private void HandleJumpInput()
    {
        jumpTriggered = true;
    }
    
    private void HandleRunInput(bool isPressed)
    {
        isRunning = isPressed && !_walkScene;
    }
}
