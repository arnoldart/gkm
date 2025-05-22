using UnityEngine;
using GKM.QuestSystem;

public class QuestStarter : MonoBehaviour
{
  	public Quest questToStart;

    void OnTriggerEnter(Collider other)
    {
		if (other.CompareTag("Player") && questToStart != null)
		{
			if(Input.GetKeyDown(KeyCode.F)) // Tekan E untuk memulai quest
			{
				QuestManager.Instance.AddQuest(questToStart);
				// Destroy(gameObject); // Hapus objek setelah quest dimulai
			}
		}
    }
}