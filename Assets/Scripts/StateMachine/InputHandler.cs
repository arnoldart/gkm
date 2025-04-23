using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Menangani input pemain dan berkomunikasi dengan state machine pemain.
/// </summary>
[RequireComponent(typeof(PlayerStateMachine))]
public class InputHandler : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerStateMachine = GetComponent<PlayerStateMachine>();
    }

    private void OnEnable()
    {
        // Aktifkan tindakan input
        playerInputActions.Gameplay.Enable();
        
        // Daftarkan callback event
        playerInputActions.Gameplay.Move.performed += OnMove;
        playerInputActions.Gameplay.Move.canceled += OnMove;
        playerInputActions.Gameplay.Jump.performed += OnJump;
        playerInputActions.Gameplay.Running.performed += OnRun;
        playerInputActions.Gameplay.Running.canceled += OnRun;
    }

    private void OnDisable()
    {
        // Batalkan pendaftaran callback event
        playerInputActions.Gameplay.Move.performed -= OnMove;
        playerInputActions.Gameplay.Move.canceled -= OnMove;
        playerInputActions.Gameplay.Jump.performed -= OnJump;
        playerInputActions.Gameplay.Running.performed -= OnRun;
        playerInputActions.Gameplay.Running.canceled -= OnRun;
        
        // Nonaktifkan tindakan input
        playerInputActions.Gameplay.Disable();
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

    private void OnAim(InputAction.CallbackContext context)
    {
        
    }
}