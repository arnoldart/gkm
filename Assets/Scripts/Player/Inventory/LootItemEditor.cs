using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LootItem))]
public class LootItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LootItem lootItem = (LootItem)target;
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("item"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("quantity"));
        
        if (lootItem.item != null)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Item Properties", EditorStyles.boldLabel);
            
            if (lootItem.item.itemType == ItemType.Weapon)
            {
                EditorGUILayout.LabelField("Weapon Properties", EditorStyles.boldLabel);
                lootItem.damage = EditorGUILayout.IntField("Damage", lootItem.damage);
                lootItem.durability = EditorGUILayout.IntField("Durability", lootItem.durability);
                
                lootItem.item.damage = lootItem.damage;
                lootItem.item.durability = lootItem.durability;
            }
            else if (lootItem.item.itemType == ItemType.Consumable)
            {
                EditorGUILayout.LabelField("Consumable Properties", EditorStyles.boldLabel);
                lootItem.maxStack = EditorGUILayout.IntField("Max Stack", lootItem.maxStack);
                lootItem.healthRestored = EditorGUILayout.IntField("Health Restored", lootItem.healthRestored);
                
                lootItem.item.maxStack = lootItem.maxStack;
                lootItem.item.healthRestored = lootItem.healthRestored;
            }
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
