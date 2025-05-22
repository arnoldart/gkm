using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;
using TMPro;
using System;

namespace GKM.QuestSystem
{
    public class QuestUI : MonoBehaviour
    {
        public TMP_Text questTitle;
        public TMP_Text questDescription;
        public TMP_Text questObjectives;

        private int currentQuestIndex = 0;


         void Update()
        {
            if (questTitle != null && QuestManager.Instance != null)
            {
                // Otomatis skip ke quest berikutnya jika quest saat ini sudah selesai
                var activeQuests = QuestManager.Instance.activeQuests;
                if (activeQuests.Count > 0)
                {
                    if (currentQuestIndex >= activeQuests.Count)
                        currentQuestIndex = activeQuests.Count - 1;
                    if (currentQuestIndex < 0)
                        currentQuestIndex = 0;
                    // Jika quest sudah selesai, pindah ke quest berikutnya
                    if (activeQuests[currentQuestIndex].IsCompleted && currentQuestIndex < activeQuests.Count - 1)
                        currentQuestIndex++;
                }
                questTitle.text = GetQuestLogString();
            }
        }

        private string GetQuestLogString()
        {
            var activeQuests = QuestManager.Instance.activeQuests;

            if (activeQuests.Count == 0)
                return null;

            if (currentQuestIndex >= activeQuests.Count)
                currentQuestIndex = activeQuests.Count - 1;
            if (currentQuestIndex < 0)
                currentQuestIndex = 0;

            var questInstance = activeQuests[currentQuestIndex];

            return questInstance.ToString();
        }
    }
}
