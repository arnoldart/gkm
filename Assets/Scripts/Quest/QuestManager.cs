using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap;

    private void Awake()
    {
        questMap = CreateQuestMap();



        Quest quest = GetQuestById("GetWeapon");
        Debug.Log(quest.info.displayName);
        Debug.Log(quest.state);
        Debug.Log(quest.CurrentStepExists());
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quest");

        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();

        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning($"Duplicate quest ID found: {questInfo.id}. Skipping quest.");
                continue;
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogWarning($"Quest with ID {id} not found.");
        }

        return quest;
    }
}
