using UnityEngine;

public class WalkState : State
{
    private readonly Transform _playerTransform;
    private readonly Rigidbody _playerRigidbody;
    private readonly Vector2 _moveInput;
    private readonly MovementConfig _config;
    private readonly Transform _cameraTransform;

    private Vector3 _currentVelocity;

    public WalkState(StateMachine stateMachine, Transform playerTransform, Rigidbody playerRigidbody, Vector2 moveInput, MovementConfig config, Transform cameraTransform)
        : base(stateMachine)
    {
        _playerTransform = playerTransform;
        _playerRigidbody = playerRigidbody;
        _moveInput = moveInput;
        _config = config;
        _cameraTransform = cameraTransform;
    }

    public override void UpdatePhysics()
    {
        float speed = _config.walkSpeed;

        Vector3 cameraForward = _cameraTransform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        
        Vector3 cameraRight = _cameraTransform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();
        
        Vector3 targetDirection = (cameraRight * _moveInput.x + cameraForward * _moveInput.y).normalized;

        if (targetDirection.magnitude > 0.1f)
        {
            // Rotasi
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            _playerTransform.rotation = Quaternion.Slerp(
                _playerTransform.rotation,
                targetRotation,
                _config.rotationSpeed * Time.fixedDeltaTime
            );

            // Gerakan
            Vector3 targetPosition = _playerTransform.position + targetDirection * speed * Time.fixedDeltaTime;
            Vector3 smoothedPosition = Vector3.SmoothDamp(
                _playerTransform.position,
                targetPosition,
                ref _currentVelocity,
                _config.movementSmoothTime
            );
            _playerRigidbody.MovePosition(smoothedPosition);
        }
    }
}