using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    public class QuestLog : MonoBehaviour
    {
        public List<Quest> activeQuests = new List<Quest>();
        public List<Quest> completedQuests = new List<Quest>();

        public event Action<Quest> OnQuestAdded;
        public event Action<Quest> OnQuestCompleted;

        public void AddQuest(Quest quest)
        {
            if (!activeQuests.Contains(quest) && !completedQuests.Contains(quest))
            {
                quest.status = QuestStatus.Active;
                activeQuests.Add(quest);
                OnQuestAdded?.Invoke(quest);
            }
        }

        public void CompleteQuest(Quest quest)
        {
            if (activeQuests.Contains(quest))
            {
                quest.status = QuestStatus.Completed;
                activeQuests.Remove(quest);
                completedQuests.Add(quest);
                OnQuestCompleted?.Invoke(quest);
            }
        }
    }
}
