using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public enum ItemType
{
    FirstItemSlot,
    SecondItemSlot,
    ThirdItemSlot,
    Helmet,
    ShoulderPlate,
    ChestPlate,
    Cape,
    Gloves,
    Belt,
    Pants,
    Boots,
    Earring,
    Ring,
    Necklace,
    Consumable,
    Miscellaneous,
    Trash,
    Money
}
namespace Endless.Inventory.Items
{

    public enum Attributes
    {
        Agility,
        Intellect,
        Stamina,
        Strength
    }

    public abstract class ItemObject : ScriptableObject
    {
        [Header("Item Data")]
        public Sprite uiDisplay = null;
        public GameObject characterDisplay;
        public Rarity rarity;
        public ItemType type;
        public string itemName;
        public string useText = "";

        [TextArea(15, 20)]
        public string description = "New Item Description";
        public ItemData data = new ItemData();

        [Min(0)] public int sellPrice = 0;
        [Min(0)] public int buyPrice = 1;
        [Min(1)] public int maxStack = 1;

        public List<string> boneNames = new List<string>();

        public Sprite UiDisplay => uiDisplay;

        private void OnValidate()
        {
            if(this is ItemObject and not null)
            {
                if (ItemDataBase.itemObjectsIndex.ContainsKey(this))
                {
                    if ((this.data.Id == -1))
                    {
                        this.data.Id = ItemDataBase.itemObjectsIndex[this];
                        ItemDataBase.  UpdateItem(this);
                    }
                    else
                    {
                        if (ItemDataBase.itemObjects[ItemDataBase.itemObjectsIndex[this]].name == this.name)
                            ItemDataBase.UpdateItem(this);
                        else
                            ItemDataBase.AddItem(this);
                    }
                }
                else
                    ItemDataBase.AddItem(this);
            }

            boneNames.Clear();
            if (characterDisplay == null)
                return;
            if (!characterDisplay.GetComponent<SkinnedMeshRenderer>())
                return;

            var renderer = characterDisplay.GetComponent<SkinnedMeshRenderer>();
            var bones = renderer.bones;

            foreach (var t in bones)
            {
                boneNames.Add(t.name);
            }

        }

        public string Name => itemName;

        public ItemData CreateItem()
        {
            ItemData newItem = new ItemData(this);
            return newItem;
        }
        public void Use() { }
    }

    [Serializable]
    public class ItemData : IComparable<ItemData>
    {
        [Header("Basic Info")]
        public int Id = -1;
        [NonSerialized]
        public Rarity rarity;
        [NonSerialized]
        public string itemName;
        [NonSerialized]
        public string useText = "";
        [NonSerialized]
        public Sprite uiDisplay = null;
        public ItemStats[] buffs;

        public bool canBeBoughtInfinitely = false;
        public bool isUsableInfinitely = false;

        public ItemData()
        {
            this.itemName = "";
            this.Id = -1;
        }

        public ItemData(ItemObject item)
        {
            itemName = item.itemName;
            uiDisplay = item.UiDisplay;
            rarity = item.rarity;
            useText = item.useText;

            buffs = new ItemStats[item.data.buffs.Length];

            for (int i = 0; i < buffs.Length; i++)
            {
                buffs[i] = new ItemStats(item.data.buffs[i].min, item.data.buffs[i].max)
                {
                    attribute = item.data.buffs[i].attribute
                };
            }
        }
        public string ColouredName
        {
            get
            {
                if (this.rarity == null) 
                    return itemName;

                string hexColour = ColorUtility.ToHtmlStringRGB(rarity.textColour);
                return $"<color=#{hexColour}>{itemName}</color>";
            }
        }

        public int CompareTo(ItemData obj)
        {
            if (obj == null || this == null) 
                return 0;
            if (this.itemName == "" || obj.itemName == "") 
                return 0;
            return this.itemName.CompareTo(obj.itemName);
        }

    }

    [System.Serializable]
    public class ItemStats : IModifier
    {
        public Attributes attribute;
        public int actualValue;
        public int min;
        public int max;
        public ItemStats(int _min, int _max)
        {
            min = _min;
            max = _max;
            GenerateValue();
        }

        public void AddValue(ref int baseValue)
        {
            baseValue += actualValue;
        }


        public void GenerateValue()
        {
            actualValue = UnityEngine.Random.Range(min, max);
        }
    }
}