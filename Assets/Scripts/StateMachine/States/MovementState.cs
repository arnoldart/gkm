using UnityEngine;

/// <summary>
/// State untuk ketika pemain bergerak (berjalan atau berlari).
/// </summary>
public class MovementState : PlayerBaseState
{
    /// <summary>
    /// Membuat state Gerakan baru untuk pemain.
    /// </summary>
    /// <param name="stateMachine">State machine pemain.</param>
    public MovementState(PlayerStateMachine stateMachine) : base(stateMachine) {}

    public override void Enter()
    {
        PlayerStateMachine.PlayerAnimator.SetBool("isWalk", true);
        PlayerStateMachine.PlayerAnimator.SetFloat("runspeed", 0);
        PlayerStateMachine.CurrentSpeed = 0;
    }

    public override void UpdateLogic()
    {
        Vector3 movement = GetCameraAdjustedMovement();
        
        // Hitung target kecepatan berdasarkan state gerakan
        float targetSpeed = TentukanTargetKecepatan();
        PlayerStateMachine.CurrentSpeed = Mathf.Lerp(
            PlayerStateMachine.CurrentSpeed, 
            targetSpeed, 
            Time.deltaTime * PlayerStateMachine.Acceleration
        );

        // Perbarui parameter kecepatan animasi
        float targetRunSpeed = HitungTargetKecepatanLari();
        float currentRunSpeed = PlayerStateMachine.PlayerAnimator.GetFloat("runspeed");
        PlayerStateMachine.PlayerAnimator.SetFloat(
            "runspeed", 
            Mathf.Lerp(currentRunSpeed, targetRunSpeed, Time.deltaTime * PlayerStateMachine.Acceleration)
        );
        
        ApplyGravity();
        MoveCharacter(movement, PlayerStateMachine.CurrentSpeed);
        RotateTowardsMovementDirection(movement);

        // Tangani transisi
        if (PlayerStateMachine.MovementInput == Vector3.zero)
        {
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
            return;
        }

        if (PlayerStateMachine.JumpTriggered && PlayerStateMachine.Controller.isGrounded)
        {
            PlayerStateMachine.ChangeState(new JumpState(PlayerStateMachine));
            PlayerStateMachine.ConsumeJumpTrigger();
            return;
        }

        if (!PlayerStateMachine.Controller.isGrounded)
        {
            PlayerStateMachine.ChangeState(new FallingState(PlayerStateMachine));
        }
    }

    /// <summary>
    /// Menentukan kecepatan yang sesuai berdasarkan pengaturan scene dan status berlari.
    /// </summary>
    /// <returns>Target kecepatan gerakan.</returns>
    private float TentukanTargetKecepatan()
    {
        // Jika berlari diaktifkan dan tombol berlari ditekan
        if (PlayerStateMachine.IsRunning)
        {
            return PlayerStateMachine.RunSpeed;
        }
        
        // Default movement speed
        return PlayerStateMachine.WalkScene ? 
            PlayerStateMachine.WalkSpeed : 
            PlayerStateMachine.SlowRunSpeed;
    }
    
    /// <summary>
    /// Menghitung nilai kecepatan animasi untuk blend animasi berlari.
    /// </summary>
    /// <returns>Nilai parameter kecepatan animasi.</returns>
    private float HitungTargetKecepatanLari()
    {
        if (PlayerStateMachine.IsRunning && !PlayerStateMachine.WalkScene)
        {
            return 2f; // Lari cepat
        }
        else if (PlayerStateMachine.CurrentSpeed <= PlayerStateMachine.SlowRunSpeed &&
                 PlayerStateMachine.CurrentSpeed >= PlayerStateMachine.WalkSpeed && 
                 !PlayerStateMachine.WalkScene)
        {
            return 1f; // Lari lambat
        }
        else
        {
            return PlayerStateMachine.WalkScene ? 0f : 1f; // Jalan atau default
        }
    }
}