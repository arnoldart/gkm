using UnityEngine;
using System.Collections.Generic;

namespace QuestSystem
{
    public class CollectibleItem : MonoBehaviour
    {
        public string itemId;
        public string itemName;
        public string description;
    }

    public class CollectibleQuestHandler : MonoBehaviour
    {
        public Quest quest;
        public List<string> requiredItemIds;
        private HashSet<string> collectedItems = new HashSet<string>();

        public void Collect(string itemId)
        {
            if (requiredItemIds.Contains(itemId) && !collectedItems.Contains(itemId))
            {
                collectedItems.Add(itemId);
                var obj = quest.objectives.Find(o => o.description.Contains(itemId));
                if (obj != null) obj.isCompleted = true;
                if (collectedItems.Count == requiredItemIds.Count)
                {
                    var questLog = FindObjectOfType<QuestLog>();
                    if (questLog != null) questLog.CompleteQuest(quest);
                }
            }
        }
    }
}
