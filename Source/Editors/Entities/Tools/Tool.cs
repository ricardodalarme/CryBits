using Logic;
using System.Drawing;

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
}
