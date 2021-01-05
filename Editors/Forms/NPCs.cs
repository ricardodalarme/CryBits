using System;
using System.Windows.Forms;
using CryBits.Editors.Logic;
using CryBits.Editors.Media;
using CryBits.Editors.Media.Graphics;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Enums;
using DarkUI.Forms;
using SFML.Graphics;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Editors.Forms
{
    internal partial class EditorNpcs : DarkForm
    {
        // Usado para acessar os dados da janela
        public static EditorNpcs Form;

        // Npc selecionado
        public Npc Selected;

        public EditorNpcs()
        {
            InitializeComponent();

            // Abre janela
            EditorMaps.Form.Hide();
            Show();

            // Inicializa a janela de renderização
            Renders.WinNpc = new RenderWindow(picTexture.Handle);

            // Define os limites
            numTexture.Maximum =Textures.Characters.Count-1;

            // Lista os dados
            foreach (var item in Item.List.Values) cmbDrop_Item.Items.Add(item);
            foreach (var shop in Shop.List.Values) cmbShop.Items.Add(shop);
            List_Update();
        }

        private void Editor_Npcs_FormClosed(object sender, FormClosedEventArgs e)
        {
            Renders.WinNpc = null;
            EditorMaps.Form.Show();
        }

        private void Groups_Visibility()
        {
            // Atualiza a visiblidade dos paineis
            grpGeneral.Visible = grpAttributes.Visible = grpBehaviour.Visible = grpDrop.Visible = grpAllies.Visible = List.SelectedNode != null;
            grpAllie_Add.Visible = grpDrop_Add.Visible = false;
        }

        private void List_Update()
        {
            // Lista os Npcs
            List.Nodes.Clear();
            foreach (var npc in Npc.List.Values)
                if (npc.Name.StartsWith(txtFilter.Text))
                    List.Nodes.Add(new TreeNode(npc.Name)
                    {
                        Tag = npc.ID
                    });

            // Seleciona o primeiro
            if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
            Groups_Visibility();
        }

        private void List_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Atualiza o valor da loja selecionada
            Selected = Npc.List[(Guid)List.SelectedNode.Tag];

            // Reseta os dados necessários
            grpDrop_Add.Visible = false;
            grpAllie_Add.Visible = false;

            // Lista os dados
            txtName.Text = Selected.Name;
            txtSayMsg.Text = Selected.SayMsg;
            numTexture.Value = Selected.Texture;
            cmbBehavior.SelectedIndex = (byte)Selected.Behaviour;
            numSpawn.Value = Selected.SpawnTime;
            numRange.Value = Selected.Sight;
            numExperience.Value = Selected.Experience;
            numHP.Value = Selected.Vital[(byte)Vital.HP];
            numMP.Value = Selected.Vital[(byte)Vital.MP];
            numStrength.Value = Selected.Attribute[(byte)Attribute.Strength];
            numResistance.Value = Selected.Attribute[(byte)Attribute.Resistance];
            numIntelligence.Value = Selected.Attribute[(byte)Attribute.Intelligence];
            numAgility.Value = Selected.Attribute[(byte)Attribute.Agility];
            numVitality.Value = Selected.Attribute[(byte)Attribute.Vitality];
            cmbMovement.SelectedIndex = (byte)Selected.Movement;
            numFlee_Health.Value = Selected.FleeHealth;
            chkAttackNpc.Checked = Selected.AttackNpc;
            if (Selected.Shop != null) cmbShop.SelectedItem = Selected.Shop;
            else cmbShop.SelectedIndex = -1;

            // Conecta as listas com os componentes
            lstDrop.Tag = Selected.Drop;
            lstAllies.Tag = Selected.Allie;

            // Atualiza as listas
            lstDrop.UpdateData();
            lstAllies.UpdateData();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            List_Update();
        }

        private void butNew_Click(object sender, EventArgs e)
        {
            // Adiciona uma loja nova
            Npc @new = new Npc();
            @new.Name = "New Npc";
            Npc.List.Add(@new.ID, @new);

            // Adiciona na lista
            TreeNode node = new TreeNode(@new.Name);
            node.Tag = @new.ID;
            List.Nodes.Add(node);
            List.SelectedNode = node;

            // Altera a visiblidade dos grupos caso necessários
            Groups_Visibility();
        }

        private void butRemove_Click(object sender, EventArgs e)
        {
            // Remove a loja selecionada
            if (List.SelectedNode != null)
            {
                Npc.List.Remove(Selected.ID);
                List.SelectedNode.Remove();
                Groups_Visibility();
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados e volta à janela principal
            Send.WriteNpcs();
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Send.RequestNpcs();
            Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Atualiza a lista
            Selected.Name = txtName.Text;
            List.SelectedNode.Text = txtName.Text;
        }

        private void txtSayMsg_TextChanged(object sender, EventArgs e)
        {
            Selected.SayMsg = txtSayMsg.Text;
        }

        private void numTexture_ValueChanged(object sender, EventArgs e)
        {
            Selected.Texture = (byte)numTexture.Value;
        }

        private void numRange_ValueChanged(object sender, EventArgs e)
        {
            Selected.Sight = (byte)numRange.Value;
        }

        private void cmbBehavior_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Evita erros
            cmbShop.Enabled = false;
            cmbShop.SelectedIndex = -1;
            if (cmbBehavior.SelectedIndex == (byte)Behaviour.ShopKeeper)
                if (Shop.List.Count == 0)
                {
                    cmbBehavior.SelectedIndex = (byte)Selected.Behaviour;
                    return;
                }
                else
                {
                    cmbShop.Enabled = true;
                    if (Selected.Shop == null) cmbShop.SelectedIndex = 0;
                }

            Selected.Behaviour = (Behaviour)cmbBehavior.SelectedIndex;
        }

        private void numHP_ValueChanged(object sender, EventArgs e)
        {
            Selected.Vital[(byte)Vital.HP] = (short)numHP.Value;
        }

        private void numMP_ValueChanged(object sender, EventArgs e)
        {
            Selected.Vital[(byte)Vital.MP] = (short)numMP.Value;
        }

        private void numStrength_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attribute.Strength] = (short)numStrength.Value;
        }

        private void numResistance_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attribute.Resistance] = (short)numResistance.Value;
        }

        private void numIntelligence_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attribute.Intelligence] = (short)numIntelligence.Value;
        }

        private void numAgility_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attribute.Agility] = (short)numAgility.Value;
        }

        private void numVitality_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attribute.Vitality] = (short)numVitality.Value;
        }

        private void numSpawn_ValueChanged(object sender, EventArgs e)
        {
            Selected.SpawnTime = (byte)numSpawn.Value;
        }

        private void numExperience_ValueChanged(object sender, EventArgs e)
        {
            Selected.Experience = (int)numExperience.Value;
        }

        private void butDrop_Add_Click(object sender, EventArgs e)
        {
            // Evita erros
            if (Item.List.Count == 0)
            {
                MessageBox.Show("It must have at least one item registered add initial items.");
                return;
            }

            // Reseta os valores e abre a janela para adicionar o item
            grpDrop_Add.Visible = true;
            cmbDrop_Item.SelectedIndex = 0;
            numDrop_Amount.Value = 1;
            numDrop_Chance.Value = 100;
        }

        private void butDrop_Delete_Click(object sender, EventArgs e)
        {
            // Deleta a item
            short selectedItem = (short)lstDrop.SelectedIndex;
            if (selectedItem != -1)
            {
                Selected.Drop.RemoveAt(selectedItem);
                lstDrop.UpdateData();
            }
        }

        private void butItem_Ok_Click(object sender, EventArgs e)
        {
            // Adiciona o item
            Selected.Drop.Add(new NpcDrop((Item)cmbDrop_Item.SelectedItem, (short)numDrop_Amount.Value, (byte)numDrop_Chance.Value));
            lstDrop.UpdateData();
            grpDrop_Add.Visible = false;
        }

        private void chkAttackNpc_CheckedChanged(object sender, EventArgs e)
        {
            Selected.AttackNpc = lstAllies.Enabled = chkAttackNpc.Checked;
            if (!Selected.AttackNpc) Selected.Allie.Clear();
        }

        private void butAllie_Add_Click(object sender, EventArgs e)
        {
            if (chkAttackNpc.Checked)
            {
                // Abre a janela para adicionar o aliado
                grpAllie_Add.Visible = true;

                // Adiciona os Npcs
                cmbAllie_Npc.Items.Clear();
                foreach (var npc in Npc.List.Values) cmbAllie_Npc.Items.Add(npc);
                cmbAllie_Npc.SelectedIndex = 0;
            }
        }

        private void butAllie_Delete_Click(object sender, EventArgs e)
        {
            // Deleta a aliado
            if (lstAllies.SelectedItem != null)
            {
                Selected.Allie.Remove((Npc)lstAllies.SelectedItem);
                lstAllies.UpdateData();
            }
        }

        private void butAllie_Ok_Click(object sender, EventArgs e)
        {
            // Adiciona o aliado
            var allie = (Npc)cmbAllie_Npc.SelectedItem;
            if (!Selected.Allie.Contains(allie)) Selected.Allie.Add(allie);
            lstAllies.UpdateData();
            grpAllie_Add.Visible = false;
        }

        private void cmbMovement_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Movement = (MovementStyle)cmbMovement.SelectedIndex;
        }

        private void numFlee_Health_ValueChanged(object sender, EventArgs e)
        {
            Selected.FleeHealth = (byte)numFlee_Health.Value;
        }

        private void cmbShop_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Shop = (Shop)cmbShop.SelectedItem;
        }
    }
}