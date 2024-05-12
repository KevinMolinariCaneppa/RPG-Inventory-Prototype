using Endless.Inventory.Items;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DatabaseReflect", menuName = "Inventory System/Items/DatabaseReflect")]
public class ItemDatabaseReflectionObject : ScriptableObject
{
    public Dictionary<int, ItemObject> itemObjects = new Dictionary<int, ItemObject>();

    public List<ItemObject> items = new List<ItemObject>();

    [ContextMenu("Reflect")]
    void ReflectDatabase()
    {
        itemObjects = ItemDataBase.Reflect();

        items.Clear();

        for (int i = 0; i < itemObjects.Count; i++)
        {
            items.Add(itemObjects[i]);
        }
    }

    public ItemDatabaseReflectionObject(Dictionary<int, ItemObject> itemObjects)
    {
        this.itemObjects = itemObjects;
    }
}