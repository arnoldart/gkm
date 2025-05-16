using UnityEngine;

public class CollectItem : QuestStep
{
    private int itemCollected = 0;
    private int itemToComplete = 5;

    // void OnEnable()
    // {
    //     GameEventsManager.instance.miscEvents.onItemCollected += OnItemCollected;
    // }

    // void OnDisable()
    // {
    //     GameEventsManager.instance.miscEvents.onItemCollected -= OnItemCollected;
    // }

    private void OnItemCollected(Item item)
    {
        // if (item.itemType == Item.ItemType.Collectible)
        // {
        //     itemCollected++;
        //     Debug.Log($"Item collected: {itemCollected}/{itemToComplete}");

        //     if (itemCollected >= itemToComplete)
        //     {
        //         FinishQuestStep();
        //     }
        // }

        if (itemCollected < itemToComplete)
        {
            itemCollected++;
        }
        
        if(itemCollected >= itemToComplete)
        {
            FinishQuestStep();
        } 
    }
}
