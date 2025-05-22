using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GKM.QuestSystem
{
    public class QuestManager : MonoBehaviour
    {
        public static QuestManager Instance;
        public List<QuestInstance> activeQuests = new List<QuestInstance>();

        public List<QuestInstance> completedQuests = new List<QuestInstance>();

        [Header("Auto Start Main Quest")]
        public List<Quest> autoStartMainQuests = new List<Quest>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            // Otomatis trigger main quest di awal game
            foreach (var quest in autoStartMainQuests)
            {
                if (quest.questType == QuestType.Main)
                    AddQuest(quest);
            }
        }

        public void AddQuest(Quest quest)
        {
            if (activeQuests.Any(q => q.template == quest) || completedQuests.Any(q => q.template == quest))
                return;
            activeQuests.Add(new QuestInstance(quest));
        }

        public void CompleteObjective(QuestInstance questInstance, GameObject targetPrefab, int amount = 1)
        {
            foreach (var obj in questInstance.objectives)
            {
                if (obj.template.targetPrefab == targetPrefab && !obj.IsCompleted)
                {
                    obj.currentAmount = Mathf.Min(obj.currentAmount + amount, obj.template.requiredAmount);
                    if (questInstance.IsCompleted)
                        CompleteQuest(questInstance);
                    break;
                }
            }
        }

        public void CompleteQuest(QuestInstance questInstance)
        {
            if (activeQuests.Contains(questInstance))
            {
                activeQuests.Remove(questInstance);
                completedQuests.Add(questInstance);
                // TODO: Give rewards, notify UI, etc.
            }
        }
    }
}
