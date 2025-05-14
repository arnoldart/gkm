using UnityEngine;
using UnityEngine.Events;

namespace QuestSystem
{
    public class QuestTrigger : MonoBehaviour
    {
        public Quest questToTrigger;
        public UnityEvent onTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var questLog = other.GetComponentInChildren<QuestLog>();
                if (questLog != null && questToTrigger != null)
                {
                    questLog.AddQuest(questToTrigger);
                    onTriggered?.Invoke();
                }
            }
        }
    }
}
