using UnityEngine;

public class VaultState : PlayerBaseState
{
    private ObstacleInfo obstacleInfo;
    private float vaultDuration = 0.3f;
    private float vaultTimer = 0f;
    private Vector3 startPosition;
    private Vector3 peakPosition;
    private Vector3 endPosition;
    private Quaternion startRotation;
    private Quaternion targetRotation;
    
    public VaultState(PlayerStateMachine stateMachine, ObstacleInfo obstacleInfo) : base(stateMachine)
    {
        this.obstacleInfo = obstacleInfo;
    }

    public override void Enter()
    {
        startPosition = PlayerStateMachine.transform.position;
        startRotation = PlayerStateMachine.transform.rotation;
        
        Vector3 forwardDirection = -obstacleInfo.hitInfo.normal;
        forwardDirection.y = 0;
        forwardDirection.Normalize();
        
        float vaultDepth = obstacleInfo.hitInfo.collider.bounds.size.z;
        
        peakPosition = new Vector3(
            obstacleInfo.hitInfo.point.x,
            obstacleInfo.heightInfo.point.y + 0.1f, // Lebih tinggi dari permukaan
            obstacleInfo.hitInfo.point.z
        );
        
        // Posisi akhir vault (di sisi lain obstacle)
        endPosition = obstacleInfo.hitInfo.point + (forwardDirection * (vaultDepth + 0.5f));
        endPosition.y = obstacleInfo.heightInfo.point.y;
        
        // Hitung rotasi target
        targetRotation = Quaternion.LookRotation(forwardDirection, Vector3.up);
        
        // Set animasi vault
        PlayerStateMachine.PlayerAnimator.SetTrigger("isVaulting");
        
        // Nonaktifkan controller
        PlayerStateMachine.Controller.enabled = false;
        
        // Reset timer
        vaultTimer = 0f;
    }

    public override void UpdateLogic()
    {
        vaultTimer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(vaultTimer / vaultDuration);
        
        // Gerakan vault dengan kurva parabola
        if (normalizedTime <= 0.5f)
        {
            // Separuh pertama: bergerak dari start ke puncak
            float firstHalfTime = normalizedTime * 2f;
            PlayerStateMachine.transform.position = Vector3.Lerp(startPosition, peakPosition, firstHalfTime);
        }
        else
        {
            // Separuh kedua: bergerak dari puncak ke akhir
            float secondHalfTime = (normalizedTime - 0.5f) * 2f;
            PlayerStateMachine.transform.position = Vector3.Lerp(peakPosition, endPosition, secondHalfTime);
        }
        
        // Interpolasi rotasi
        PlayerStateMachine.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime);
        
        // Transition ke Idle state ketika selesai
        if (vaultTimer >= vaultDuration)
        {
            PlayerStateMachine.ChangeState(new IdleState(PlayerStateMachine));
        }
    }

    public override void Exit()
    {
        // Aktifkan kembali controller
        PlayerStateMachine.Controller.enabled = true;
        
        // Reset vertical velocity
        PlayerStateMachine.VerticalVelocity = 0f;
    }
}
