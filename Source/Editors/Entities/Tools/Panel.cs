namespace CryBits.Editors.Entities.Tools
{
    internal class Panel : Tool
    {
        public byte TextureNum { get; set; }

        public override string ToString() => "[Panel] " + Name;
    }
}
