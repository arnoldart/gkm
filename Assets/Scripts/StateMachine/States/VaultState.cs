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
        
        // Ganti SetTrigger dengan CrossFadeInFixedTime
        PlayAnimation("Vault", 0.1f);
        
        // Nonaktifkan controller
        PlayerStateMachine.Controller.enabled = false;
        
        // Reset timer
        vaultTimer = 0f;
    }

    public override void UpdateLogic()
    {
        vaultTimer += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(vaultTimer / vaultDuration);
        
        // Gerakan vault dengan kurva yang lebih halus
        if (normalizedTime <= 0.5f)
        {
            // Pertama 50%: dari awal ke puncak
            float upProgress = normalizedTime / 0.5f;
            PlayerStateMachine.transform.position = Vector3.Lerp(startPosition, peakPosition, SmoothStep(upProgress));
        }
        else
        {
            // 50% sisanya: dari puncak ke akhir
            float downProgress = (normalizedTime - 0.5f) / 0.5f;
            PlayerStateMachine.transform.position = Vector3.Lerp(peakPosition, endPosition, SmoothStep(downProgress));
        }
        
        // Interpolasi rotasi
        PlayerStateMachine.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, normalizedTime);
        
        // Transition ke Idle state ketika selesai
        if (vaultTimer >= vaultDuration)
        {
            // Pastikan posisi akhir tepat
            PlayerStateMachine.transform.position = endPosition;
            PlayerStateMachine.transform.rotation = targetRotation;
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
    
    // Fungsi interpolasi SmoothStep untuk gerakan lebih halus
    private float SmoothStep(float x)
    {
        return x * x * (3 - 2 * x);
    }
}
