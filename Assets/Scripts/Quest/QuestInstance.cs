using System.Collections.Generic;
using UnityEngine;

namespace GKM.QuestSystem
{
    [System.Serializable]
    public class QuestObjectiveInstance
    {
        public QuestObjective template;
        public int currentAmount;
        public bool IsCompleted => currentAmount >= template.requiredAmount;
    }

    // [System.Serializable]
    public class QuestInstance
    {
        public Quest template;
        public List<QuestObjectiveInstance> objectives = new List<QuestObjectiveInstance>();
        public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);

        public QuestInstance(Quest questTemplate)
        {
            template = questTemplate;
            foreach (var obj in questTemplate.objectives)
            {
                objectives.Add(new QuestObjectiveInstance { template = obj, currentAmount = 0 });
            }
        }
    }
}
