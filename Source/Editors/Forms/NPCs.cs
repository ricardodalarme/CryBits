using CryBits.Editors.Network;
using CryBits.Entities;
using DarkUI.Forms;
using System;
using System.Windows.Forms;
using CryBits.Editors.Media;

namespace CryBits.Editors.Forms
{
    partial class EditorNPCs : DarkForm
    {
        // Usado para acessar os dados da janela
        public static EditorNPCs Form;

        // NPC selecionado
        public NPC Selected;

        public EditorNPCs()
        {
            InitializeComponent();

            // Abre janela
            EditorMaps.Form.Hide();
            Show();

            // Inicializa a janela de renderização
            Graphics.Win_NPC = new SFML.Graphics.RenderWindow(picTexture.Handle);

            // Define os limites
            numTexture.Maximum = Graphics.Tex_Character.GetUpperBound(0);

            // Lista os dados
            foreach (var item in Item.List.Values) cmbDrop_Item.Items.Add(item);
            foreach (var shop in Shop.List.Values) cmbShop.Items.Add(shop);
            List_Update();
        }

        private void Editor_NPCs_FormClosed(object sender, FormClosedEventArgs e)
        {
            Graphics.Win_NPC = null;
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
            // Lista os NPCBehaviour
            List.Nodes.Clear();
            foreach (var npc in NPC.List.Values)
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
            Selected = NPC.List[(Guid)List.SelectedNode.Tag];

            // Reseta os dados necessários
            grpDrop_Add.Visible = false;
            grpAllie_Add.Visible = false;

            // Conecta as listas com os componentes
            lstDrop.DataSource = Selected.Drop;
            lstAllies.DataSource = Selected.Allie;

            // Lista os dados
            txtName.Text = Selected.Name;
            txtSayMsg.Text = Selected.SayMsg;
            numTexture.Value = Selected.Texture;
            cmbBehavior.SelectedIndex = (byte)Selected.Behaviour;
            numSpawn.Value = Selected.SpawnTime;
            numRange.Value = Selected.Sight;
            numExperience.Value = Selected.Experience;
            numHP.Value = Selected.Vital[(byte)Vitals.HP];
            numMP.Value = Selected.Vital[(byte)Vitals.MP];
            numStrength.Value = Selected.Attribute[(byte)Attributes.Strength];
            numResistance.Value = Selected.Attribute[(byte)Attributes.Resistance];
            numIntelligence.Value = Selected.Attribute[(byte)Attributes.Intelligence];
            numAgility.Value = Selected.Attribute[(byte)Attributes.Agility];
            numVitality.Value = Selected.Attribute[(byte)Attributes.Vitality];
            cmbMovement.SelectedIndex = (byte)Selected.Movement;
            numFlee_Health.Value = Selected.Flee_Helth;
            chkAttackNPC.Checked = Selected.AttackNPC;
            if (Selected.Shop != null) cmbShop.SelectedItem = Selected.Shop;
            else cmbShop.SelectedIndex = -1;

            // Seleciona os primeiros itens
            if (lstDrop.Items.Count > 0) lstDrop.SelectedIndex = 0;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            List_Update();
        }

        private void butNew_Click(object sender, EventArgs e)
        {
            // Adiciona uma loja nova
            NPC @new = new NPC(Guid.NewGuid());
            @new.Name = "New NPC";
            NPC.List.Add(@new.ID, @new);

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
                NPC.List.Remove(Selected.ID);
                List.SelectedNode.Remove();
                Groups_Visibility();
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados e volta à janela principal
            Send.Write_NPCs();
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
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
            if (cmbBehavior.SelectedIndex == (byte)NPCBehaviour.ShopKeeper)
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

            Selected.Behaviour = (NPCBehaviour)cmbBehavior.SelectedIndex;
        }

        private void numHP_ValueChanged(object sender, EventArgs e)
        {
            Selected.Vital[(byte)Vitals.HP] = (short)numHP.Value;
        }

        private void numMP_ValueChanged(object sender, EventArgs e)
        {
            Selected.Vital[(byte)Vitals.MP] = (short)numMP.Value;
        }

        private void numStrength_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attributes.Strength] = (short)numStrength.Value;
        }

        private void numResistance_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attributes.Resistance] = (short)numResistance.Value;
        }

        private void numIntelligence_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attributes.Intelligence] = (short)numIntelligence.Value;
        }

        private void numAgility_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attributes.Agility] = (short)numAgility.Value;
        }

        private void numVitality_ValueChanged(object sender, EventArgs e)
        {
            Selected.Attribute[(byte)Attributes.Vitality] = (short)numVitality.Value;
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
                MessageBox.Show("It must have at least one item registered add inital items.");
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
                Selected.Drop.RemoveAt(selectedItem);
        }

        private void butItem_Ok_Click(object sender, EventArgs e)
        {
            // Adiciona o item
            Selected.Drop.Add(new NPCDrop((Item)cmbDrop_Item.SelectedItem, (short)numDrop_Amount.Value, (byte)numDrop_Chance.Value));
            grpDrop_Add.Visible = false;
        }

        private void chkAttackNPC_CheckedChanged(object sender, EventArgs e)
        {
            Selected.AttackNPC = lstAllies.Enabled = chkAttackNPC.Checked;
            if (!Selected.AttackNPC) Selected.Allie.Clear();
        }

        private void butAllie_Add_Click(object sender, EventArgs e)
        {
            if (chkAttackNPC.Checked)
            {
                // Abre a janela para adicionar o aliado
                grpAllie_Add.Visible = true;

                // Adiciona os NPCBehaviour
                cmbAllie_NPC.Items.Clear();
                foreach (var npc in NPC.List.Values) cmbAllie_NPC.Items.Add(npc);
                cmbAllie_NPC.SelectedIndex = 0;
            }
        }

        private void butAllie_Delete_Click(object sender, EventArgs e)
        {
            // Deleta a aliado
            if (lstAllies.SelectedItem != null) Selected.Allie.Remove((NPC)lstAllies.SelectedItem);
        }

        private void butAllie_Ok_Click(object sender, EventArgs e)
        {
            // Adiciona o aliado
            var allie = (NPC)cmbAllie_NPC.SelectedItem;
            if (!Selected.Allie.Contains(allie)) Selected.Allie.Add(allie);
            grpAllie_Add.Visible = false;
        }

        private void cmbMovement_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Movement = (NPCMovements)cmbMovement.SelectedIndex;
        }

        private void numFlee_Health_ValueChanged(object sender, EventArgs e)
        {
            Selected.Flee_Helth = (byte)numFlee_Health.Value;
        }

        private void cmbShop_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Shop = (Shop)cmbShop.SelectedItem;
        }
    }
}