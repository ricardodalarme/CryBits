using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Editors.Logic;

internal static class Utils
{
    // Dimensão das grades 
    public static Size GridSize = new(Grid, Grid);

    public static Point Block_Position(byte direction)
    {
        // Retorna a posição de cada seta do bloqueio direcional
        switch ((Direction)direction)
        {
            case Direction.Up: return new Point(Grid / 2 - 4, 0);
            case Direction.Down: return new Point(Grid / 2 - 4, Grid - 9);
            case Direction.Left: return new Point(0, Grid / 2 - 4);
            case Direction.Right: return new Point(Grid - 9, Grid / 2 - 4);
            default: return new Point(0);
        }
    }

    public static void UpdateData(this ListBox list)
    {
        var lastSelected = list.SelectedItem;
        var lastSelectedIndex = list.SelectedIndex;

        // Limpa a lista e adiciona os itens 
        list.Items.Clear();
        foreach (var value in (IEnumerable)list.Tag) list.Items.Add(value);

        // Seleciona algum item de forma inteligente
        if (list.Items.Count > 0)
        {
            list.SelectedIndex = 0;

            if (lastSelected != null)
                if (list.Items.Contains(lastSelected))
                    list.SelectedItem = lastSelected;
                else if (lastSelectedIndex <= list.Items.Count)
                    list.SelectedIndex = Math.Max(0, lastSelectedIndex - 1);
        }
    }
}