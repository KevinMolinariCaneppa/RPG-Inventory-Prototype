using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using Endless.Inventory.Tooltip;
using System.Linq;

namespace Endless.Inventory.Items
{
    [RequireComponent(typeof(PlayerInventory))]
    public abstract class InventoryUserInterface : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputQuantity;
        [SerializeField] private PlayerInventory playerInventory;
        private InventorySlot inventorySlotToSplitTo;
        private ItemDestroyer itemDestroyer;
        private HoverPopUpInfo hoverPopUpInfo;
        private bool usingMoneyAsText;
        public bool usingItem = false;
        public bool splitingItem = false;
        public bool splitDirectly;
        public Toggle sorterToggle = null;
        public GridLayoutGroup gridLayoutGroup;
        public InventorySlot itemInventorySlot;
        public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        public List<GameObject> objectsOnInterface;
        public List<GameObject> objectsToIgnore = new List<GameObject>();
        public GameObject itemMenu;
        public GameObject tempItem;
        public GameObject sorterObject;

        protected PlayerInventory Inventory => playerInventory;

        public InventorySlot InventorySlotToSplitTo 
        { 
            get => inventorySlotToSplitTo; 
            set => inventorySlotToSplitTo = value; 
        }

        public bool inputEmpty => inputQuantity.text == "";

        private void OnEnable()
        {
            //for (int i = 0; i < inventory.itemSlots.Length; i++)
            //{
            //    inventory.itemSlots[i].parent = this;
            //    

            //}

            if (sorterToggle != null)
            {
                if (sorterToggle.isOn == true)
                    sorterObject.SetActive(true);
            }
        }

        private void OnDisable()
        {
            if (sorterToggle != null)
                sorterObject.SetActive(false);

            if (MouseData.interfaceMouseIsOver = this)
                hoverPopUpInfo.HideInfo();
        }

        void Awake()
        {
            playerInventory = GetComponent<PlayerInventory>();
            objectsOnInterface = new List<GameObject>();

            for (int i = 0; i < Inventory.itemSlots.Length; i++)
            {
                Inventory.itemSlots[i].parent = this;
                Inventory.itemSlots[i].OnAfterUpdate += OnSlotUpdate;
            }

            CreateSlots();
            AddEvent(this.gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(this.gameObject); });
            AddEvent(this.gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(this.gameObject); });

            if (objectsToIgnore != null)
            {
                for (int i = 0; i < objectsToIgnore.Count; i++)
                {
                    if (objectsToIgnore[i] != null)
                    {
                        AddEvent(objectsToIgnore[i], EventTriggerType.PointerEnter, delegate { OnEnterInterface(this.gameObject); });
                        AddEvent(objectsToIgnore[i], EventTriggerType.PointerExit, delegate { OnExitInterface(this.gameObject); });
                    }
                }
            }

            itemDestroyer = FindAnyObjectByType<ItemDestroyer>();
            hoverPopUpInfo = FindAnyObjectByType<HoverPopUpInfo>();
        }

        private void Start()
        {
            for (int i = 0; i < playerInventory.itemSlots.Length; i++)
            {
                playerInventory.itemSlots[i].parent = this;
                playerInventory.itemSlots[i].OnAfterUpdate += OnSlotUpdate;
            }
        }


        public void SetInventorySlotIndex()
        {
            for (int i = 0; i < Inventory.itemSlots.Length; i++)
            {
                Inventory.itemSlots[i].index = i;
                Inventory.itemSlots[i].parent = this;
            }
        }
        private void OnSlotUpdate(InventorySlot _slot)
        {
            if (_slot.item.Id >= 0)
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.ItemObject.uiDisplay;
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
            }
            else
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }

        // Update is called once per frame
        //void Update()
        //{
        //    slotsOnInterface.UpdateSlotDisplay();
        //}
        public void ByRaritySorter()
        {
            InventorySlot[] inventorySlotsSorted = new InventorySlot[Inventory.itemSlots.Length];

            inventorySlotsSorted = Inventory.itemSlots.OrderBy(itemSlots => itemSlots).ToArray();
            int freePlacement = 0;
            for (int i = 0; i < Inventory.rarities.rarityList.Length; i++)
            {
                for (int j = 0; j < inventorySlotsSorted.Length; j++)
                {
                    if (inventorySlotsSorted[j].item.Id < 0 || inventorySlotsSorted[j].item.rarity == null)
                        continue;

                    if (inventorySlotsSorted[j].item.rarity.name == Inventory.rarities.rarityList[i].name)
                    {
                        inventorySlotsSorted[j].slotDisplay.transform.SetSiblingIndex(freePlacement);
                        freePlacement++;
                    }
                }
            }
        }

        public abstract void CreateSlots();

        protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            EventTrigger trigger = obj.GetComponent<EventTrigger>();
            var eventTrigger = new EventTrigger.Entry();
            eventTrigger.eventID = type;
            eventTrigger.callback.AddListener(action);
            trigger.triggers.Add(eventTrigger);
        }

        public void OnEnter(GameObject obj)
        {
            MouseData.slotHoveredOver = obj;
            hoverPopUpInfo.DisplayInfo(slotsOnInterface[obj].item);
        }

        public void OnExit(GameObject obj)
        {
            MouseData.slotHoveredOver = null;
            hoverPopUpInfo.HideInfo();
        }

        public void OnEnterInterface(GameObject obj)
        {
            MouseData.interfaceMouseIsOver = obj.GetComponent<InventoryUserInterface>();
        }

        public void OnExitInterface(GameObject obj)
        {
            MouseData.interfaceMouseIsOver = null;
        }

        public void OnDragStart(GameObject obj)
        {
            if (sorterToggle.isOn == true)
                return;

            if (slotsOnInterface[obj].item.Id >= 0)
            {
                obj.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 0.65f);
                MouseData.tempItemBeingDragged = CreateTempItem(obj);
            }
        }

        public GameObject CreateTempItem(GameObject obj)
        {
            if (slotsOnInterface[obj].item.Id >= 0)
            {
                var rt = tempItem.GetComponent<RectTransform>();
                rt.sizeDelta = new Vector2(gridLayoutGroup.cellSize.x, gridLayoutGroup.cellSize.y);
                var img = tempItem.GetComponent<Image>();
                img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
                img.raycastTarget = false;
                tempItem.SetActive(true);
            }

            return tempItem;
        }

        public void OnDragEnd(GameObject obj)
        {
            if (MouseData.slotHoveredOver != null)
                MouseData.interfaceMouseIsOver = MouseData.slotHoveredOver.GetComponentInParent<InventoryUserInterface>();

            if (sorterToggle.isOn == true)
                return;

            if (slotsOnInterface[obj].item.Id >= 0)
            {
                MouseData.tempItemBeingDragged.SetActive(false);

                obj.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1f, 1f, 1f, 1f);

                if (MouseData.interfaceMouseIsOver == null)
                {
                    itemDestroyer.SetItemSlot(slotsOnInterface[obj], playerInventory);
                    itemDestroyer.SetInitialText();
                    itemDestroyer.inputField.gameObject.SetActive(true);
                    return;
                }

                if (MouseData.slotHoveredOver && !splitingItem)
                {
                    InventorySlot mouseHoveredSlot = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
                    Inventory.SwapItems(slotsOnInterface[obj], mouseHoveredSlot);
                }
            }
        }

        public void OnDrag(GameObject obj)
        {
            if (sorterToggle.isOn == true)
                return;

            if (slotsOnInterface[obj].item.Id >= 0)
            {
                if (MouseData.tempItemBeingDragged != null && MouseData.tempItemBeingDragged.activeInHierarchy == true)
                    MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
            }
        }

        public void FinishUsingOrSplitting()
        {
            splitingItem = false;
            usingItem = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (itemDestroyer.inputField.gameObject.activeInHierarchy == true)
                    itemDestroyer.inputField.gameObject.SetActive(false);
            }
        }

        public void SetInitialText()
        {
            inputQuantity.text = "1";
        }

        public void GetQuantity()
        {
            if (!inputEmpty)
            {
                int amountToTakeFromSlot = int.Parse(inputQuantity.text);

                if (amountToTakeFromSlot > 0)
                {
                    if (usingItem)
                        MultipleUse(amountToTakeFromSlot);
                    else if (splitingItem)
                        Split(amountToTakeFromSlot);

                    return;
                }
            }

            inputQuantity.gameObject.SetActive(false);
            return;
        }

        public void TextMaxSetter()
        {
            if (inputEmpty)
            {
                inputQuantity.text = "1";
                return;
            }

            int quantityParsed = int.Parse(inputQuantity.text);

            // money

            if (usingMoneyAsText)
            {
                if (quantityParsed > playerInventory.money)
                {
                    inputQuantity.text = playerInventory.money.ToString();
                    return;
                }

                if (quantityParsed < 1f)
                {
                    inputQuantity.text = "1";
                    return;
                }

                return;
            }

            // item

            if (itemInventorySlot == null)
                return;

            if (playerInventory.HasItem(itemInventorySlot.item))
            {
                int quantityCount = 0;
                if (splitingItem)
                    quantityCount = itemInventorySlot.amount;
                else
                    quantityCount = playerInventory.GetItemAmount(itemInventorySlot.item);


                if (quantityParsed > quantityCount)
                {
                    inputQuantity.text = quantityCount.ToString();
                    return;
                }

                if (quantityParsed < 1)
                {
                    inputQuantity.text = "1";
                    return;
                }
            }
        }

        public void TextPercentSetter(int Percent)
        {
            if (inputEmpty)
            {
                inputQuantity.text = "1";
                return;
            }

            int quantityParsed = int.Parse(inputQuantity.text);

            if (usingMoneyAsText)
            {
                inputQuantity.text = ((playerInventory.money / 100) * Percent).ToString();
                if (quantityParsed > playerInventory.money)
                {
                    inputQuantity.text = playerInventory.money.ToString();
                    return;
                }

                if (quantityParsed < 1f)
                {
                    inputQuantity.text = "1";
                    return;
                }

                return;
            }

            if (itemInventorySlot == null)
                return;

            if (playerInventory.HasItem(itemInventorySlot.item))
            {
                inputQuantity.text = ((playerInventory.GetItemAmount(itemInventorySlot.item) / 100) * Percent).ToString();
                int quantityCount = playerInventory.GetItemAmount(itemInventorySlot.item);

                if (quantityParsed > quantityCount)
                {
                    inputQuantity.text = quantityCount.ToString();
                    return;
                }

                if (quantityParsed < 1f)
                {
                    inputQuantity.text = "1";
                    return;
                }
            }
        }

        public void Splitting()
        {
            splitingItem = true;
            usingItem = false;
            inputQuantity.transform.parent.gameObject.SetActive(true);
        }
        public void MultipleUse()
        {
            splitingItem = false;
            usingItem = true;
            inputQuantity.transform.parent.gameObject.SetActive(true);
        }

        public void Split(int amountToTakeFromSlot)
        {
            bool splitDone = false;
            if (InventorySlotToSplitTo.item.Id >= 0)
            {
                if (inventorySlotToSplitTo.amount + amountToTakeFromSlot > inventorySlotToSplitTo.ItemObject.maxStack)
                    amountToTakeFromSlot = inventorySlotToSplitTo.ItemObject.maxStack - amountToTakeFromSlot;
            }

            if (inventorySlotToSplitTo.item == itemInventorySlot.item && !splitDone)
            {
                inventorySlotToSplitTo.amount += amountToTakeFromSlot;
                itemInventorySlot.amount -= amountToTakeFromSlot;
                splitDone = true;
            }

            if (inventorySlotToSplitTo.item.Id < 0 && !splitDone)
            {
                inventorySlotToSplitTo.UpdateSlot(itemInventorySlot.item, amountToTakeFromSlot);
                itemInventorySlot.amount -= amountToTakeFromSlot;
                splitDone = true;
            }

            if (itemInventorySlot.amount <= 0 && splitDone)
                itemInventorySlot.RemoveItem();

        }

        public void Use()
        {
            itemInventorySlot.ItemObject.Use();
        }

        public void MultipleUse(int amountToTakeFromSlot)
        {
            for (int i = 0; i < amountToTakeFromSlot; i++)
            {
                if (itemInventorySlot.amount >= amountToTakeFromSlot)
                    itemInventorySlot.ItemObject.Use();
            }
        }
        public void UseStack()
        {
            int toUse = itemInventorySlot.amount;
            for (int i = 0; i < toUse; i++)
            {
                itemInventorySlot.ItemObject.Use();
            }
        }
    }

    public static class MouseData
    {
        public static InventoryUserInterface interfaceMouseIsOver;
        //public static PickUpUserInterface pickUpInterfaceMouseIsOver;
        //public static HotbarUserInterface hotbarUserInterfaceMouseIsOver;
        public static GameObject tempItemBeingDragged;
        public static GameObject slotHoveredOver;
    }


    public static class ExtensionMethods
    {
        public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
        {
            foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
            {
                if (_slot.Value.item != null && _slot.Value.item.Id >= 0)
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.uiDisplay;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
                }
                else
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            }
        }
    }
}

