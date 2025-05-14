using UnityEngine;
using UnityEngine.UI;
using System.Text;

namespace QuestSystem
{
    public class QuestLogUI : MonoBehaviour
    {
        public QuestLog questLog;
        public Text questText;

        void Start()
        {
            if (questLog != null)
            {
                questLog.OnQuestAdded += UpdateUI;
                questLog.OnQuestCompleted += UpdateUI;
            }
            UpdateUI(null);
        }

        void UpdateUI(Quest _)
        {
            if (questText == null || questLog == null) return;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<b>Active Quests:</b>");
            foreach (var quest in questLog.activeQuests)
            {
                sb.AppendLine($"- {quest.questName}: {quest.objectives.FindAll(o => !o.isCompleted).Count} objectives left");
            }
            sb.AppendLine("\n<b>Completed Quests:</b>");
            foreach (var quest in questLog.completedQuests)
            {
                sb.AppendLine($"- {quest.questName}");
            }
            questText.text = sb.ToString();
        }
    }
}
