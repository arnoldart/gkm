using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace QuestSystem
{
    public static class QuestSaveSystem
    {
        private static string savePath = Application.persistentDataPath + "/questsave.dat";

        public static void Save(QuestLog questLog)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                var data = new QuestLogData(questLog);
                formatter.Serialize(stream, data);
            }
        }

        public static void Load(QuestLog questLog)
        {
            if (File.Exists(savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream stream = new FileStream(savePath, FileMode.Open))
                {
                    QuestLogData data = (QuestLogData)formatter.Deserialize(stream);
                    data.ApplyTo(questLog);
                }
            }
        }
    }

    [System.Serializable]
    public class QuestLogData
    {
        public System.Collections.Generic.List<Quest> activeQuests;
        public System.Collections.Generic.List<Quest> completedQuests;

        public QuestLogData(QuestLog log)
        {
            activeQuests = log.activeQuests;
            completedQuests = log.completedQuests;
        }

        public void ApplyTo(QuestLog log)
        {
            log.activeQuests = activeQuests;
            log.completedQuests = completedQuests;
        }
    }
}
