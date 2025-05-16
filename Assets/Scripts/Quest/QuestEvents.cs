using System;
using UnityEngine;

public class QuestEvents
{
    public event Action<string> onStartQuest;

    public void StartQuest(string questId)
    {
        if (onStartQuest != null)
        {
            onStartQuest(questId);
        }
    }

    public event Action<string> onAdvanceQuest;

    public void AdvanceQuest(string questId)
    {
        if (onAdvanceQuest != null)
        {
            onAdvanceQuest(questId);
        }
    }
    
    public event Action<string> onFinishQuest;

    public void FinishQuest(string questId)
    {
        if (onFinishQuest != null)
        {
            onFinishQuest(questId);
        }
    }

    public event Action<Quest> onQuestStateChange;

    public void QuestStateChange(Quest quest)
    {
        if (onQuestStateChange != null)
        {
            onQuestStateChange(quest);
        }
    }
}
