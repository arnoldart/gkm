using UnityEngine;

[System.Serializable]
public class ClimbableSettings
{
    [Header("Climbable Properties")]
    public bool isClimbable = true;
    public float climbSpeedModifier = 1f;
    
    [Header("Surface Properties")]
    public bool allowVerticalClimbing = true;
    public bool allowHorizontalClimbing = true;
    
    [Header("Visual Feedback")]
    public Material climbableMaterial;
    public Color climbableColor = Color.green;
}

public class ClimbableObject : MonoBehaviour
{
    [SerializeField] private ClimbableSettings settings = new ClimbableSettings();
    
    private Renderer objectRenderer;
    private Material originalMaterial;

    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
        }
        
        // Ensure the object has a collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogWarning($"ClimbableObject '{gameObject.name}' doesn't have a Collider component. Adding BoxCollider.");
            gameObject.AddComponent<BoxCollider>();
        }
    }

    public ClimbableSettings GetClimbableSettings()
    {
        return settings;
    }

    public bool IsClimbable()
    {
        return settings.isClimbable;
    }

    public float GetClimbSpeedModifier()
    {
        return settings.climbSpeedModifier;
    }

    public bool AllowsVerticalClimbing()
    {
        return settings.allowVerticalClimbing;
    }

    public bool AllowsHorizontalClimbing()
    {
        return settings.allowHorizontalClimbing;
    }

    void OnDrawGizmos()
    {
        if (settings.isClimbable)
        {
            Gizmos.color = settings.climbableColor;
            Gizmos.matrix = transform.localToWorldMatrix;
            
            // Draw wireframe cube to show climbable area
            if (GetComponent<BoxCollider>() != null)
            {
                BoxCollider box = GetComponent<BoxCollider>();
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (GetComponent<Collider>() != null)
            {
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }
        }
    }

    // Method to highlight the object when player is near
    public void HighlightAsClimbable(bool highlight)
    {
        if (objectRenderer == null) return;

        if (highlight && settings.climbableMaterial != null)
        {
            objectRenderer.material = settings.climbableMaterial;
        }
        else if (!highlight)
        {
            objectRenderer.material = originalMaterial;
        }
    }
}
