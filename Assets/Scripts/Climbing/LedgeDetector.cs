using UnityEngine;
using System.Collections.Generic;
using Climbing;


[System.Serializable]
public class LedgeDetectionHit
{
    public RaycastHit horizontalHit;
    public RaycastHit verticalHit;
    public Ledge ledgeComponent;
    public Transform grabPoint;
    public float distance;
    public float angleAlignment;
}

public class LedgeDetector : MonoBehaviour
{
    [Header("Detection Parameters")]
    [SerializeField] private LayerMask ledgeMask = 1;
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private float capsuleRadius = 0.25f;
    [SerializeField] private float capsuleHeight = 1.5f;
    [SerializeField] private int capsuleCastIterations = 8;
    [SerializeField] private float capsuleCastDistance = 1f;
    
    [Header("Sphere Cast Parameters")]
    [SerializeField] private float sphereCastRadius = 0.2f;
    [SerializeField] private float sphereCastMaxHeight = 1.5f;
    [SerializeField] private float sphereCastDistance = 2f;
    
    [Header("Validation")]
    [SerializeField] private float minNormalDot = 0.5f;
    [SerializeField] private float minForwardAlignment = -0.1f;
    [SerializeField] private string[] ignoreTags = { "Player" };
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugDrawing = true;
    [SerializeField] private Color debugColor = Color.cyan;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float debugDuration = 0.1f;
    
    // Detection reference points
    [SerializeField] private Transform detectionCenter;
    
    // Cached results
    private List<LedgeDetectionHit> detectedLedges = new List<LedgeDetectionHit>();
    private LedgeDetectionHit bestLedge;
    
    private void Awake()
    {
        // Use this transform as detection center if none specified
        if (detectionCenter == null)
            detectionCenter = transform;
    }
    
    /// <summary>
    /// Main detection method that finds all available ledges around the detection center
    /// </summary>
    /// <returns>True if any valid ledges were found</returns>
    public bool DetectLedges()
    {
        detectedLedges.Clear();
        bestLedge = null;
        
        // First step: Quick overlap sphere check to see if there are any potential ledges nearby
        Collider[] nearbyColliders = Physics.OverlapSphere(detectionCenter.position, detectionRadius, ledgeMask, QueryTriggerInteraction.Collide);
        
        if (nearbyColliders.Length == 0)
            return false;
        
        // Perform detailed detection using capsule casting in multiple directions
        PerformCapsuleCastDetection();
        
        // Find the best ledge based on alignment and distance
        FindBestLedge();
        
        return detectedLedges.Count > 0;
    }
    
    /// <summary>
    /// Performs capsule casting in multiple directions to detect ledges
    /// </summary>
    private void PerformCapsuleCastDetection()
    {
        // Set capsule points for casting
        Vector3 capsuleBotPoint = detectionCenter.position + Vector3.down * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 capsuleTopPoint = detectionCenter.position + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        float angleStep = 360.0f / capsuleCastIterations;
        
        // Cast capsules in all directions around the detection center
        for (int i = 0; i < capsuleCastIterations; i++)
        {
            // Calculate current direction
            float currentAngleRad = i * angleStep * Mathf.Deg2Rad;
            Vector3 direction = new Vector3(Mathf.Cos(currentAngleRad), 0, Mathf.Sin(currentAngleRad)).normalized;
            
            // Perform capsule cast
            Vector3 startBot = capsuleBotPoint - direction * capsuleCastDistance;
            Vector3 startTop = capsuleTopPoint - direction * capsuleCastDistance;
            
            if (enableDebugDrawing)
                DrawCapsule(startBot, startTop, capsuleRadius, debugColor, debugDuration);
            
            RaycastHit[] hits = Physics.CapsuleCastAll(startBot, startTop, capsuleRadius, direction, 
                capsuleCastDistance * 2, ledgeMask, QueryTriggerInteraction.Collide);
            
            // Process each hit
            foreach (RaycastHit horizontalHit in hits)
            {
                ProcessHorizontalHit(horizontalHit);
            }
        }
    }
    
    /// <summary>
    /// Processes a horizontal hit to validate if it's a climbable ledge
    /// </summary>
    private void ProcessHorizontalHit(RaycastHit horizontalHit)
    {
        // Skip invalid hits
        if (horizontalHit.distance == 0) return;
        
        // Skip ignored tags
        if (System.Array.Exists(ignoreTags, tag => horizontalHit.collider.CompareTag(tag))) return;
        
        // Check if the surface is facing the right direction
        if (Vector3.Dot(transform.forward, -horizontalHit.normal) < minForwardAlignment) return;
        
        // Perform top detection to find the ledge surface
        RaycastHit verticalHit;
        if (!PerformTopDetection(horizontalHit, out verticalHit)) return;
        
        // Validate the ledge
        if (!ValidateLedge(horizontalHit, verticalHit)) return;
        
        // Create detection hit and add to results
        LedgeDetectionHit detectionHit = CreateDetectionHit(horizontalHit, verticalHit);
        detectedLedges.Add(detectionHit);
        
        if (enableDebugDrawing)
        {
            DrawSphere(horizontalHit.point, 0.1f, hitColor, debugDuration);
            DrawSphere(verticalHit.point, 0.1f, Color.green, debugDuration);
        }
    }
    
    /// <summary>
    /// Performs top detection using sphere cast to find the ledge surface
    /// </summary>
    private bool PerformTopDetection(RaycastHit horizontalHit, out RaycastHit verticalHit)
    {
        verticalHit = new RaycastHit();
        
        // Set start position for top detection
        Vector3 startSphere = horizontalHit.point;
        startSphere.y = detectionCenter.position.y + sphereCastMaxHeight;
        
        if (enableDebugDrawing)
            DrawSphere(startSphere, sphereCastRadius, debugColor, debugDuration);
        
        // Perform sphere cast downward
        RaycastHit[] topHits = Physics.SphereCastAll(startSphere, sphereCastRadius, Vector3.down, 
            sphereCastDistance, ledgeMask, QueryTriggerInteraction.Collide);
        
        // Find the best top hit
        List<RaycastHit> validTopHits = new List<RaycastHit>();
        
        foreach (RaycastHit topHit in topHits)
        {
            // Skip invalid hits
            if (topHit.distance == 0) continue;
            
            // Must be the same collider as horizontal hit
            if (topHit.collider != horizontalHit.collider) continue;
            
            // Must have upward facing normal
            if (Vector3.Dot(Vector3.up, topHit.normal) < minNormalDot) continue;
            
            validTopHits.Add(topHit);
        }
        
        if (validTopHits.Count == 0) return false;
        
        // Select the closest hit to the detection center height
        RaycastHit closestHit = validTopHits[0];
        float minDistance = Mathf.Abs(closestHit.point.y - detectionCenter.position.y);
        
        foreach (RaycastHit hit in validTopHits)
        {
            float distance = Mathf.Abs(hit.point.y - detectionCenter.position.y);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestHit = hit;
            }
        }
        
        verticalHit = closestHit;
        return true;
    }
    
    /// <summary>
    /// Validates if the detected ledge is suitable for climbing
    /// </summary>
    private bool ValidateLedge(RaycastHit horizontalHit, RaycastHit verticalHit)
    {
        // Check if there's enough space for the character
        // This is a simplified validation - you might want to add more sophisticated checks
        
        // Check height difference
        float heightDifference = verticalHit.point.y - detectionCenter.position.y;
        if (heightDifference < -1f || heightDifference > 2f) return false;
        
        // Additional validation can be added here based on your specific needs
        return true;
    }
    
    /// <summary>
    /// Creates a LedgeDetectionHit from the raycast results
    /// </summary>
    private LedgeDetectionHit CreateDetectionHit(RaycastHit horizontalHit, RaycastHit verticalHit)
    {
        LedgeDetectionHit detectionHit = new LedgeDetectionHit
        {
            horizontalHit = horizontalHit,
            verticalHit = verticalHit,
            distance = Vector3.Distance(detectionCenter.position, verticalHit.point)
        };
        
        // Calculate alignment with current forward direction
        detectionHit.angleAlignment = Vector3.Dot(transform.forward, -horizontalHit.normal);
        
        // Try to get Ledge component and find closest grab point
        if (verticalHit.collider.TryGetComponent(out Ledge ledge))
        {
            detectionHit.ledgeComponent = ledge;
            detectionHit.grabPoint = ledge.GetClosestPoint(verticalHit.point);
            
            // If ledge has grab points, update the hit information
            if (detectionHit.grabPoint != null)
            {
                detectionHit.horizontalHit.normal = detectionHit.grabPoint.forward;
                detectionHit.verticalHit.point = detectionHit.grabPoint.position;
                detectionHit.angleAlignment = Vector3.Dot(transform.forward, detectionHit.grabPoint.forward);
            }
        }
        
        return detectionHit;
    }
    
    /// <summary>
    /// Finds the best ledge from all detected ledges
    /// </summary>
    private void FindBestLedge()
    {
        if (detectedLedges.Count == 0) return;
        
        // Sort by angle alignment (best alignment first)
        detectedLedges.Sort((a, b) => b.angleAlignment.CompareTo(a.angleAlignment));
        
        bestLedge = detectedLedges[0];
    }
    
    /// <summary>
    /// Gets the best detected ledge
    /// </summary>
    public LedgeDetectionHit GetBestLedge()
    {
        return bestLedge;
    }
    
    /// <summary>
    /// Gets all detected ledges
    /// </summary>
    public List<LedgeDetectionHit> GetAllDetectedLedges()
    {
        return new List<LedgeDetectionHit>(detectedLedges);
    }
    
    /// <summary>
    /// Checks if any ledges are currently detected
    /// </summary>
    public bool HasDetectedLedges()
    {
        return detectedLedges.Count > 0;
    }
    
    /// <summary>
    /// Gets the closest ledge to a specific position
    /// </summary>
    public LedgeDetectionHit GetClosestLedge(Vector3 position)
    {
        if (detectedLedges.Count == 0) return null;
        
        LedgeDetectionHit closest = detectedLedges[0];
        float minDistance = Vector3.Distance(position, closest.verticalHit.point);
        
        foreach (var ledge in detectedLedges)
        {
            float distance = Vector3.Distance(position, ledge.verticalHit.point);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = ledge;
            }
        }
        
        return closest;
    }
    
    // Debug drawing methods
    private void DrawCapsule(Vector3 bottom, Vector3 top, float radius, Color color, float duration = 0f)
    {
        if (!enableDebugDrawing) return;
        
        Debug.DrawLine(bottom, top, color, duration);
        
        // Draw circles at top and bottom
        DrawCircle(bottom, radius, color, duration);
        DrawCircle(top, radius, color, duration);
    }
    
    private void DrawCircle(Vector3 center, float radius, Color color, float duration = 0f)
    {
        if (!enableDebugDrawing) return;
        
        int segments = 16;
        float angleStep = 360f / segments;
        
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;
            
            Debug.DrawLine(point1, point2, color, duration);
        }
    }
    
    private void DrawSphere(Vector3 center, float radius, Color color, float duration = 0f)
    {
        if (!enableDebugDrawing) return;
        
        // Draw three circles for a simple sphere representation
        DrawCircle(center, radius, color, duration);
        
        // Vertical circle 1
        for (int i = 0; i < 16; i++)
        {
            float angle1 = i * 22.5f * Mathf.Deg2Rad;
            float angle2 = (i + 1) * 22.5f * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(0, Mathf.Cos(angle1), Mathf.Sin(angle1)) * radius;
            Vector3 point2 = center + new Vector3(0, Mathf.Cos(angle2), Mathf.Sin(angle2)) * radius;
            
            Debug.DrawLine(point1, point2, color, duration);
        }
        
        // Vertical circle 2
        for (int i = 0; i < 16; i++)
        {
            float angle1 = i * 22.5f * Mathf.Deg2Rad;
            float angle2 = (i + 1) * 22.5f * Mathf.Deg2Rad;
            
            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * radius;
            
            Debug.DrawLine(point1, point2, color, duration);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (detectionCenter == null) return;
        
        // Draw detection radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRadius);
        
        // Draw capsule detection area
        Gizmos.color = debugColor;
        Vector3 capsuleBotPoint = detectionCenter.position + Vector3.down * (capsuleHeight * 0.5f - capsuleRadius);
        Vector3 capsuleTopPoint = detectionCenter.position + Vector3.up * (capsuleHeight * 0.5f - capsuleRadius);
        
        Gizmos.DrawWireSphere(capsuleBotPoint, capsuleRadius);
        Gizmos.DrawWireSphere(capsuleTopPoint, capsuleRadius);
        Gizmos.DrawLine(capsuleBotPoint, capsuleTopPoint);
        
        // Draw detected ledges
        if (Application.isPlaying && detectedLedges != null)
        {
            foreach (var ledge in detectedLedges)
            {
                Gizmos.color = ledge == bestLedge ? Color.green : Color.red;
                Gizmos.DrawSphere(ledge.verticalHit.point, 0.1f);
                
                if (ledge.grabPoint != null)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(ledge.grabPoint.position, 0.05f);
                    Gizmos.DrawRay(ledge.grabPoint.position, ledge.grabPoint.forward * 0.5f);
                }
            }
        }
    }
}