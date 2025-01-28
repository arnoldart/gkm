using UnityEngine;

public class WalkState : State
{
    private readonly Transform _playerTransform;
    private readonly Rigidbody _playerRigidbody;
    private readonly Vector2 _moveInput;
    private readonly MovementConfig _config;

    private Vector3 _currentVelocity;

    public WalkState(StateMachine stateMachine, Transform playerTransform, Rigidbody playerRigidbody, Vector2 moveInput, MovementConfig config)
        : base(stateMachine)
    {
        _playerTransform = playerTransform;
        _playerRigidbody = playerRigidbody;
        _moveInput = moveInput;
        _config = config;
    }

    public override void UpdatePhysics()
    {
        float speed = _config.walkSpeed;

        Vector3 targetDirection = new Vector3(_moveInput.x, 0, _moveInput.y).normalized;
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