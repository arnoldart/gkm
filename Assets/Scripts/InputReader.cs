using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerInputActions.IGameplayActions
{
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }
    public bool JumpTriggered { get; private set; }
    public bool AttackTriggered { get; private set; }

    private PlayerInputActions _controls;

    private void Awake()
    {
        _controls = new PlayerInputActions();
        _controls.Gameplay.SetCallbacks(this);
        _controls.Gameplay.Enable();
    }

    private void OnDestroy()
    {
        _controls.Gameplay.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    public void OnRunning(InputAction.CallbackContext context)
    {
        IsRunning = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpTriggered = true;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        // Implement look logic if needed
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackTriggered = true;
    }

    public void ResetFlags()
    {
        JumpTriggered = false;
        AttackTriggered = false;
    }
}