using UnityEngine;

/// <summary>
/// Example script demonstrating how to use the LedgeDetector component
/// </summary>
public class LedgeDetectorExample : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private LedgeDetector ledgeDetector;
    
    [Header("Input")]
    [SerializeField] private KeyCode detectKey = KeyCode.E;
    [SerializeField] private bool autoDetect = true;
    [SerializeField] private float detectionInterval = 0.1f;
    
    [Header("Display")]
    [SerializeField] private bool showDebugInfo = true;
    
    private float lastDetectionTime;
    
    private void Awake()
    {
        // Get LedgeDetector component if not assigned
        if (ledgeDetector == null)
            ledgeDetector = GetComponent<LedgeDetector>();
    }
    
    private void Update()
    {
        // Manual detection with key press
        if (Input.GetKeyDown(detectKey))
        {
            PerformDetection();
        }
        
        // Auto detection with interval
        if (autoDetect && Time.time - lastDetectionTime >= detectionInterval)
        {
            PerformDetection();
            lastDetectionTime = Time.time;
        }
    }
    
    private void PerformDetection()
    {
        if (ledgeDetector == null) return;
        
        bool foundLedges = ledgeDetector.DetectLedges();
        
        if (showDebugInfo)
        {
            if (foundLedges)
            {
                var bestLedge = ledgeDetector.GetBestLedge();
                var allLedges = ledgeDetector.GetAllDetectedLedges();
                
                Debug.Log($"[LedgeDetector] Found {allLedges.Count} ledge(s)");
                
                if (bestLedge != null)
                {
                    Debug.Log($"[LedgeDetector] Best ledge: {bestLedge.verticalHit.collider.name} " +
                              $"at distance {bestLedge.distance:F2}m, alignment {bestLedge.angleAlignment:F2}");
                    
                    if (bestLedge.ledgeComponent != null)
                    {
                        // Debug.Log($"[LedgeDetector] Ledge component found with {bestLedge.ledgeComponent.GrabPoints.Count} grab points");
                        
                        if (bestLedge.grabPoint != null)
                        {
                            Debug.Log($"[LedgeDetector] Closest grab point: {bestLedge.grabPoint.name}");
                        }
                    }
                }
            }
            else
            {
                Debug.Log("[LedgeDetector] No ledges detected");
            }
        }
    }
    
    private void OnGUI()
    {
        if (!showDebugInfo || ledgeDetector == null) return;
          GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.BeginVertical("box");
        
        GUILayout.Label("Ledge Detector Debug");
        
        if (GUILayout.Button($"Detect Ledges ({detectKey})"))
        {
            PerformDetection();
        }
        
        GUILayout.Space(10);
        
        if (ledgeDetector.HasDetectedLedges())
        {
            var allLedges = ledgeDetector.GetAllDetectedLedges();
            var bestLedge = ledgeDetector.GetBestLedge();
            
            GUILayout.Label($"Detected Ledges: {allLedges.Count}");
            
            if (bestLedge != null)
            {
                GUILayout.Label($"Best Ledge: {bestLedge.verticalHit.collider.name}");
                GUILayout.Label($"Distance: {bestLedge.distance:F2}m");
                GUILayout.Label($"Alignment: {bestLedge.angleAlignment:F2}");
                
                if (bestLedge.ledgeComponent != null)
                {
                    // GUILayout.Label($"Grab Points: {bestLedge.ledgeComponent.GrabPoints.Count}");
                }
            }
        }
        else
        {
            GUILayout.Label("No ledges detected");
        }
        
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
