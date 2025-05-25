using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Menangani input pemain dan berkomunikasi dengan state machine pemain.
/// </summary>
// [RequireComponent(typeof(PlayerStateMachine))]
public class InputHandler : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private PlayerStateMachine playerStateMachine;    public bool AttackPressed { get; private set; }
    public bool FirePressed { get; private set; }
    public bool AimPressed { get; private set; }
    public bool ClimbPressed { get; private set; }

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

        // Daftarkan callback event
        playerInputActions.Gameplay.Move.performed += OnMove;
        playerInputActions.Gameplay.Move.canceled += OnMove;
        playerInputActions.Gameplay.Jump.performed += OnJump;
        playerInputActions.Gameplay.Running.performed += OnRun;
        playerInputActions.Gameplay.Running.canceled += OnRun;
        playerInputActions.WeaponShortcut.AIM.performed += OnAim;
        playerInputActions.WeaponShortcut.AIM.canceled += OnAim;
        playerInputActions.Gameplay.Heal.performed += OnHeal;
        playerInputActions.Gameplay.Heal.canceled += OnHeal;        playerInputActions.Gameplay.Attack.performed += OnAttack;
        playerInputActions.WeaponShortcut.Fire.performed += OnFire;
        playerInputActions.WeaponShortcut.Fire.canceled += OnFire;
        
        // Add climb input binding (assuming E key is mapped in input actions)
        // If not available in input actions, we'll handle it manually
    }

    private void OnDisable()
    {
        // Batalkan pendaftaran callback event
        playerInputActions.Gameplay.Move.performed -= OnMove;
        playerInputActions.Gameplay.Move.canceled -= OnMove;
        playerInputActions.Gameplay.Jump.performed -= OnJump;
        playerInputActions.Gameplay.Running.performed -= OnRun;
        playerInputActions.Gameplay.Running.canceled -= OnRun;
        playerInputActions.WeaponShortcut.AIM.performed -= OnAim;
        playerInputActions.WeaponShortcut.AIM.canceled -= OnAim;
        playerInputActions.Gameplay.Heal.performed -= OnHeal;
        playerInputActions.Gameplay.Heal.canceled -= OnHeal;
        playerInputActions.Gameplay.Attack.performed -= OnAttack;
        playerInputActions.WeaponShortcut.Fire.performed -= OnFire;
        playerInputActions.WeaponShortcut.Fire.canceled -= OnFire;
        
        // Nonaktifkan tindakan input
        playerInputActions.Gameplay.Disable();
        playerInputActions.WeaponShortcut.Disable();
    }

    /// <summary>
    /// Menangani perubahan input gerakan.
    /// </summary>
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        playerStateMachine.SetMovementInput(new Vector3(input.x, 0, input.y));
    }

    /// <summary>
    /// Menangani input lompat.
    /// </summary>
    private void OnJump(InputAction.CallbackContext context)
    {
        playerStateMachine.JumpTriggered = true;
    }

    /// <summary>
    /// Menangani input berlari.
    /// </summary>
    private void OnRun(InputAction.CallbackContext context)
    {
        playerStateMachine.IsRunning = !playerStateMachine.WalkScene && context.performed;
    }

    /// <summary>
    /// Menangani input membidik.
    /// </summary>
    private void OnAim(InputAction.CallbackContext context)
    {
        AimPressed = context.ReadValueAsButton();
        playerStateMachine.IsAiming = AimPressed;
        if (AimPressed)
        {
            Debug.Log("Aim button pressed");
        }
        else
        {
            Debug.Log("Aim button released");
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        FirePressed = context.performed;

        // Debug output when fire is pressed
        if (context.performed)
        {
            Debug.Log("Fire button pressed");
        }
    }

    private void OnHeal(InputAction.CallbackContext context)
    {
        playerStateMachine.IsHealing = context.performed;
        if (context.performed && playerStateMachine.PlayerHealthManager != null)
        {
            playerStateMachine.PlayerHealthManager.HealPlayer();
        }
    }    private void OnAttack(InputAction.CallbackContext context)
    {
        AttackPressed = context.performed;
    }
    
    // Handle climb input manually for now (E key)
    private void Update()
    {
        // Handle climb input with E key
        if (Input.GetKeyDown(KeyCode.E))
        {
            ClimbPressed = true;
        }
    }    public bool IsAttackPressed()
    {
        return AttackPressed;
    }
    
    public bool IsFirePressed()
    {
        return FirePressed;
    }
    
    public bool IsClimbPressed()
    {
        return ClimbPressed;
    }    // Reset AttackPressed setiap frame (bisa dipanggil dari PlayerStateMachine setelah update logic)
    public void ResetAttackPressed()
    {
        AttackPressed = false;
    }
    
    // Reset FirePressed setiap frame
    public void ResetFirePressed()
    {
        FirePressed = false;
    }
    
    // Reset ClimbPressed setiap frame
    public void ResetClimbPressed()
    {
        ClimbPressed = false;
    }
}