using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Entities
{
    public class ItemEquipment : Item
    {
        // Equipamento
        public byte EquipType { get; set; }
        public short[] EquipAttribute { get; set; } = new short[(byte)Attribute.Count];
        public short WeaponDamage { get; set; }
    }
}
