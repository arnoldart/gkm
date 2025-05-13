using UnityEngine;

/// <summary>
/// State untuk ketika pemain melompat (bergerak ke atas).
/// </summary>
public class JumpState : PlayerBaseState
{
    private float momentumDecayRate = 0.95f; // Rate penurunan momentum per frame
    private float minMomentumThreshold = 0.1f; // Threshold minimum momentum

    /// <summary>
    /// Membuat state Lompat baru untuk pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    public JumpState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        // Tetapkan kecepatan vertikal awal
        PlayerStateMachine.VerticalVelocity = PlayerStateMachine.JumpForce;
        
        // Ganti SetTrigger dengan CrossFadeInFixedTime
        PlayAnimation("Jump", 0.1f);
    }

    public override void UpdateLogic()
    {
        // Terapkan momentum horizontal (tanpa Y) dengan decay
        Vector3 momentumMovement = PlayerStateMachine.JumpMomentum * PlayerStateMachine.JumpMomentumMultiplier;
        momentumMovement.y = 0f;
        Vector3 inputMovement = GetCameraAdjustedMovement() * PlayerStateMachine.AirControl;
        inputMovement.y = 0f;
        // Gabungkan momentum horizontal dan input player
        Vector3 finalMovement = momentumMovement + inputMovement;
        // Update decay momentum
        PlayerStateMachine.JumpMomentum *= momentumDecayRate;
        if (PlayerStateMachine.JumpMomentum.magnitude < minMomentumThreshold)
        {
            PlayerStateMachine.JumpMomentum = Vector3.zero;
        }
        ApplyGravity();
        // Gunakan finalMovement untuk XZ, VerticalVelocity untuk Y
        Vector3 move = new Vector3(finalMovement.x, PlayerStateMachine.VerticalVelocity, finalMovement.z);
        PlayerStateMachine.Controller.Move(move * Time.deltaTime);
        RotateTowardsMovementDirection(finalMovement);
        // Transisi ke idle jika menyentuh tanah
        if (PlayerStateMachine.Controller.isGrounded)
        {
            PlayerStateMachine.JumpMomentum = Vector3.zero;
            PlayerStateMachine.ConsumeJumpTrigger();
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
        }
    }

    public override void Exit() 
    {
        // Reset momentum saat keluar dari state lompat
        PlayerStateMachine.JumpMomentum = Vector3.zero;
    }
}