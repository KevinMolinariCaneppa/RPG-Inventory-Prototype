using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using Endless.Inventory.Items;


namespace Endless.Inventory
{

    public class PlayerInventory : InventoryObjectBase
    {
        public string savePath;
        [SerializeField] private int maxInventorySlots = 270;

        public void AddSlots(int slotsToBeAdded)
        {
            int _slotsToBeAdded = slotsToBeAdded;
            var thisUserInterface = itemSlots[0].parent;

            if (itemSlots.Length + _slotsToBeAdded > maxInventorySlots)
                return;

            InventorySlot[] inventorySlots2 = new InventorySlot[itemSlots.Length + _slotsToBeAdded];

            for (int i = 0; i < itemSlots.Length; i++)
            {
                inventorySlots2[i] = itemSlots[i];
            }

            itemSlots = inventorySlots2;
            thisUserInterface.CreateSlots();
        }

        [ContextMenu("Save")]
        public void Save()
        {
            string saveData = JsonUtility.ToJson(this, true);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
            binaryFormatter.Serialize(file, saveData);
            file.Close();
        }
        [ContextMenu("Load")]
        public void Load()
        {
            if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
            {
#if UNITY_EDITOR
                rarities = (RarityList)AssetDatabase.LoadAssetAtPath("Assets/Resources/Items/Rarity/RarityList.asset", typeof(RarityList));
#else
                rarities = Resources.Load<RarityList>("Items/Rarity/RarityList");
#endif
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
                JsonUtility.FromJsonOverwrite(binaryFormatter.Deserialize(file).ToString(), this);
                file.Close();
            }
        }
    }
}