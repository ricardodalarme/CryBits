namespace CryBits.Editors.Entities.Tools
{
    class CheckBox : Tool
    {
        public string Text { get; set; }
        public bool Checked { get; set; }

        public override string ToString() => "[CheckBox] " + Name;
    }
}
