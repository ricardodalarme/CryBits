using System.Drawing;
using static Logic.Utils;

namespace Entities
{
    class Tool
    {
        // Informações gerais de todas as ferramentas
        public string Name { get; set; }
        public Point Position { get; set; }
        public bool Visible { get; set; }
        public Windows Window { get; set; }
    }

    class Button : Tool
    {
        public byte Texture_Num { get; set; }

        public override string ToString() => "[Button] " + Name;
    }

    class TextBox : Tool
    {
        public short Max_Characters { get; set; }
        public short Width { get; set; }
        public bool Password { get; set; }

        public override string ToString() => "[TextBox] " + Name;
    }

    class CheckBox : Tool
    {
        public string Text { get; set; }
        public bool Checked { get; set; }

        public override string ToString() => "[CheckBox] " + Name;
    }

    class Panel : Tool
    {
        public byte Texture_Num { get; set; }

        public override string ToString() => "[Panel] " + Name;
    }
}
