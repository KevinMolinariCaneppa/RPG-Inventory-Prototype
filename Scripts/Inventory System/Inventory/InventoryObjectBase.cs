using System;
using System.Collections.Generic;
using System.Linq;
using Endless.Inventory.Items;
using UnityEngine;

namespace Endless.Inventory
{

    public class InventoryObjectBase : MonoBehaviour, IItemContainer, ISerializationCallbackReceiver
    {
        public RarityList rarities;
        public InventorySlot[] itemSlots = new InventorySlot[0];
        public int money = 0;


        [SerializeField] protected ItemObject itemToAddInEditor = null;
        [SerializeField] protected int amountToAddInEditor = 0;
        [SerializeField] protected DynamicInterface dynamicInterface;

        public int availableSlots
        {
            get
            {
                int availableItemSlots = 0;

                for (int i = 0; i < itemSlots.Length; i++)
                {
                    if (itemSlots[i].item.Id < 0)
                        availableItemSlots++;
                }

                return availableItemSlots;
            }
        }

        private void Awake()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i] == null)
                {
                    itemSlots[i] = new InventorySlot();
                    continue;
                }

                if (itemSlots[i].item.Id < 0)
                    itemSlots[i].amount = 0;
            }
        }

        public InventorySlot GetSlotByIndex(int index) => itemSlots[index];

        public int SlotsWithItem(ItemData itemSlot)
        {
            int occupiedSlotsWithItem = 0;
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].item.Id == itemSlot.Id)
                    occupiedSlotsWithItem++;
            }

            return occupiedSlotsWithItem;
        }

        public int GetSpaceRemainingInSlotWithItem(ItemData itemSlot)
        {
            return (SlotsWithItem(itemSlot) * ItemDataBase.itemObjects[itemSlot.Id].maxStack) - GetItemAmount(itemSlot);
        }

        [ContextMenu("AddItem")]
        public void AddItemByMenu()
        {
            ItemData _item = new ItemData(itemToAddInEditor);

            AddItem(_item, amountToAddInEditor);
        }

        public int AddItem(ItemData _item, int _amount)
        {
            int freeSlots = availableSlots;

            if (freeSlots <= 0)
            {
                return _amount;
            }

            ItemData item = _item;

            int amount = _amount;
            int thisItemMaxStack = ItemDataBase.itemObjects[item.Id].maxStack;
            int quantityToAddInSlot;

            do
            {
                if (freeSlots <= 0 || amount == 0)
                    return amount;
                

                InventorySlot slot = FindItemToAddOnInventory(item);

                if (slot == null)
                {
                    quantityToAddInSlot = amount;

                    if (quantityToAddInSlot > thisItemMaxStack)
                        quantityToAddInSlot = thisItemMaxStack;
                    

                    SetEmptySlot(item, quantityToAddInSlot);
                    amount -= quantityToAddInSlot;

                    continue;
                }
                else
                {
                    quantityToAddInSlot = thisItemMaxStack - slot.amount;

                    if (quantityToAddInSlot > amount)
                        quantityToAddInSlot = amount;

                    slot.AddAmount(quantityToAddInSlot);
                    amount -= quantityToAddInSlot;

                    continue;
                }

            } while (amount > 0);

            return amount;
        }

        public InventorySlot FindItemToAddOnInventory(ItemData _item)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if ((itemSlots[i].item.Id == _item.Id) && (itemSlots[i].amount < ItemDataBase.itemObjects[_item.Id].maxStack))
                    return itemSlots[i];
            }

            return null;
        }

        public int SlotsToUse(ItemData _item, int _amount)
        {
            int requiredSlots = (_amount % ItemDataBase.itemObjects[_item.Id].maxStack);

            if (requiredSlots != 0)
                requiredSlots++;

            if (requiredSlots > availableSlots)
                return availableSlots;

            return requiredSlots;
        }

        public int GetItemAmount(ItemObject _itemObject)
        {
            if (_itemObject == null) 
                return 0;
            int totalCount = 0;

            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i] == null) 
                    continue;
                if (itemSlots[i].item.Id < 0) 
                    continue; // slot is empty so it doesnt try to get quantity
                if (itemSlots[i].item.Id != _itemObject.data.Id) 
                    continue; // slot isnt the item required so it doesnt try to get quantity

                totalCount += itemSlots[i].amount;
            }

            return totalCount;
        }

        public int GetItemAmount(ItemData _item)
        {
            if (_item == null || _item.Id < 0)
                return 0;

            int totalCount = 0;

            for (int i = 0; i < itemSlots.Length; i++)
            {
                bool isTheSameItem = true;
                if (itemSlots[i] == null) 
                    continue;
                if (itemSlots[i].item.Id < 0) 
                    continue; // slot is empty so it doesnt try to get quantity
                if (itemSlots[i].item.Id != _item.Id) 
                    continue; // slot isnt the item required so it doesnt try to get quantity

                for (int j = 0; j < _item.buffs.Length; j++)
                {
                    if (_item.buffs[j].actualValue != itemSlots[i].item.buffs[j].actualValue)
                    {
                        isTheSameItem = false;
                        break;
                    }
                }

                if (isTheSameItem == true)
                    totalCount += itemSlots[i].amount;
            }

            return totalCount;
        }

        public bool HasItem(ItemData item)
        {
            foreach (InventorySlot itemSlot in itemSlots)
            {
                if (itemSlot.item == null) 
                    continue;
                if (itemSlot.item != item) 
                    continue;

                return true;
            }
            return false;
        }

        public void RemoveAt(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex > itemSlots.Length - 1) 
                return;

            itemSlots[slotIndex].RemoveItem();
        }

        public void RemoveItem(ItemData _item, int _amount)
        {
            if (_item == null) 
                return;
            if (_item.isUsableInfinitely == true) 
                return;

            int amountToRemove = _amount;
            for (int i = itemSlots.Length - 1; i > -1; i--)
            {
                InventorySlot slot = itemSlots[i];
                if (amountToRemove <= 0) 
                    return;
                if (slot == null) 
                    continue;
                if (slot.item.Id != _item.Id) 
                    continue;

                bool isTheSameItem = true;

                if (_item.buffs.Length != 0 && slot.item.buffs.Length == _item.buffs.Length)
                {
                    for (int k = 0; k < _item.buffs.Length; k++)
                    {
                        if(_item.buffs[k].attribute != slot.item.buffs[k].attribute) 
                        {
                            isTheSameItem = false;
                            break;
                        }

                        if (_item.buffs[k].actualValue != slot.item.buffs[k].actualValue)
                        {
                            isTheSameItem = false;
                            break;
                        }
                    }
                }

                if (isTheSameItem == true)
                {
                    if (slot.amount > amountToRemove)
                    {
                        slot.amount -= amountToRemove;
                        return;
                    }

                    amountToRemove -= slot.amount;
                    slot.RemoveItem();
                }
            }
        }

        public List<InventorySlot> GetAllUniqueItems()
        {
            List<InventorySlot> items = new List<InventorySlot>();

            for (int i = 0; i < itemSlots.Length; i++)
            {
                InventorySlot slot = itemSlots[i];

                if (slot == null || slot.item == null || slot.item.Id < 0) 
                    continue;

                bool repeated = false;

                if (items.Count == 0)
                {
                    items.Add(slot);
                    continue;
                }

                for (int j = 0; j < items.Count; j++)
                {
                    if (items[j].item.Id == slot.item.Id)
                    {
                        if (items[j].item.buffs.Length == 0 && slot.item.buffs.Length == 0)
                        {
                            repeated = true;
                        }
                    }
                }

                if (repeated) 
                    continue;

                items.Add(slot);
            }

            return items;
        }

        public void SwapItems(InventorySlot item1, InventorySlot item2)
        {
            if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))
            {
                InventorySlot temp = new InventorySlot(item2.item, item2.amount);   
                item2.UpdateSlot(item1.item, item1.amount);
                item1.UpdateSlot(temp.item, temp.amount);
            }
        }

        public void EmptyInventory()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                itemSlots[i].item = null;
                itemSlots[i].amount = 0;
            }
        }

        public void ClearMoney()
        {
            money = 0;
        }

        [ContextMenu("Sorter")]
        public void ByRaritySorter()
        {
            var itemSlotsSortedByRarity = new InventorySlot[itemSlots.Length];
            int freeSlotIndex = 0;

            itemSlots = itemSlots.OrderBy(itemSlots => itemSlots).ToArray();

            for (int i = 0; i < rarities.rarityList.Length; i++)
            {
                for (int j = 0; j < itemSlots.Length/2; j++)
                {
                    if (itemSlots[j].item.Id < 0 || itemSlots[j].item.rarity == null)
                        continue;
                    
                    if (itemSlots[j].item.rarity.name == rarities.rarityList[i].name)
                    {
                        itemSlotsSortedByRarity[freeSlotIndex] = itemSlots[j];
                        freeSlotIndex++;
                    }
                    if (itemSlots[itemSlots.Length-j-1].item.rarity.name == rarities.rarityList[i].name)
                    {
                        itemSlotsSortedByRarity[freeSlotIndex] = itemSlots[j];
                        freeSlotIndex++;
                    }
                }
            }
            itemSlots = itemSlotsSortedByRarity;
        }

        //set up functionality for full inventory
        public InventorySlot SetEmptySlot(ItemData _item, int _amount)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].item.Id < 0)
                {
                    itemSlots[i].UpdateSlot(_item, _amount);

                    return itemSlots[i];
                }
            }
            
            return null;
        }

        public int MaxOfItemFitIntoInventory(ItemData itemSlot)
        {
            return (availableSlots * ItemDataBase.itemObjects[itemSlot.Id].maxStack) + GetSpaceRemainingInSlotWithItem(itemSlot);
        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                if (itemSlots[i].amount == 0)
                    itemSlots[i].item.Id = -1;
            }
        }
    }

    public delegate void SlotUpdated(InventorySlot _slot);

    [System.Serializable]
    public class InventorySlot : IComparable<InventorySlot>
    {
        public ItemType[] AllowedItems = new ItemType[0];

        public InventoryUserInterface parent;
        //public PickUpUserInterface pickUpParent;
        [System.NonSerialized]
        public GameObject slotDisplay;
        [System.NonSerialized]
        public SlotUpdated OnAfterUpdate;
        [System.NonSerialized]
        public SlotUpdated OnBeforeUpdate;
        public ItemData item = new ItemData();
        public int index;
        public int amount;

        public ItemObject ItemObject
        {
            get
            {
                if (item.Id >= 0)
                {
                    if (item.ColouredName == "") 
                        return null;
                    if (parent != null)
                        return ItemDataBase.itemObjects[item.Id];

                    //if (parent == null && pickUpParent != null)
                    //{
                    //    return pickUpParent.inventory.ItemDataBase.ItemObjects[item.Id];
                    //}
                }
                return null;
            }
        }
        public void UpdateSlot(ItemData _item, int _amount)
        {
            if (OnBeforeUpdate != null)
                OnBeforeUpdate.Invoke(this);

            item = _item;
            amount = _amount;

            if (OnAfterUpdate != null)
                OnAfterUpdate.Invoke(this);
        }

        public InventorySlot()
        {
            UpdateSlot(new ItemData(), 0);
        }

        public InventorySlot(ItemData _item, int _amount)
        {
            UpdateSlot(_item, _amount);
        }

        public void RemoveItem()
        {
            UpdateSlot(new ItemData(), 0);
        }

        public void RemoveItem(ItemData _item, int _amount)
        {
            if (_item == null) 
                return;
            if (_item.isUsableInfinitely == true)
                return;
            if (_amount <= 0 || _amount > this.amount)
                return;
            if (item.buffs.Length != _item.buffs.Length)
                return;

            if (item.Id == _item.Id)
            {
                bool isTheSameItem = true;
                if (_item.buffs.Length > 0)
                {
                    for (int k = 0; k < _item.buffs.Length; k++)
                    {
                        if (_item.buffs[k].actualValue != item.buffs[k].actualValue)
                        {
                            isTheSameItem = false;
                            break;
                        }
                    }
                }


                if (isTheSameItem == true)
                {
                    if (this.amount <= _amount)
                        RemoveItem();
                    else
                        amount -= _amount;
                }
            }
        }

        public void AddAmount(int value)
        {
            UpdateSlot(item, amount += value);
        }

        public void SetItemAndAmount(ItemData _item, int _amount)
        {
            item = _item;
            amount = _amount;
        }

        public bool CanPlaceInSlot(ItemObject _itemObject)
        {
            if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0)
                return true;

            for (int i = 0; i < AllowedItems.Length; i++)
            {
                if (_itemObject.type == AllowedItems[i])
                    return true;
            }

            return false;
        }

        public int CompareTo(InventorySlot obj)
        {
            if (this.item == null || obj.item == null)
                return 0;

            if (this.item.itemName == "" || obj.item.itemName == "")
                return 0;

            return this.item.itemName.CompareTo(obj.item.itemName);
        }

    }
}