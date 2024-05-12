namespace Endless.Inventory.Items
{
    public interface IModifier
    {
        void AddValue(ref int baseValue);
    }
}
