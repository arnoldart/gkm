using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{

    [SerializeField] private int healAmount = 20;
    [SerializeField] private HealthSystem healthSystem;

  	void Awake()
  	{
		// healthSystem = GetComponent<HealthSystem>();
		
		if (healthSystem == null)
		{
			Debug.LogError("HealthSystem tidak ditemukan pada player!");
		}
  	}

	public void HealPlayer()
    {
        if (healthSystem != null)
        {
            healthSystem.Heal(healAmount);
        }
    }
}
