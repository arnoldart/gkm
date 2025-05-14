using System.Collections.Generic;
using UnityEngine;

namespace QuestSystem
{
    [CreateAssetMenu(fileName = "QuestDatabase", menuName = "Quest/Database")]
    public class QuestDatabase : ScriptableObject
    {
        public List<Quest> quests;
    }
}
