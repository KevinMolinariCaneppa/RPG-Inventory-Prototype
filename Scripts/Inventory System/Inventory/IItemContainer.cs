using System.Collections.Generic;
using TMPro;

namespace Endless.Inventory.Items
{
    public interface IItemContainer
    {
        void RemoveItem(ItemData _item, int _amount);
        void RemoveAt(int slotIndex);
        void ByRaritySorter();
        void EmptyInventory();
        //void Swap(int indexOne, int indexTwo, TextMeshProUGUI inputTextField);
        bool HasItem(ItemData item);
        int AddItem(ItemData _item, int _amount);
        int SlotsToUse(ItemData _item, int _amount);
        int GetItemAmount(ItemObject _itemObject);
        int GetItemAmount(ItemData item);
        int SlotsWithItem(ItemData itemSlot);
        int GetSpaceRemainingInSlotWithItem(ItemData itemSlot);
        int MaxOfItemFitIntoInventory(ItemData itemSlot);
        List<InventorySlot> GetAllUniqueItems();
        InventorySlot GetSlotByIndex(int index);
        InventorySlot SetEmptySlot(ItemData _item, int _amount);
    }
}
