using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerInputActions.IGameplayActions
{
    public Vector2 MovementValue { get; private set; }
    public bool IsRunning { get; private set; }

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
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        AttackEvent?.Invoke();
    }

    public void OnRunning(InputAction.CallbackContext context)
    {
        IsRunning = context.ReadValueAsButton();
    }
}