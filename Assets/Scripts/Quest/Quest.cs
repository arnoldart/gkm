using UnityEngine;

public class Quest
{
    public QuestInfoSO info;
    public QuestState state;

    private int currentQuestStepIndex;

    public Quest(QuestInfoSO info)
    {
        this.info = info;
        this.state = QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
    }

    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    public bool CurrentStepExists()
    {
        return currentQuestStepIndex < info.questStepPrefabs.Length;
    }

    public void instantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            Object.Instantiate<GameObject>(questStepPrefab, parentTransform);
        }
    }

    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("No more quest steps available.");
        }

        return questStepPrefab;
    }
}
