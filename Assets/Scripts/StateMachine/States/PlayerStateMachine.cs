using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputHandler))]

public class PlayerStateMachine : StateMachine
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 8f;
    public float gravity = -25f;
    public float airControl = 3f;

    [Header("References")]
    public CharacterController controller;
    public InputHandler inputHandler;
    public Camera playerCamera;

    public Vector3 movementInput;
    public bool isRunning;
    public bool jumpTriggered;
    public float verticalVelocity;

    
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputHandler = GetComponent<InputHandler>();
        
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        ChangeState(new IdleState(this));
    }

    // private void Update()
    // {
    //     jumpTriggered = false;
    // }
}
