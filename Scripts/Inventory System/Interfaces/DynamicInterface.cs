using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Endless.Inventory.Items
{
    public class DynamicInterface : InventoryUserInterface
    {
        public GameObject buttonPrefab;
        public TMP_InputField itemSearch;
        [SerializeField] private Transform buttonHolderTransform = null;
        private static PointerEventData pointerEventData = new PointerEventData(EventSystem.current);



        public override void CreateSlots()
        {
            slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
            for (int i = 0; i < Inventory.itemSlots.Length; i++)
            {
                if (Inventory.itemSlots[i].slotDisplay == null)
                {
                    GameObject buttonInstance = Instantiate(buttonPrefab, buttonHolderTransform);

                    AddEvent(buttonInstance, EventTriggerType.PointerEnter, delegate { OnEnter(buttonInstance); });
                    AddEvent(buttonInstance, EventTriggerType.PointerExit, delegate { OnExit(buttonInstance); });
                    AddEvent(buttonInstance, EventTriggerType.BeginDrag, delegate { OnDragStart(buttonInstance); });
                    AddEvent(buttonInstance, EventTriggerType.EndDrag, delegate { OnDragEnd(buttonInstance); });
                    AddEvent(buttonInstance, EventTriggerType.Drag, delegate { OnDrag(buttonInstance); });
                    Inventory.itemSlots[i].slotDisplay = buttonInstance;

                    slotsOnInterface.Add(buttonInstance, Inventory.itemSlots[i]);

                    objectsOnInterface.Add(buttonInstance);
                }
            }

            slotsOnInterface.UpdateSlotDisplay();
        }

        public void SearchItem()
        {
            var ItemSearchText = itemSearch.text.ToLower();
            if (ItemSearchText == null) 
                return;

            foreach (KeyValuePair<GameObject, InventorySlot> _slot in slotsOnInterface)
            {
                if (ItemSearchText == "") 
                { 
                    _slot.Key.SetActive(true); 
                    continue; 
                }

                if (_slot.Value.item.itemName.ToLower().Contains(ItemSearchText))
                    _slot.Key.SetActive(true);
                else
                    _slot.Key.SetActive(false);
            }
        }
    }
}
