using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CharacterController))]
public class PlayerClimbFixed : MonoBehaviour
{
    [Header("Climbing Settings")]
    [SerializeField] private float climbSpeed = 5f;
    [SerializeField] private float climbDistance = 1f;
    [SerializeField] private LayerMask climbableLayerMask = 1;
    [SerializeField] private float additionalOffset = 0.02f; // Small buffer to prevent clipping
    [SerializeField] private bool showDebugGizmos = true;
    
    [Header("Gravity Settings")]
    [SerializeField] private float gravityStrength = 9.81f;
    
    [Header("Status (Read Only)")]
    [SerializeField] private bool isClimbing = false;
    
    private CharacterController controller;
    private Vector3 lastClimbNormal;
    private GameObject currentClimbableObject;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            Debug.LogError("PlayerClimb requires a CharacterController component!");
        }
    }

    void Update()
    {
        HandleClimbing();
    }

    void HandleClimbing()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 input = SquareToCircle(new Vector2(h, v));

        RaycastHit hit;
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.height * 0.5f);
        
        // Check if there's a climbable surface in front
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, climbDistance, climbableLayerMask))
        {
            if (!isClimbing)
            {
                StartClimbing(hit);
            }
            
            // Update climbing
            UpdateClimbing(input, hit);
        }
        else if (isClimbing)
        {
            StopClimbing();
        }
        
        // If not climbing, apply gravity
        if (!isClimbing)
        {
            ApplyGravity();
        }
    }

    void StartClimbing(RaycastHit hit)
    {
        isClimbing = true;
        lastClimbNormal = hit.normal;
        currentClimbableObject = hit.collider.gameObject;
        
        // Orient player to face away from the wall
        transform.forward = -hit.normal;
        
        // Calculate optimal distance based on character controller and surface
        float optimalDistance = CalculateOptimalStickDistance(hit);
        
        // Position player at the calculated distance from the wall
        Vector3 targetPosition = hit.point + hit.normal * optimalDistance;
        controller.enabled = false;
        transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        controller.enabled = true;
        
        Debug.Log($"Started climbing on: {currentClimbableObject.name} at distance: {optimalDistance:F3}");
    }

    void UpdateClimbing(Vector2 input, RaycastHit hit)
    {
        // Update orientation to wall
        transform.forward = -hit.normal;
        lastClimbNormal = hit.normal;
        
        // Calculate movement direction relative to the wall
        Vector3 rightDirection = Vector3.Cross(Vector3.up, hit.normal).normalized;
        Vector3 upDirection = Vector3.up;
        
        Vector3 moveDirection = (rightDirection * input.x + upDirection * input.y) * climbSpeed;
        
        // Move the character
        controller.Move(moveDirection * Time.deltaTime);
        
        // Calculate optimal distance and keep player stuck to the wall
        float optimalDistance = CalculateOptimalStickDistance(hit);
        Vector3 targetPosition = hit.point + hit.normal * optimalDistance;
        Vector3 currentPos = transform.position;
        Vector3 stickPosition = new Vector3(targetPosition.x, currentPos.y, targetPosition.z);
        
        controller.enabled = false;
        transform.position = Vector3.Lerp(currentPos, stickPosition, 10f * Time.deltaTime);
        controller.enabled = true;
    }

    void StopClimbing()
    {
        isClimbing = false;
        currentClimbableObject = null;
        Debug.Log("Stopped climbing");
    }

    void ApplyGravity()
    {
        // Simple gravity application
        Vector3 gravity = Vector3.down * gravityStrength * Time.deltaTime;
        controller.Move(gravity);
    }

    Vector2 SquareToCircle(Vector2 input)
    {
        return (input.sqrMagnitude >= 1f) ? input.normalized : input;
    }

    /// <summary>
    /// Automatically calculates the optimal distance to stick to the wall
    /// based on the character controller's radius and the hit surface
    /// </summary>
    float CalculateOptimalStickDistance(RaycastHit hit)
    {
        // Base distance is the character controller radius
        float baseDistance = controller.radius;
        
        // Get the collider type and adjust accordingly
        Collider hitCollider = hit.collider;
        
        if (hitCollider is BoxCollider)
        {
            // For box colliders, use the exact hit point
            return baseDistance + additionalOffset;
        }
        else if (hitCollider is SphereCollider sphereCol)
        {
            // For sphere colliders, account for the curvature
            Vector3 centerToHit = hit.point - sphereCol.bounds.center;
            float distanceFromCenter = centerToHit.magnitude;
            float sphereRadius = sphereCol.radius * Mathf.Max(sphereCol.transform.localScale.x, 
                                                               sphereCol.transform.localScale.y, 
                                                               sphereCol.transform.localScale.z);
            
            // Calculate the offset needed to account for sphere curvature
            float curvatureOffset = sphereRadius - distanceFromCenter;
            return baseDistance + additionalOffset + Mathf.Max(0, curvatureOffset * 0.1f);
        }
        else if (hitCollider is CapsuleCollider)
        {
            // For capsule colliders, similar to sphere but less pronounced
            return baseDistance + additionalOffset * 0.5f;
        }
        else if (hitCollider is MeshCollider)
        {
            // For mesh colliders, do additional raycasting to find the true surface
            return CalculateMeshColliderDistance(hit);
        }
        
        // Default fallback
        return baseDistance + additionalOffset;
    }

    /// <summary>
    /// Special handling for mesh colliders to find the optimal distance
    /// </summary>
    float CalculateMeshColliderDistance(RaycastHit hit)
    {
        // Cast multiple rays around the hit point to understand the surface better
        Vector3 hitPoint = hit.point;
        Vector3 normal = hit.normal;
        float baseDistance = controller.radius;
        
        // Cast rays in a small pattern around the hit point
        float minDistance = float.MaxValue;
        int rayCount = 5;
        float raySpread = controller.radius * 0.5f;
        
        for (int i = 0; i < rayCount; i++)
        {
            for (int j = 0; j < rayCount; j++)
            {
                // Create a grid of rays around the hit point
                float offsetX = (i - rayCount/2) * raySpread / rayCount;
                float offsetY = (j - rayCount/2) * raySpread / rayCount;
                
                Vector3 rightDir = Vector3.Cross(normal, Vector3.up).normalized;
                Vector3 upDir = Vector3.Cross(rightDir, normal).normalized;
                
                Vector3 rayStart = hitPoint + normal * 0.5f + rightDir * offsetX + upDir * offsetY;
                Vector3 rayDir = -normal;
                
                RaycastHit localHit;
                if (Physics.Raycast(rayStart, rayDir, out localHit, 1f, climbableLayerMask))
                {
                    float distance = localHit.distance;
                    minDistance = Mathf.Min(minDistance, distance);
                }
            }
        }
        
        // Use the minimum distance found, or fallback to base distance
        if (minDistance != float.MaxValue)
        {
            return baseDistance + additionalOffset + (0.5f - minDistance);
        }
        
        return baseDistance + additionalOffset;
    }

    // Public getter methods for external scripts
    public bool IsClimbing()
    {
        return isClimbing;
    }

    public GameObject GetCurrentClimbableObject()
    {
        return currentClimbableObject;
    }

    public LayerMask GetClimbableLayerMask()
    {
        return climbableLayerMask;
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;
        
        // Get the CharacterController component for height calculation
        CharacterController cc = GetComponent<CharacterController>();
        if (cc == null) return;
        
        // Draw raycast for debugging
        Vector3 rayOrigin = transform.position + Vector3.up * (cc.height * 0.5f);
        Gizmos.color = isClimbing ? Color.green : Color.red;
        Gizmos.DrawRay(rayOrigin, transform.forward * climbDistance);
        
        // Draw a sphere at the ray origin
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(rayOrigin, 0.1f);
        
        // Draw climb detection area
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(rayOrigin + transform.forward * climbDistance, 0.05f);
        
        // Draw character controller radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * (cc.height * 0.5f), cc.radius);
        
        // If climbing, show the calculated stick distance
        if (isClimbing && currentClimbableObject != null)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, transform.forward, out hit, climbDistance, climbableLayerMask))
            {
                float optimalDistance = CalculateOptimalStickDistance(hit);
                Vector3 optimalPosition = hit.point + hit.normal * optimalDistance;
                
                // Draw the optimal position
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(optimalPosition, 0.1f);
                
                // Draw line from hit point to optimal position
                Gizmos.color = Color.white;
                Gizmos.DrawLine(hit.point, optimalPosition);
                
                // Draw distance text in scene (you'll see this in scene view)
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(optimalPosition + Vector3.up * 0.2f, 
                    $"Distance: {optimalDistance:F3}m");
                #endif
            }
        }
    }
}
