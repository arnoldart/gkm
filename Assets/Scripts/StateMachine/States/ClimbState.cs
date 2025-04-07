using UnityEngine;

public class ClimbState : PlayerBaseState
{
    private ObstacleInfo obstacleInfo;
    private float climbDuration = 1.5f;
    private float climbTimer = 0f;
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    
    public ClimbState(PlayerStateMachine stateMachine, ObstacleInfo obstacleInfo) : base(stateMachine)
    {
        this.obstacleInfo = obstacleInfo;
    }

    public override void Enter()
    {
        // Simpan posisi dan rotasi awal
        startPosition = PlayerStateMachine.transform.position;
        startRotation = PlayerStateMachine.transform.rotation;
        
        // Hitung posisi target (di atas obstacle)
        targetPosition = new Vector3(
            obstacleInfo.heightInfo.point.x,
            obstacleInfo.heightInfo.point.y + 0.1f, // Sedikit di atas permukaan
            obstacleInfo.heightInfo.point.z
        );
        
        // Hitung offset dari tepi obstacle
        Vector3 forwardDirection = -obstacleInfo.heightInfo.normal;
        forwardDirection.y = 0; // Pastikan hanya arah horizontal
        forwardDirection.Normalize();
        
        // Sesuaikan posisi target agar karakter tidak jatuh dari tepi
        targetPosition += forwardDirection * 0.3f;
        
        // Hitung rotasi target (menghadap arah normal permukaan)
        targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
        
        // Set animasi climbing
        PlayerStateMachine.PlayerAnimator.SetBool("isClimbing", true);
        
        // Nonaktifkan controller sementara
        PlayerStateMachine.Controller.enabled = false;
        
        // Reset timer
        climbTimer = 0f;
    }

    public override void UpdateLogic()
    {
        climbTimer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(climbTimer / climbDuration);
        
        // Interpolasi posisi dan rotasi
        PlayerStateMachine.transform.position = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
        PlayerStateMachine.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime);
        
        // Transition ke Idle state ketika selesai
        if (climbTimer >= climbDuration)
        {
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
        }
    }

    public override void Exit()
    {
        // Reset animasi
        PlayerStateMachine.PlayerAnimator.SetBool("isClimbing", false);
        
        // Aktifkan kembali controller
        PlayerStateMachine.Controller.enabled = true;
        
        // Reset vertical velocity
        PlayerStateMachine.VerticalVelocity = 0f;
    }
}
