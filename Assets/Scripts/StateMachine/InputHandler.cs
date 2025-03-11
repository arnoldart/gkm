using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    private PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        playerInputActions.Gameplay.Enable();
    }

    private void OnEnable()
    {
        playerInputActions.Gameplay.Move.performed += OnMove;
        playerInputActions.Gameplay.Move.canceled += OnMove;
        playerInputActions.Gameplay.Jump.performed += OnJump;
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
        playerStateMachine.movementInput = new Vector3(input.x, 0, input.y);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        playerStateMachine.jumpTriggered = true;
    }

    private void OnRun(InputAction.CallbackContext context)
    {
        if (!playerStateMachine.walkScene)
        {
            playerStateMachine.isRunning = context.performed;
        }
        else
        {
            playerStateMachine.isRunning = false;
        }
    }
}