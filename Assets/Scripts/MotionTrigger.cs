using UnityEngine;

/// <summary>
/// Trigger untuk memindahkan player dengan transisi halus dari startPosObject ke targetPosObject
/// </summary>
public class MotionTrigger : MonoBehaviour
{    [Header("Teleport Positions")]
    [Tooltip("Posisi awal transisi")]
    public GameObject startPosObject;
    
    [Tooltip("Posisi akhir transisi")]
    public GameObject targetPosObject;
    
    [Header("Teleport Settings")]
    [Tooltip("Durasi transisi untuk setiap fase (dalam detik)")]
    [Range(0.1f, 5.0f)]
    public float transitionDuration = 1.0f;    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Dapatkan PlayerStateMachine dari player
            PlayerStateMachine playerStateMachine = other.GetComponent<PlayerStateMachine>();
            
            if (playerStateMachine != null)
            {
                // Ganti ke TeleportState untuk transisi halus dengan durasi yang dapat dikonfigurasi
                playerStateMachine.ChangeState(new TeleportState(playerStateMachine, startPosObject, targetPosObject, transitionDuration));
                Debug.Log($"MotionTrigger: Initiated smooth teleport sequence with duration: {transitionDuration}s per phase");
            }
            else
            {
                Debug.LogError("MotionTrigger: PlayerStateMachine not found on player object");
            }
        }
    }
}
