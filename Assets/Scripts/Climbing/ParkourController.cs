using UnityEngine;

[RequireComponent(typeof(EnvironmentChecker))]
public class ParkourController : MonoBehaviour
{
    public EnvironmentChecker environmentChecker;
    private PlayerStateMachine stateMachine;
    
    [Header("Parkour Input Settings")]
    public KeyCode parkourKey = KeyCode.Space;
    
    [Header("Parkour Debug")]
    public bool showDebugMessages = true;
    
    private bool isParkourRequested = false;
    private ObstacleInfo currentObstacle;

    private void Awake()
    {
        // Dapatkan komponen yang diperlukan
        if (environmentChecker == null)
            environmentChecker = GetComponent<EnvironmentChecker>();
            
        stateMachine = GetComponent<PlayerStateMachine>();
    }

    private void Update()
    {
        // Periksa input parkour
        isParkourRequested = Input.GetKeyDown(parkourKey);
        
        // Periksa obstacle di depan pemain
        currentObstacle = environmentChecker.CheckObstacle();
        
        // Handler parkour request
        if (isParkourRequested && currentObstacle.hitFound)
        {
            TryPerformParkour();
        }
        
        // Debug messages
        if (showDebugMessages && currentObstacle.hitFound)
        {
            string debugMessage = $"Obstacle: {currentObstacle.hitInfo.transform.name}, " +
                                  $"Height: {currentObstacle.obstacleHeight:F2}m, " +
                                  $"Type: {currentObstacle.obstacleType}";
            Debug.Log(debugMessage);
        }
    }
    
    private void TryPerformParkour()
    {
        // Pastikan state machine ada
        if (stateMachine == null) return;
        
        // Transisi ke state yang sesuai berdasarkan tipe obstacle
        switch (currentObstacle.obstacleType)
        {
            case ParkourType.Vault:
                stateMachine.ChangeState(new VaultState(stateMachine, currentObstacle));
                break;
                
            case ParkourType.Climb:
                stateMachine.ChangeState(new ClimbState(stateMachine, currentObstacle));
                break;
                
            case ParkourType.TooHigh:
                Debug.Log("Obstacle terlalu tinggi untuk diparkour");
                break;
                
            case ParkourType.None:
            default:
                // Jika tidak ada parkour yang sesuai, lakukan jump biasa jika sedang di tanah
                if (stateMachine.Controller.isGrounded)
                {
                    stateMachine.JumpTriggered = true;
                }
                break;
        }
    }
}
