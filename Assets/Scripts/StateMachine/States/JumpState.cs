using UnityEngine;

public class JumpState : State
{
    private float _verticalVelocity;
    private bool _hasJumped;

    public JumpState(StateMachine stateMachine, Transform playerTransform, CharacterController characterController, Vector2 moveInput, MovementConfig config, Transform cameraTransform)
        : base(stateMachine)
    {
        _hasJumped = true; // Set hasJumped true saat memasuki state ini
    }

    // public override void Enter()
    // {
    //     base.Enter();
    //     _verticalVelocity = Mathf.Sqrt(2 * _config.jumpHeight * _config.gravity); // Hitung kecepatan vertikal awal
    // }
    //
    // public override void UpdatePhysics()
    // {
    //     // Terapkan gravitasi
    //     if (!_characterController.isGrounded)
    //     {
    //         _verticalVelocity -= _config.gravity * Time.deltaTime; // Terapkan gravitasi
    //     }
    //     else
    //     {
    //         // Reset kecepatan vertikal saat di tanah
    //         if (_verticalVelocity < 0)
    //         {
    //             _verticalVelocity = 0;
    //         }
    //     }
    //
    //     // Hitung arah gerakan
    //     Vector3 moveDirection = CalculateMovementDirection();
    //     Vector3 velocity = moveDirection * GetSpeed() + Vector3.up * _verticalVelocity;
    //
    //     // Gerakkan karakter
    //     _characterController.Move(velocity * Time.deltaTime);
    //
    //     // Cek apakah karakter sudah mendarat
    //     if (_characterController.isGrounded && _verticalVelocity <= 0)
    //     {
    //         _stateMachine.ChangeState(new IdleState(_stateMachine)); // Kembali ke IdleState saat mendarat
    //     }
    // }
    //
    // public override void UpdateLogic()
    // {
    //     base.UpdateLogic();
    //     
    //     if (!_characterController.isGrounded)
    //     {
    //         _hasJumped = false; // Reset status jump
    //         _stateMachine.ChangeState(new IdleState(_stateMachine)); // Kembali ke IdleState saat mendarat
    //     }
    // }
    //
    //
    // protected override float GetSpeed()
    // {
    //     return _config.jumpMoveSpeed; // Kecepatan saat melompat
    // }
}