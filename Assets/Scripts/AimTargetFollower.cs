using UnityEngine;

/// <summary>
/// Menempatkan AimTarget di titik tengah kamera pada jarak tertentu dari kamera.
/// Attach script ini di kamera atau player, lalu drag AimTarget ke Inspector.
/// </summary>
public class AimTargetFollower : MonoBehaviour
{
    [Tooltip("Target yang akan diposisikan di tengah kamera (misal: untuk IK)")]
    public Transform aimTarget;
    [Tooltip("Jarak AimTarget dari kamera")]
    public float distanceFromCamera = 10f;
    [Tooltip("Kamera yang digunakan (jika null, pakai Camera.main)")]
    public Camera targetCamera;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (aimTarget == null || targetCamera == null)
            return;
        // Buat ray dari tengah viewport
        Ray ray = targetCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        // Tempatkan AimTarget di titik pada ray sejauh distanceFromCamera
        aimTarget.position = ray.origin + ray.direction * distanceFromCamera;
    }
}
