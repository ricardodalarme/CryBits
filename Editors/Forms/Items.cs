using System;
using System.Windows.Forms;
using CryBits.Editors.Media.Graphics;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Enums;
using DarkUI.Forms;
using SFML.Graphics;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Editors.Forms;

internal partial class EditorItems : DarkForm
{
    // Usado para acessar os dados da janela
    public static EditorItems Form;

    // Item selecionado
    public Item Selected;

    public EditorItems()
    {
        InitializeComponent();

        // Abre janela
        EditorMaps.Form.Hide();
        Show();

        // Inicializa a janela de renderização
        Renders.WinItem = new RenderWindow(picTexture.Handle);

        // Define os limites
        numTexture.Maximum = Textures.Items.Count-1;

        // Lista os dados
        cmbReq_Class.Items.Add("None");
        foreach (var @class in Class.List.Values) cmbReq_Class.Items.Add(@class);
        for (byte i = 0; i < (byte)Rarity.Count; i++) cmbRarity.Items.Add((Rarity)i);
        for (byte i = 0; i < (byte)BindOn.Count; i++) cmbBind.Items.Add((BindOn)i);
        List_Update();
    }

    private void Editor_Items_FormClosed(object sender, FormClosedEventArgs e)
    {
        Renders.WinItem = null;
        EditorMaps.Form.Show();
    }

    private void Groups_Visibility()
    {
        // Atualiza a visiblidade dos paineis
        grpGeneral.Visible = grpRequirements.Visible = List.SelectedNode != null;
        if (List.SelectedNode == null) grpEquipment.Visible = grpPotion.Visible = false;
    }

    private void List_Update()
    {
        // Lista os itens
        List.Nodes.Clear();
        foreach (var item in Item.List.Values)
            if (item.Name.StartsWith(txtFilter.Text))
                List.Nodes.Add(new TreeNode(item.Name)
                {
                    Tag = item.Id
                });

        // Seleciona o primeiro
        if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
        Groups_Visibility();
    }

    private void List_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza o valor da loja selecionada
        Selected = Item.List[(Guid)List.SelectedNode.Tag];

        // Visibilidade dos paineis
        grpEquipment.Visible = Selected is ItemEquipment;
        grpPotion.Visible = Selected is ItemPotion;

        // Lista os dados
        txtName.Text = Selected.Name;
        txtDescription.Text = Selected.Description;
        numTexture.Value = Selected.Texture;
        chkStackable.Checked = Selected.Stackable;
        cmbBind.SelectedIndex = (byte)Selected.Bind;
        cmbRarity.SelectedIndex = (byte)Selected.Rarity;
        numReq_Level.Value = Selected.ReqLevel;
        cmbReq_Class.SelectedIndex = Selected.ReqClass != null ? cmbReq_Class.Items.IndexOf(Selected.ReqClass) : 0;

        switch (Selected)
        {
            case ItemPotion potion:
                numPotion_Experience.Value = potion.PotionExperience;
                numPotion_HP.Value = potion.PotionVital[(byte) Vital.Hp];
                numPotion_MP.Value = potion.PotionVital[(byte) Vital.Mp];
                break;
            case ItemEquipment equipment:
                cmbEquipment_Type.SelectedIndex = equipment.EquipType;
                numEquip_Strength.Value = equipment.EquipAttribute[(byte) Attribute.Strength];
                numEquip_Resistance.Value = equipment.EquipAttribute[(byte) Attribute.Resistance];
                numEquip_Intelligence.Value = equipment.EquipAttribute[(byte) Attribute.Intelligence];
                numEquip_Agility.Value = equipment.EquipAttribute[(byte) Attribute.Agility];
                numEquip_Vitality.Value = equipment.EquipAttribute[(byte) Attribute.Vitality];
                numWeapon_Damage.Value = equipment.WeaponDamage;
                break;
        }
    }

    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
        List_Update();
    }

    private void butNew_Click(object sender, EventArgs e)
    {
        var selectForm = new EditorItemType();
        var result = selectForm.ShowDialog();

        if (result == DialogResult.OK)
        {
            var type = selectForm.ItemType;
            Item item;

            if (type == typeof(ItemEquipment))
                item = new ItemEquipment();
            else if (type == typeof(ItemPotion))
                item = new ItemPotion();
            else
                item = new Item();

            // Cria um item novo
            Item.List.Add(item.Id, item);

            // Adiciona na lista
            var node = new TreeNode(item.Name) { Tag = item.Id };
            List.Nodes.Add(node);
            List.SelectedNode = node;

            // Altera a visiblidade dos grupos caso necessários
            Groups_Visibility();
        }
    }

    private void butRemove_Click(object sender, EventArgs e)
    {
        // Remove a loja selecionada
        if (List.SelectedNode != null)
        {
            Item.List.Remove(Selected.Id);
            List.SelectedNode.Remove();
            Groups_Visibility();
        }
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados e volta à janela principal
        Send.WriteItems();
        Close();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta à janela principal
        Send.RequestItems();
        Close();
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Selected.Name = txtName.Text;
        List.SelectedNode.Text = txtName.Text;
    }

    private void numTexture_ValueChanged(object sender, EventArgs e)
    {
        Selected.Texture = (short)numTexture.Value;
    }

    private void numReq_Level_ValueChanged(object sender, EventArgs e)
    {
        Selected.ReqLevel = (short)numReq_Level.Value;
    }

    private void cmbReq_Class_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbReq_Class.SelectedIndex == 0)
            Selected.ReqClass = null;
        else
            Selected.ReqClass = (Class)cmbReq_Class.SelectedItem;
    }

    private void numPotion_HP_ValueChanged(object sender, EventArgs e)
    {
        ((ItemPotion)Selected).PotionVital[(byte)Vital.Hp] = (short)numPotion_HP.Value;
    }

    private void numPotion_MP_ValueChanged(object sender, EventArgs e)
    {
        ((ItemPotion)Selected).PotionVital[(byte)Vital.Mp] = (short)numPotion_MP.Value;
    }

    private void numEquip_Experience_ValueChanged(object sender, EventArgs e)
    {
        ((ItemPotion)Selected).PotionExperience = (int)numPotion_Experience.Value;
    }

    private void numEquip_Strength_ValueChanged(object sender, EventArgs e)
    {
        ((ItemEquipment)Selected).EquipAttribute[(byte)Attribute.Strength] = (short)numEquip_Strength.Value;
    }

    private void numEquip_Resistance_ValueChanged(object sender, EventArgs e)
    {
        ((ItemEquipment)Selected).EquipAttribute[(byte)Attribute.Resistance] = (short)numEquip_Resistance.Value;
    }

    private void numEquip_Intelligence_ValueChanged(object sender, EventArgs e)
    {
        ((ItemEquipment)Selected).EquipAttribute[(byte)Attribute.Intelligence] = (short)numEquip_Intelligence.Value;
    }

    private void numEquip_Agility_ValueChanged(object sender, EventArgs e)
    {
        ((ItemEquipment)Selected).EquipAttribute[(byte)Attribute.Agility] = (short)numEquip_Agility.Value;
    }

    private void numEquip_Vitality_ValueChanged(object sender, EventArgs e)
    {
        ((ItemEquipment)Selected).EquipAttribute[(byte)Attribute.Vitality] = (short)numEquip_Vitality.Value;
    }

    private void chkStackable_CheckedChanged(object sender, EventArgs e)
    {
        Selected.Stackable = chkStackable.Checked;
    }

    private void numWeapon_Damage_ValueChanged(object sender, EventArgs e)
    {
        ((ItemEquipment)Selected).WeaponDamage = (short)numWeapon_Damage.Value;
    }

    private void cmbEquipment_Type_SelectedIndexChanged(object sender, EventArgs e)
    {
        ((ItemEquipment)Selected).EquipType = (byte)cmbEquipment_Type.SelectedIndex;
        numWeapon_Damage.Visible = lblWeapon_Damage.Visible = cmbEquipment_Type.SelectedIndex == (byte)Equipment.Weapon;
    }

    private void txtDescription_TextChanged(object sender, EventArgs e)
    {
        Selected.Description = txtDescription.Text;
    }

    private void cmbRarity_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Rarity = (Rarity)cmbRarity.SelectedIndex;
    }

    private void cmbBind_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Bind = (BindOn)cmbBind.SelectedIndex;
    }
}