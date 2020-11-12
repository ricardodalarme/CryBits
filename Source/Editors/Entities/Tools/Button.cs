namespace CryBits.Editors.Entities.Tools
{
    internal class Button : Tool
    {
        public byte Texture_Num { get; set; }

        public override string ToString() => "[Button] " + Name;
    }
}
