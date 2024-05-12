using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Endless.Inventory.Items
{
    [CreateAssetMenu(fileName = "New Trash Item", menuName = "Inventory System/Items/ItemTypes/Trash Item")]
    public class TrashItem : ItemObject
    {
        private void Awake()
        {
            type = ItemType.Trash;
        }
    }
}
