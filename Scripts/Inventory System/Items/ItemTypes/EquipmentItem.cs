using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Endless.Inventory.Items
{
    [CreateAssetMenu(fileName = "New Equipment Item", menuName = "Inventory System/Items/ItemTypes/Equipment Item")]
    public class EquipmentItem : ItemObject
    {
        public int BaseAttackPoints = 0;
        public int RealAttackPoints = 0;
        public int EnhancementLevel = 0;
        
        private void Awake()
        {
            type = ItemType.FirstItemSlot;
        }

        //| Level | Att | Def | Air | Fire | Earth | Water |
        //--------------------------------------------------
        //| 1     | 1   | 0   | 0   | 2    | 3     | 0     |
        //--------------------------------------------------
        //| 2     | 1   | 2   | 1   | 2    | 3     | 0     |
        //--------------------------------------------------
        //| 3     | 1   | 4   | 0   | 2    | 3     | 3     |
        //--------------------------------------------------
        //| 4     | 3   | 3   | 0   | 2    | 5     | 0     |
        //--------------------------------------------------*/

    }
}

