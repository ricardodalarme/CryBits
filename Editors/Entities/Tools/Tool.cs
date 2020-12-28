﻿using System.Drawing;
using System.Windows.Forms;
using CryBits.Enums;

namespace CryBits.Editors.Entities.Tools
{
    internal class Tool
    {
        // Árvore das ferramentas
        public static TreeNode Tree;

        // Informações gerais de todas as ferramentas
        public string Name { get; set; }
        public Point Position { get; set; }
        public bool Visible { get; set; }
        public Window Window { get; set; }
    }
}
