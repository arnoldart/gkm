using System;
using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    public enum QuestType { Main, Collectible, TriggerEvent }
    public enum QuestStatus { Inactive, Active, Completed }

    [Serializable]
    public class QuestObjective
    {
        public string description;
        public bool isCompleted;
    }

    [Serializable]
    public class Quest
    {
        public string questName;
        public QuestType type;
        public QuestStatus status;
        public List<QuestObjective> objectives = new List<QuestObjective>();
        public string summary;
        public string reward;
    }
}
