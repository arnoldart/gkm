using System.Collections.Generic;
using UnityEngine;

namespace GKM.QuestSystem
{
    public enum QuestType
    {
        Main,
        Side
    }

    public enum ObjectiveType
    {
        CollectItem,
        DefeatEnemy,
        GoLocation
    }

    [System.Serializable]
    public class QuestObjective
    {
        public ObjectiveType objectiveType;
        public GameObject targetPrefab; // Referensi prefab untuk objective
        // public string targetID; // ItemID, EnemyID, or LocationID (hapus)
        public int requiredAmount = 1;
        public int currentAmount = 0;
        public bool IsCompleted => currentAmount >= requiredAmount;
    }

  [CreateAssetMenu(fileName = "Quest", menuName = "Quest System/Quest", order = 1)]
  public class Quest : ScriptableObject
  {
      public string questID;
      public string title;
      public string description;
      public QuestType questType;
      public List<QuestObjective> objectives = new List<QuestObjective>();
      public QuestReward reward;
      public bool IsCompleted => objectives.TrueForAll(o => o.IsCompleted);

    void OnValidate()
    {
      questID = name;
    }

  }
}
