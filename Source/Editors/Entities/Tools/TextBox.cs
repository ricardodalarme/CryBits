namespace CryBits.Editors.Entities.Tools
{
    internal class TextBox : Tool
    {
        public short MaxCharacters { get; set; }
        public short Width { get; set; }
        public bool Password { get; set; }

        public override string ToString() => "[TextBox] " + Name;
    }
}
