using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerInputActions.IGameplayActions
{
    public Vector2 MoveInput { get; private set; }
    public bool IsRunning { get; private set; }

    public event Action RunningEvent;
    // public event Action<Vector2> MoveEvent;
    // public event Action<bool> RunningEvent;
    public event Action JumpEvent;
    public event Action AttackEvent;

    private PlayerInputActions _controls;

    private void Start()
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
        Debug.Log($"OnRunning Triggered: {IsRunning}");
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpEvent?.Invoke();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackEvent?.Invoke();
    }
}