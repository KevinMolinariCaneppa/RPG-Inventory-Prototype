using Endless.Inventory.Items;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Endless.Inventory.Tooltip
{
    public class HoverPopUpInfo : MonoBehaviour
    {
        [SerializeField] private GameObject popupCanvasObject, itemBuffsHolder;
        [SerializeField] private RectTransform popupObject;
        [SerializeField] private TextMeshProUGUI itemName, itemBuffs, itemDescription, onUse;
        [SerializeField] private Vector3 offset = new Vector3(0f, 50f, 0f);
        [SerializeField] private float padding = 25f;
        [SerializeField] private Image itemIcon = null;
        private Canvas popupCanvas = null;
        public TextMeshProUGUI itemSellPrice;

        private void Awake()
        {
            popupCanvas = popupCanvasObject.GetComponent<Canvas>();
        }

        private void OnDisable()
        {
            itemBuffsHolder.SetActive(false);
            onUse.gameObject.SetActive(false);
        }

        private void Update()
        {
            FollowCursor();
        }

        private void FollowCursor()
        {
            if (!popupCanvasObject.activeSelf) 
                return;

            //calcular la posicion deseada
            Vector3 newPos = Input.mousePosition + offset;
            newPos.z = 0f;


            float leftEdgeToScreenEdgeDistance = 0 - (newPos.x - popupObject.rect.width * popupCanvas.scaleFactor / 0.9825f) + padding;
            if (leftEdgeToScreenEdgeDistance > 0)
                newPos.x += leftEdgeToScreenEdgeDistance;

            float rightEdgeToScreenEdgeDistance = Screen.width - (newPos.x + popupObject.rect.width * popupCanvas.scaleFactor / 64) - padding;
            if (rightEdgeToScreenEdgeDistance < 0)
                newPos.x += rightEdgeToScreenEdgeDistance;

            float topEdgeToScreenEdgeDistance = Screen.height - (newPos.y + popupObject.rect.height * popupCanvas.scaleFactor / 32) - padding;
            if (topEdgeToScreenEdgeDistance < 0)
                newPos.y += topEdgeToScreenEdgeDistance;

            float bottomEdgeToScreenEdgeDistance = 0 - (newPos.y - popupObject.rect.height * popupCanvas.scaleFactor / 0.9825f) + padding;
            if (bottomEdgeToScreenEdgeDistance > 0)
                newPos.y += bottomEdgeToScreenEdgeDistance;

            popupObject.transform.position = newPos;
        }

        public void DisplayInfo(ItemData infoItem)
        {
            if (infoItem == null)
                return;
            if (infoItem.Id < 0)
                return;

            itemIcon.sprite = ItemDataBase.itemObjects[infoItem.Id].uiDisplay;

            StringBuilder builderName = new StringBuilder();
            StringBuilder builderBuffs = new StringBuilder();
            StringBuilder builderUse = new StringBuilder();
            StringBuilder builderDescription = new StringBuilder();
            StringBuilder builderPrice = new StringBuilder();

            builderName.Append(ItemDataBase.itemObjects[infoItem.Id].data.ColouredName).AppendLine();
            builderDescription.Append(ItemDataBase.itemObjects[infoItem.Id].description).AppendLine();
            builderUse.Append(ItemDataBase.itemObjects[infoItem.Id].useText).AppendLine();

            if (infoItem.buffs.Length > 0)
            {
                itemBuffsHolder.SetActive(true);
                for (int i = 0; i < infoItem.buffs.Length; i++)
                {
                    if (infoItem.buffs[i].actualValue != 0)
                        builderBuffs.Append(infoItem.buffs[i].actualValue + " " + infoItem.buffs[i].attribute).AppendLine();
                }
            }
            else
                itemBuffsHolder.SetActive(false);



            builderPrice.Append("Precio de Venta: " + ItemDataBase.itemObjects[infoItem.Id].sellPrice + " <sprite=0>");

            itemName.text = builderName.ToString();
            itemBuffs.text = builderBuffs.ToString();
            itemDescription.text = builderDescription.ToString();
            itemSellPrice.text = builderPrice.ToString();

            if (ItemDataBase.itemObjects[infoItem.Id].useText != "")
            {
                onUse.text = builderUse.ToString();
                onUse.gameObject.SetActive(true);
            }
            else
                onUse.gameObject.SetActive(false);

            popupCanvasObject.SetActive(true);

            LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
        }

        public void HideInfo()
        {
            if (popupCanvas)
                popupCanvasObject.SetActive(false);
        }
    }
}


