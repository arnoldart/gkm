using UnityEngine;
using UnityEngine.InputSystem;

// [RequireComponent(typeof(PlayerStateMachine))]

public enum InputType
{
    Attack,
    Fire,
    Aim,
    Interact
}
public class InputHandler : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private PlayerStateMachine playerStateMachine;
    public InputType InputType { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool FirePressed { get; private set; }
    public bool AimPressed { get; private set; }
    public bool InteractPressed { get; private set; }

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    private void OnEnable()
    {
        // Aktifkan tindakan input
        playerInputActions.Gameplay.Enable();
        playerInputActions.WeaponShortcut.Enable();

        // Gameplay actions
        playerInputActions.Gameplay.Move.performed += OnMove;
        playerInputActions.Gameplay.Move.canceled += OnMove;
        playerInputActions.Gameplay.Jump.performed += OnJump;
        playerInputActions.Gameplay.Running.performed += OnRun;
        playerInputActions.Gameplay.Running.canceled += OnRun;
        playerInputActions.Gameplay.Heal.performed += OnHeal;
        playerInputActions.Gameplay.Heal.canceled += OnHeal;
        playerInputActions.Gameplay.Attack.performed += OnAttack;
        playerInputActions.Gameplay.Interact.performed += OnInteract;

        // Weapon actions
        playerInputActions.WeaponShortcut.AIM.performed += OnAim;
        playerInputActions.WeaponShortcut.AIM.canceled += OnAim;
        playerInputActions.WeaponShortcut.Fire.performed += OnFire;
        playerInputActions.WeaponShortcut.Fire.canceled += OnFire;
    }

    private void OnDisable()
    {
        playerInputActions.Gameplay.Disable();
        playerInputActions.WeaponShortcut.Disable();

        // Gameplay actions
        playerInputActions.Gameplay.Move.performed -= OnMove;
        playerInputActions.Gameplay.Move.canceled -= OnMove;
        playerInputActions.Gameplay.Jump.performed -= OnJump;
        playerInputActions.Gameplay.Running.performed -= OnRun;
        playerInputActions.Gameplay.Running.canceled -= OnRun;
        playerInputActions.Gameplay.Heal.performed -= OnHeal;
        playerInputActions.Gameplay.Heal.canceled -= OnHeal;
        playerInputActions.Gameplay.Attack.performed -= OnAttack;
        playerInputActions.Gameplay.Interact.performed -= OnInteract;

        // Weapon actions
        playerInputActions.WeaponShortcut.AIM.performed -= OnAim;
        playerInputActions.WeaponShortcut.AIM.canceled -= OnAim;
        playerInputActions.WeaponShortcut.Fire.performed -= OnFire;
        playerInputActions.WeaponShortcut.Fire.canceled -= OnFire;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        playerStateMachine.SetMovementInput(new Vector3(input.x, 0, input.y));
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        playerStateMachine.JumpTriggered = true;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        playerStateMachine.IsRunning = !playerStateMachine.WalkScene && context.performed;
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        AimPressed = context.ReadValueAsButton();
        playerStateMachine.IsAiming = AimPressed;
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        FirePressed = context.performed;
    }

    private void OnHeal(InputAction.CallbackContext context)
    {
        playerStateMachine.IsHealing = context.performed;
        if (context.performed && playerStateMachine.PlayerHealthManager != null)
        {
            playerStateMachine.PlayerHealthManager.HealPlayer();
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        AttackPressed = context.performed;
    }    private void OnInteract(InputAction.CallbackContext context)
    {
        InteractPressed = context.performed;
        if (context.performed)
        {
            Debug.Log("Interact button pressed");
        }
    }    public bool IsPressed(InputType inputType)
    {
        return inputType switch
        {
            InputType.Attack => AttackPressed,
            InputType.Fire => FirePressed,
            InputType.Aim => AimPressed,
            InputType.Interact => InteractPressed,
            _ => false
        };
    }    public void ResetPressed(InputType inputType)
    {
        switch (inputType)
        {
            case InputType.Attack:
                AttackPressed = false;
                break;
            case InputType.Fire:
                FirePressed = false;
                break;
            case InputType.Aim:
                AimPressed = false;
                break;
            case InputType.Interact:
                InteractPressed = false;
                break;
        }
    }    // Backward compatibility methods (optional - bisa dihapus jika tidak perlu)
    public bool IsAttackPressed() => IsPressed(InputType.Attack);
    public bool IsFirePressed() => IsPressed(InputType.Fire);
    public bool IsInteractPressed() => IsPressed(InputType.Interact);
    public void ResetAttackPressed() => ResetPressed(InputType.Attack);
    public void ResetFirePressed() => ResetPressed(InputType.Fire);
    public void ResetInteractPressed() => ResetPressed(InputType.Interact);
}