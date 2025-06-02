using UnityEngine;

/// <summary>
/// Attach this to any GameObject that should deal damage to entities with a HealthSystem
/// Can be used for enemies, traps, projectiles, etc.
/// </summary>
public class Damager : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private bool damageOnTriggerEnter = true;
    [SerializeField] private bool damageOnTriggerStay = false;
    [SerializeField] private float damageInterval = 1.0f; // Used for continuous damage with TriggerStay
    [SerializeField] private string[] targetTags = { "Player", "Enemy", "Damageable" };
    
    private float lastDamageTime;

    /// <summary>
    /// Apply damage to a target with a HealthSystem
    /// </summary>
    public int ApplyDamage(GameObject target)
    {
        if (target == null) return 0;
        
        HealthSystem healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem == null) return 0;
        
        int damageDealt = healthSystem.TakeDamage(damageAmount, gameObject);
        return damageDealt;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!damageOnTriggerEnter) return;
        
        if (ShouldDamage(other.gameObject))
        {
            ApplyDamage(other.gameObject);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (!damageOnTriggerStay) return;
        
        if (Time.time >= lastDamageTime + damageInterval)
        {
            if (ShouldDamage(other.gameObject))
            {
                if (ApplyDamage(other.gameObject) > 0)
                {
                    lastDamageTime = Time.time;
                }
            }
        }
    }
    
    private bool ShouldDamage(GameObject target)
    {
        if (target == null) return false;
        
        // If no tags are specified, damage any object with a HealthSystem
        if (targetTags.Length == 0) return true;
        
        // Check if the target has one of the specified tags
        foreach (string tag in targetTags)
        {
            if (target.CompareTag(tag))
            {
                return true;
            }
        }
        
        return false;
    }
    
    // For manual damage application (e.g., for ranged attacks or abilities)
    public void SetDamage(int newDamage)
    {
        damageAmount = newDamage;
    }
    
    // For visualizing in the editor
    private void OnDrawGizmosSelected()
    {
        // Can add visual indicators for damage areas if needed
        // For example, draw a red sphere for AOE damage
    }
}