using System;
using System.Windows.Forms;
using CryBits.Entities;
using DarkUI.Forms;

namespace CryBits.Editors.Forms;

public partial class EditorItemType : DarkForm
{
    public Type ItemType { get; set; } = typeof(Item);

    public EditorItemType()
    {
        InitializeComponent();

        // Adiciona os itens à lista
        object[] itemTypes = {typeof(Item), typeof(ItemEquipment), typeof(ItemPotion)};
        cmbType.Items.AddRange(itemTypes);
        cmbType.SelectedIndex = 0;
    }

    private void butSelect_Click(object sender, EventArgs e)
    {
        ItemType = (Type)cmbType.SelectedValue;
        DialogResult = DialogResult.OK;
        Close();
    }
}