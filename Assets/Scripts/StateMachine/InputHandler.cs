using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    
    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Gameplay.Enable();
    }

    private void OnEnable()
    {
        // Movement
        playerInputActions.Gameplay.Move.performed += OnMove;
        playerInputActions.Gameplay.Move.canceled += OnMove;
        
        // Jump
        playerInputActions.Gameplay.Jump.performed += OnJump;
        
        // Run
        playerInputActions.Gameplay.Running.performed += OnRun;
        playerInputActions.Gameplay.Running.canceled += OnRun;
    }

    private void OnDisable()
    {
        playerInputActions.Gameplay.Move.performed -= OnMove;
        playerInputActions.Gameplay.Move.canceled -= OnMove;
        playerInputActions.Gameplay.Jump.performed -= OnJump;
        playerInputActions.Gameplay.Running.performed -= OnRun;
        playerInputActions.Gameplay.Running.canceled -= OnRun;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        GetComponent<PlayerStateMachine>().movementInput = new Vector3(input.x, 0, input.y);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        GetComponent<PlayerStateMachine>().jumpTriggered = true;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        GetComponent<PlayerStateMachine>().isRunning = context.performed;
    }
}