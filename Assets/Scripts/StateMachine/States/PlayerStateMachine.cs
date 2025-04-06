using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]

public class PlayerStateMachine : StateMachine
{
    [Header("Movement Settings")]
    public float currentSpeed;
    public float acceleration = 5f;
    public float walkSpeed;
    public float slowRunSpeed;
    public float runSpeed;
    public float jumpForce = 8f;
    public float gravity = -25f;
    public float airControl = 3f;
    public bool walkScene = false;
    public bool runScene = true;
    public bool isRunning;

    [Header("References")]
    public CharacterController controller;
    public InputHandler inputHandler;
    public Camera playerCamera;

    public Vector3 movementInput;
    public bool jumpTriggered;
    public float verticalVelocity;

    public Animator Animator;

    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();
        Animator = GetComponent<Animator>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        ChangeState(new IdleState(this));
    }

    private void OnDestroy()
    {
        
    }

    // private void Update()
    // {
    //     jumpTriggered = false;
    // }
}
