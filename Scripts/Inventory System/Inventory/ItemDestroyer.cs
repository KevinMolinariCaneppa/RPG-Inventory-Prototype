//using Endless.PlayerNameSpace;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Endless.Inventory.Items
{
    public class ItemDestroyer : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputQuantity;
        [SerializeField] private PlayerInventory inventory = null;
        [SerializeField] private TextMeshProUGUI confirmationText = null;
        private InventorySlot itemToDestroy;
        private int inputQuantityToStringToInt;

        public TMP_InputField inputField => inputQuantity;

        public bool inputEmpty => inputQuantity.text == "";

        private void Awake()
        {
            inventory = this.transform.parent.GetComponentInChildren<PlayerInventory>();
        }

        public void SetInitialText()
        {
            inputQuantity.text = "1";
        }
        public void GetQuantity()
        {
            int quantityParsed = int.Parse(inputQuantity.text);
            if (!inputEmpty)
            {
                inputQuantityToStringToInt = quantityParsed;

                if (inputQuantityToStringToInt > 0)
                {
                    ConfirmDestroy();
                    return;
                }
            }

            inputQuantity.gameObject.SetActive(false);
            return;
        }
        public void TextMaxSetter()
        {
            var itemInventorySlot = itemToDestroy;
            if (itemInventorySlot != null)
            {
                if (inventory.HasItem(itemInventorySlot.item))
                {
                    int quantityParsed = int.Parse(inputQuantity.text);
                    int quantityCount = inventory.GetItemAmount(itemInventorySlot.item);
                    if (!inputEmpty)
                    {
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
            }
        }

        public void SetItemSlot(InventorySlot itemSlot, PlayerInventory inventoryObject)
        {
            itemToDestroy = itemSlot;
            inventory = inventoryObject;
        }

        public void SetItemSlot(InventorySlot itemSlot)
        {
            itemToDestroy = itemSlot;
        }

        public void ConfirmDestroy()
        {
            if (ItemDataBase.itemObjects[itemToDestroy.item.Id].maxStack > 1)
            {
                confirmationText.text = $"¿Estas seguro de que quieres destruir: {inputQuantityToStringToInt}x {itemToDestroy.item.ColouredName}?";
                gameObject.SetActive(true);
            }
            else
            {
                confirmationText.text = $"¿Estas seguro de que quieres destruir: {itemToDestroy.item.ColouredName}?";
                gameObject.SetActive(true);
            }


        }

        public void Destroy()
        {
            inputQuantity.text = "";

            if (ItemDataBase.itemObjects[itemToDestroy.item.Id].maxStack == 1)
                inventory.RemoveAt(itemToDestroy.index);
            else
                inventory.RemoveItem(itemToDestroy.item, inputQuantityToStringToInt);

            inputQuantityToStringToInt = 0;
        }
    }
}
