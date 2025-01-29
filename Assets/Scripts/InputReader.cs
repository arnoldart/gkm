using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, PlayerInputActions.IGameplayActions
{
    public event Action<Vector2> MoveEvent;
    public event Action<bool> RunningEvent;
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
        if (context.performed || context.canceled)
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnRunning(InputAction.CallbackContext context)
    {
        RunningEvent?.Invoke(context.ReadValueAsButton());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            JumpEvent?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            AttackEvent?.Invoke();
    }
}