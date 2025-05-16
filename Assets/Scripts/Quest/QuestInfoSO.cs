using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfo", menuName = "ScriptableObjects/QuestInfo", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;

    [Header("Requirements")]
    // public int levelRequirement;
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

     

    void OnValidate()
    {
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
  }
}
