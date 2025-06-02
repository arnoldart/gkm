using UnityEngine;

public class EnvironmentChecker : MonoBehaviour
{
    [Header("Ray Settings")]
    public Vector3 rayOffset = new Vector3(0, 0.5f, 0);
    public float forwardRayLength = 1.0f;
    public float heightRayLength = 5.0f;
    public LayerMask obstacleLayer;

    [Header("Parkour Thresholds")]
    public float maxClimbHeight = 2.5f;   // Maksimum tinggi untuk climbing
    public float minClimbHeight = 0.5f;   // Minimum tinggi untuk climbing
    public float vaultMaxHeight = 1.2f;   // Maksimum tinggi untuk vaulting

    public ObstacleInfo CheckObstacle()
    {
        var obstacleInfo = new ObstacleInfo();

        // Forward ray untuk deteksi obstacle
        var rayOrigin = transform.position + rayOffset;
        obstacleInfo.hitFound = Physics.Raycast(rayOrigin, transform.forward, out obstacleInfo.hitInfo, forwardRayLength, obstacleLayer);

        Debug.DrawRay(rayOrigin, transform.forward * forwardRayLength, (obstacleInfo.hitFound) ? Color.red : Color.green);

        if (obstacleInfo.hitFound)
        {
            // Height ray untuk mendeteksi tinggi obstacle
            var heightOrigin = obstacleInfo.hitInfo.point + Vector3.up * heightRayLength;
            obstacleInfo.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out obstacleInfo.heightInfo, heightRayLength, obstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength, (obstacleInfo.heightHitFound) ? Color.red : Color.green);

            // Hitung tinggi obstacle jika kedua ray hit
            if (obstacleInfo.heightHitFound)
            {
                obstacleInfo.obstacleHeight = obstacleInfo.heightInfo.point.y - transform.position.y;
                obstacleInfo.obstacleType = DetermineObstacleType(obstacleInfo.obstacleHeight);
            }
        }

        return obstacleInfo;
    }

    private ParkourType DetermineObstacleType(float height)
    {
        if (height < minClimbHeight)
        {
            return ParkourType.None; // Terlalu kecil untuk parkour
        }
        else if (height <= vaultMaxHeight)
        {
            return ParkourType.Vault; // Cocok untuk vault
        }
        else if (height <= maxClimbHeight)
        {
            return ParkourType.Climb; // Cocok untuk climbing
        }
        else
        {
            return ParkourType.TooHigh; // Terlalu tinggi untuk parkour
        }
    }
}

public struct ObstacleInfo
{
    public bool hitFound;
    public bool heightHitFound;
    public float obstacleHeight;
    public ParkourType obstacleType;

    public RaycastHit heightInfo;
    public RaycastHit hitInfo;
}

public enum ParkourType
{
    None,
    Vault,
    Climb,
    TooHigh
}