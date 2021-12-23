using CryBits.Enums;

namespace CryBits.Entities
{
    public class ItemPotion : Item
    {
        public int PotionExperience { get; set; }
        public short[] PotionVital { get; set; } = new short[(byte)Vital.Count];
    }
}
