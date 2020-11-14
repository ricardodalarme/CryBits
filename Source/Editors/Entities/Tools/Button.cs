namespace CryBits.Editors.Entities.Tools
{
    internal class Button : Tool
    {
        public byte TextureNum { get; set; }

        public override string ToString() => "[Button] " + Name;
    }
}
