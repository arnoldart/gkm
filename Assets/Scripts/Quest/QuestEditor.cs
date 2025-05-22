using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GKM.QuestSystem
{
    [CustomEditor(typeof(Quest))]
    public class QuestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Quest quest = (Quest)target;

            quest.questID = EditorGUILayout.TextField("Quest ID", quest.questID);
            quest.title = EditorGUILayout.TextField("Title", quest.title);
            quest.description = EditorGUILayout.TextField("Description", quest.description);
            quest.questType = (QuestType)EditorGUILayout.EnumPopup("Quest Type", quest.questType);

            EditorGUILayout.LabelField("Objectives", EditorStyles.boldLabel);
            if (quest.objectives == null)
                quest.objectives = new List<QuestObjective>();

            for (int i = 0; i < quest.objectives.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                quest.objectives[i].objectiveType = (ObjectiveType)EditorGUILayout.EnumPopup("Objective Type", quest.objectives[i].objectiveType);
                quest.objectives[i].targetPrefab = (GameObject)EditorGUILayout.ObjectField("Target Prefab", quest.objectives[i].targetPrefab, typeof(GameObject), false);
                quest.objectives[i].requiredAmount = EditorGUILayout.IntField("Required Amount", quest.objectives[i].requiredAmount);
                if (GUILayout.Button("Remove Objective"))
                {
                    quest.objectives.RemoveAt(i);
                    break;
                }
                EditorGUILayout.EndVertical();
            }
            if (GUILayout.Button("Add Objective"))
            {
                quest.objectives.Add(new QuestObjective());
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Reward", EditorStyles.boldLabel);
            if (quest.reward == null)
                quest.reward = new QuestReward();
            quest.reward.xp = EditorGUILayout.IntField("XP", quest.reward.xp);
            SerializedObject so = new SerializedObject(quest);
            SerializedProperty itemsProp = so.FindProperty("reward.items");
            EditorGUILayout.PropertyField(itemsProp, true);
            SerializedProperty unlockProp = so.FindProperty("reward.unlockFeatures");
            EditorGUILayout.PropertyField(unlockProp, true);
            so.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(quest);
            }
        }
    }
}
