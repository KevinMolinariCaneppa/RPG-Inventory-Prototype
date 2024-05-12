using UnityEngine;

namespace Endless.Inventory.Items
{

    [CreateAssetMenu(fileName = "New RarityList", menuName = "Inventory System/Items/RarityList")]
    [System.Serializable]
    public class RarityList : ScriptableObject
    {
        public Rarity[] rarityList;
    }
}
