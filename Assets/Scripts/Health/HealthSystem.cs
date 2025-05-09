using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth = 100;
    
    public int MaxHealth { get => maxHealth; private set => maxHealth = value; }
    public int CurrentHealth { get => currentHealth; private set => currentHealth = value; }
    
    public bool IsInvulnerable { get; set; } = false;
    public bool IsDead => CurrentHealth <= 0;
    
    // Events
    public event Action<int, int> OnHealthChanged; // Parameters: damageAmount, currentHealth
    public event Action<int> OnDamage; // Parameter: damageAmount
    public event Action<int> OnHeal; // Parameter: healAmount
    public event Action OnDeath;

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public int TakeDamage(int damageAmount, GameObject damageSource = null)
    {
        if (IsDead || IsInvulnerable || damageAmount <= 0)
            return 0;

        int actualDamage = Mathf.Min(CurrentHealth, damageAmount);
        CurrentHealth -= actualDamage;
        
        // Trigger events
        OnDamage?.Invoke(actualDamage);
        OnHealthChanged?.Invoke(-actualDamage, CurrentHealth);
        
        Debug.Log($"{gameObject.name} took {actualDamage} damage. Health: {CurrentHealth}/{MaxHealth}");
        
        if (IsDead)
        {
            HandleDeath();
        }
        
        return actualDamage;
    }
    
    public int Heal(int healAmount)
    {
        if (IsDead || healAmount <= 0)
            return 0;
            
        int actualHeal = Mathf.Min(MaxHealth - CurrentHealth, healAmount);
        CurrentHealth += actualHeal;
        
        // Trigger events
        OnHeal?.Invoke(actualHeal);
        OnHealthChanged?.Invoke(actualHeal, CurrentHealth);
        
        Debug.Log($"{gameObject.name} healed for {actualHeal}. Health: {CurrentHealth}/{MaxHealth}");
        
        return actualHeal;
    }
    
    public void SetMaxHealth(int newMaxHealth, bool scaleCurrentHealth = true)
    {
        if (newMaxHealth <= 0)
            return;
            
        float healthPercent = (float)CurrentHealth / MaxHealth;
        MaxHealth = newMaxHealth;
        
        if (scaleCurrentHealth)
        {
            CurrentHealth = Mathf.RoundToInt(MaxHealth * healthPercent);
        }
        else
        {
            CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        }
        
        OnHealthChanged?.Invoke(0, CurrentHealth);
    }
    
    public void ResetHealth()
    {
        int healAmount = MaxHealth - CurrentHealth;
        CurrentHealth = MaxHealth;
        
        if (healAmount > 0)
        {
            OnHeal?.Invoke(healAmount);
            OnHealthChanged?.Invoke(healAmount, CurrentHealth);
        }
    }
    
    private void HandleDeath()
    {
        Debug.Log($"{gameObject.name} has died");
        OnDeath?.Invoke();
        
    }
}
