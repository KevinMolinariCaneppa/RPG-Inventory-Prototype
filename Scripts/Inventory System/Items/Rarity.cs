using UnityEngine;

namespace Endless.Inventory.Items
{

    [CreateAssetMenu(fileName = "New Rarity", menuName = "Inventory System/Items/Rarity")]
    [System.Serializable]
    public class Rarity : ScriptableObject
    {
        public new string name = "New Rarity Name";
        public Color textColour = new Color(1f, 1f, 1f, 1f);
    }
}
