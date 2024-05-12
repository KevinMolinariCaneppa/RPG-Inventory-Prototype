using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Endless.Inventory.Items
{
    [CreateAssetMenu(fileName = "Item Reflection", menuName = "Inventory System/Items/Item Reflection")]
    public class ItemReflection : ScriptableObject
    {
        public List<ItemObjectContainer> Items = new List<ItemObjectContainer>();
        [ContextMenu("BorrarBD")]
        public void BorrarBD()
        {
            ItemDataBase.itemObjects.Clear();
            ItemDataBase.itemObjectsIndex.Clear();
            ItemDataBase.Save();
        }


        [CreateAssetMenu(fileName = "Item Container", menuName = "Inventory System/Items/Item Container")]
        [Serializable]
        public class ItemObjectContainer : ScriptableObject
        {
            public int id = -1;
            public ItemObject item;

            private void OnValidate()
            {
                if(id >= 0)
                {
                    if(ItemDataBase.itemObjects.ContainsKey(id))
                        item = ItemDataBase.itemObjects[id];
                }
            }

            [ContextMenu("BorrarBD")]
            public void BorrarBD()
            {
                ItemDataBase.itemObjects.Clear();
                ItemDataBase.itemObjectsIndex.Clear();
                ItemDataBase.Save();
            }
        }
    }
}