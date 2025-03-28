[System.Serializable]
public class InventoryItem
{
    public Item item;
    public int quantity;

    public InventoryItem(Item newItem, int newQuantity)
    {
        item = newItem;
        quantity = newQuantity;
    }
}