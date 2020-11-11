namespace CryBits.Editors.Entities
{
    class TextBox : Tool
    {
        public short Max_Characters { get; set; }
        public short Width { get; set; }
        public bool Password { get; set; }

        public override string ToString() => "[TextBox] " + Name;
    }
}
