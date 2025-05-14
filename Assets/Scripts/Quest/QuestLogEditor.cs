using UnityEditor;
using UnityEngine;

namespace QuestSystem
{
    [CustomEditor(typeof(QuestLog))]
    public class QuestLogEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            QuestLog log = (QuestLog)target;
            GUILayout.Space(10);
            GUILayout.Label("Active Quests", EditorStyles.boldLabel);
            foreach (var quest in log.activeQuests)
            {
                GUILayout.Label($"{quest.questName} - {quest.status}");
                foreach (var obj in quest.objectives)
                {
                    GUILayout.Label($"  - {obj.description}: {(obj.isCompleted ? "Done" : "Pending")}");
                }
            }
            GUILayout.Space(10);
            GUILayout.Label("Completed Quests", EditorStyles.boldLabel);
            foreach (var quest in log.completedQuests)
            {
                GUILayout.Label($"{quest.questName} - {quest.status}");
            }
        }
    }
}
