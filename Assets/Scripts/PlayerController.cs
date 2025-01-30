using System;
using UnityEngine;

public class PlayerController : StateMachine
{
    public CharacterController Controller { get; private set; }
    public InputReader InputReader { get; private set; }
    public Transform CameraTransform { get; private set; }
    public ForceReceiver ForceReceiver { get; private set; }
    
    [SerializeField] private MovementConfig movementConfig;
    public MovementConfig MovementConfig => movementConfig;
    public bool isRunning { get; private set; }

    private void Awake()
    {
        Controller = GetComponent<CharacterController>();
        InputReader = GetComponent<InputReader>();
        CameraTransform = Camera.main.transform;
        ForceReceiver = GetComponent<ForceReceiver>();

        InputReader.RunningEvent += HandleRunning;
    }

    private void OnDestroy()
    {
        InputReader.RunningEvent -= HandleRunning;
    }

    private void HandleRunning()
    {
        isRunning = !isRunning;
    }

    private void Start()
    {
        ChangeState(new IdleState(this));
    }
}
