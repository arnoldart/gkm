using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;

    // Weapon Properties
    public int damage;
    public int durability;

    // Consumable Properties
    public int maxStack;
    public int healthRestored;
}