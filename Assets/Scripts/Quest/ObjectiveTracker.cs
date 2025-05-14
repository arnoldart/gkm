using UnityEngine;

namespace QuestSystem
{
    public class ObjectiveTracker : MonoBehaviour
    {
        public QuestLog questLog;

        public string GetCurrentObjectiveText()
        {
            if (questLog == null || questLog.activeQuests.Count == 0)
                return "No active objectives.";
            var quest = questLog.activeQuests[0];
            foreach (var obj in quest.objectives)
            {
                if (!obj.isCompleted)
                    return obj.description;
            }
            return "All objectives completed.";
        }
    }
}
