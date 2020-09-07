namespace Entities
{
    class Button : Tool
    {
        public byte Texture_Num { get; set; }

        public override string ToString() => "[Button] " + Name;
    }
}
