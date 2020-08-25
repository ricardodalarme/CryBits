using Entities;
using Network;
using System;
using System.Windows.Forms;
using static Logic.Utils;

namespace Editors
{
    partial class Editor_Items : Form
    {
        // Usado para acessar os dados da janela
        public static Editor_Items Form;

        // Item selecionado
        public Item Selected;

        public Editor_Items()
        {
            // Inicializa os componentes
            InitializeComponent();
            Graphics.Win_Item = new SFML.Graphics.RenderWindow(picTexture.Handle);

            // Define os limites
            numTexture.Maximum = Graphics.Tex_Item.GetUpperBound(0);

            // Lista os dados
            cmbReq_Class.Items.Add("None");
            foreach (var Class in Class.List.Values) cmbReq_Class.Items.Add(Class);
            for (byte i = 0; i < (byte)Rarity.Count; i++) cmbRarity.Items.Add((Rarity)i);
            for (byte i = 0; i < (byte)BindOn.Count; i++) cmbBind.Items.Add((BindOn)i);
            List_Update();

            // Abre a janela
            Editor_Maps.Form.Hide();
            Show();
        }

        private void Groups_Visibility()
        {
            // Atualiza a visiblidade dos paineis
            grpGeneral.Visible = grpRequirements.Visible = List.SelectedNode != null;
            grpEquipment.Visible = grpPotion.Visible = false;
        }

        private void List_Update()
        {
            // Lista os itens
            List.Nodes.Clear();
            foreach (var Item in Item.List.Values)
                if (Item.Name.StartsWith(txtFilter.Text))
                {
                    List.Nodes.Add(Item.Name);
                    List.Nodes[List.Nodes.Count - 1].Tag = Item.ID;
                }

            // Seleciona o primeiro
            if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
            Groups_Visibility();
        }

        private void List_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Atualiza o valor da loja selecionada
            Selected = Item.List[(Guid)List.SelectedNode.Tag];

            // Lista os dados
            txtName.Text = Selected.Name;
            txtDescription.Text = Selected.Description;
            numTexture.Value = Selected.Texture;
            cmbType.SelectedIndex = Selected.Type;
            chkStackable.Checked = Selected.Stackable;
            cmbBind.SelectedIndex = Selected.Bind;
            cmbRarity.SelectedIndex = Selected.Rarity;
            numReq_Level.Value = Selected.Req_Level;
            if (Selected.Req_Class != null) cmbReq_Class.SelectedIndex = cmbReq_Class.Items.IndexOf(Selected.Req_Class);
            else cmbReq_Class.SelectedIndex = 0;
            numPotion_Experience.Value = Selected.Potion_Experience;
            numPotion_HP.Value = Selected.Potion_Vital[(byte)Vitals.HP];
            numPotion_MP.Value = Selected.Potion_Vital[(byte)Vitals.MP];
            cmbEquipment_Type.SelectedIndex = Selected.Equip_Type;
            numEquip_Strength.Value = Selected.Equip_Attribute[(byte)Attributes.Strength];
            numEquip_Resistance.Value = Selected.Equip_Attribute[(byte)Attributes.Resistance];
            numEquip_Intelligence.Value = Selected.Equip_Attribute[(byte)Attributes.Intelligence];
            numEquip_Agility.Value = Selected.Equip_Attribute[(byte)Attributes.Agility];
            numEquip_Vitality.Value = Selected.Equip_Attribute[(byte)Attributes.Vitality];
            numWeapon_Damage.Value = Selected.Weapon_Damage;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            List_Update();
        }

        private void butNew_Click(object sender, EventArgs e)
        {
            // Adiciona uma loja nova
            Item New = new Item(Guid.NewGuid());
            New.Name = "New item";
            Item.List.Add(New.ID, New);

            // Adiciona na lista
            TreeNode Node = new TreeNode(New.Name);
            Node.Tag = New.ID;
            List.Nodes.Add(Node);
            List.SelectedNode = Node;

            // Altera a visiblidade dos grupos caso necessários
            Groups_Visibility();
        }

        private void butRemove_Click(object sender, EventArgs e)
        {
            // Remove a loja selecionada
            if (List.SelectedNode != null)
            {
                Item.List.Remove(Selected.ID);
                List.SelectedNode.Remove();
                Groups_Visibility();
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados e volta à janela principal
            Send.Write_Items();
            Close();
            Editor_Maps.Form.Show();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Close();
            Editor_Maps.Form.Show();
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

        private void cmbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Type = (byte)cmbType.SelectedIndex;

            // Visibilidade dos paineis
            if (cmbType.SelectedIndex == (byte)Items.Equipment) grpEquipment.Visible = true;
            else grpEquipment.Visible = false;
            if (cmbType.SelectedIndex == (byte)Items.Potion) grpPotion.Visible = true;
            else grpPotion.Visible = false;
        }

        private void numReq_Level_ValueChanged(object sender, EventArgs e)
        {
            Selected.Req_Level = (short)numReq_Level.Value;
        }

        private void cmbReq_Class_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbReq_Class.SelectedIndex == 0)
                Selected.Req_Class = null;
            else
                Selected.Req_Class = (Class)cmbReq_Class.SelectedItem;
        }

        private void numEquip_Vida_ValueChanged(object sender, EventArgs e)
        {
            Selected.Potion_Vital[(byte)Vitals.HP] = (short)numPotion_HP.Value;
        }

        private void numEquip_Mana_ValueChanged(object sender, EventArgs e)
        {
            Selected.Potion_Vital[(byte)Vitals.MP] = (short)numPotion_MP.Value;
        }

        private void numEquip_Experience_ValueChanged(object sender, EventArgs e)
        {
            Selected.Potion_Experience = (int)numPotion_Experience.Value;
        }

        private void numEquip_Strength_ValueChanged(object sender, EventArgs e)
        {
            Selected.Equip_Attribute[(byte)Attributes.Strength] = (short)numEquip_Strength.Value;
        }

        private void numEquip_Resistance_ValueChanged(object sender, EventArgs e)
        {
            Selected.Equip_Attribute[(byte)Attributes.Resistance] = (short)numEquip_Resistance.Value;
        }

        private void numEquip_Intelligence_ValueChanged(object sender, EventArgs e)
        {
            Selected.Equip_Attribute[(byte)Attributes.Intelligence] = (short)numEquip_Intelligence.Value;
        }

        private void numEquip_Agility_ValueChanged(object sender, EventArgs e)
        {
            Selected.Equip_Attribute[(byte)Attributes.Agility] = (short)numEquip_Agility.Value;
        }

        private void numEquip_Vitality_ValueChanged(object sender, EventArgs e)
        {
            Selected.Equip_Attribute[(byte)Attributes.Vitality] = (short)numEquip_Vitality.Value;
        }

        private void chkStackable_CheckedChanged(object sender, EventArgs e)
        {
            Selected.Stackable = chkStackable.Checked;
        }

        private void numWeapon_Damage_ValueChanged(object sender, EventArgs e)
        {
            Selected.Weapon_Damage = (short)numWeapon_Damage.Value;
        }

        private void cmbEquipment_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Equip_Type = (byte)cmbEquipment_Type.SelectedIndex;
            numWeapon_Damage.Visible = lblWeapon_Damage.Visible = (cmbEquipment_Type.SelectedIndex == (byte)Equipments.Weapon);
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            Selected.Description = txtDescription.Text;
        }

        private void cmbRarity_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Rarity = (byte)cmbRarity.SelectedIndex;
        }

        private void cmbBind_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Bind = (byte)cmbBind.SelectedIndex;
        }
    }
}