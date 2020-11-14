using System;
using System.Linq;
using System.Windows.Forms;
using CryBits.Editors.Media;
using CryBits.Editors.Network;
using CryBits.Entities;
using DarkUI.Forms;
using SFML.Graphics;

namespace CryBits.Editors.Forms
{
    internal partial class EditorClasses : DarkForm
    {
        // Usado para acessar os dados da janela
        public static EditorClasses Form;

        // Classe selecionada
        public Class Selected;

        public EditorClasses()
        {
            InitializeComponent();

            // Abre janela
            EditorMaps.Form.Hide();
            Show();

            // Inicializa a janela de renderização
            Graphics.Win_Class = new RenderWindow(picTexture.Handle);

            // Define os limites
            numSpawn_X.Maximum = Map.Width - 1;
            numSpawn_Y.Maximum = Map.Height - 1;
            numTexture.Maximum = Graphics.Tex_Character.Length - 1;

            // Lista os dados
            foreach (var item in Item.List.Values) cmbItems.Items.Add(item);
            foreach (var map in Map.List.Values) cmbSpawn_Map.Items.Add(map);
            List_Update();
        }

        private void Editor_Classes_FormClosed(object sender, FormClosedEventArgs e)
        {
            Graphics.Win_Class = null;
            EditorMaps.Form.Show();
        }

        private void Groups_Visibility()
        {
            // Atualiza a visiblidade dos paineis
            grpGeneral.Visible = grpAttributes.Visible = grpDrop.Visible = grpSpawn.Visible = grpTexture.Visible = List.SelectedNode != null;
            grpItem_Add.Visible = false;
        }

        private void List_Update()
        {
            // Lista as classes
            List.Nodes.Clear();
            foreach (var @class in Class.List.Values)
                if (@class.Name.StartsWith(txtFilter.Text))
                    List.Nodes.Add(new TreeNode(@class.Name)
                    {
                        Tag = @class.ID
                    });

            // Seleciona o primeiro
            if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
            Groups_Visibility();
        }

        private void List_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Atualiza o valor da loja selecionada
            Selected = Class.List[(Guid)List.SelectedNode.Tag];

            // Conecta as listas com os componentes
            lstMale.DataSource = Selected.Tex_Male;
            lstFemale.DataSource = Selected.Tex_Female;
            lstItems.DataSource = Selected.Item;

            // Lista os dados
            txtName.Text = Selected.Name;
            txtDescription.Text = Selected.Description;
            numHP.Value = Selected.Vital[(byte)Vitals.HP];
            numMP.Value = Selected.Vital[(byte)Vitals.MP];
            numStrength.Value = Selected.Attribute[(byte)Attributes.Strength];
            numResistance.Value = Selected.Attribute[(byte)Attributes.Resistance];
            numIntelligence.Value = Selected.Attribute[(byte)Attributes.Intelligence];
            numAgility.Value = Selected.Attribute[(byte)Attributes.Agility];
            numVitality.Value = Selected.Attribute[(byte)Attributes.Vitality];
            cmbSpawn_Map.SelectedItem = Selected.Spawn_Map;
            cmbSpawn_Direction.SelectedIndex = Selected.Spawn_Direction;
            numSpawn_X.Value = Selected.Spawn_X;
            numSpawn_Y.Value = Selected.Spawn_Y;
            grpItem_Add.Visible = false;

            // Seleciona os primeiros itens
            if (lstMale.Items.Count > 0) lstMale.SelectedIndex = 0;
            if (lstFemale.Items.Count > 0) lstFemale.SelectedIndex = 0;
            if (lstItems.Items.Count > 0) lstItems.SelectedIndex = 0;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            List_Update();
        }

        private void butNew_Click(object sender, EventArgs e)
        {
            // Adiciona uma loja nova
            Class @new = new Class();
            Class.List.Add(@new.ID, @new);
            @new.Name = "New class";
            @new.Spawn_Map = Map.List.ElementAt(0).Value;

            // Adiciona na lista
            TreeNode node = new TreeNode(@new.Name);
            node.Tag = @new.ID;
            List.Nodes.Add(node);
            List.SelectedNode = node;
        }

        private void butRemove_Click(object sender, EventArgs e)
        {
            if (List.SelectedNode != null)
            {
                // Garante que sempre vai ter pelo menos uma classse
                if (Class.List.Count == 1)
                {
                    MessageBox.Show("It must have at least one class registered.");
                    return;
                }

                // Remove a classes selecionada
                Class.List.Remove(Selected.ID);
                List.SelectedNode.Remove();
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados e volta à janela principal
            Send.Write_Classes();
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Send.Request_Classes();
            Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Atualiza a lista
            Selected.Name = txtName.Text;
            List.SelectedNode.Text = txtName.Text;
        }

        private void txtDescription_TextChanged(object sender, EventArgs e)
        {
            Selected.Description = txtDescription.Text;
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

        private void butTexture_Ok_Click(object sender, EventArgs e)
        {
            // Adiciona a textura
            if (grpTexture_Add.Tag == lstMale)
                Selected.Tex_Male.Add((short)numTexture.Value);
            else
                Selected.Tex_Female.Add((short)numTexture.Value);
            grpTexture_Add.Visible = false;
        }

        private void butMTexture_Click(object sender, EventArgs e)
        {
            // Abre a janela para adicionar a textura
            numTexture.Value = 1;
            grpTexture_Add.Tag = lstMale;
            grpTexture_Add.Visible = true;
        }

        private void butFTexture_Click(object sender, EventArgs e)
        {
            // Abre a janela para adicionar a textura
            numTexture.Value = 1;
            grpTexture_Add.Tag = lstFemale;
            grpTexture_Add.Visible = true;
        }

        private void butMDelete_Click(object sender, EventArgs e)
        {
            // Deleta a textura
            if (lstMale.SelectedIndex != -1)
                Selected.Tex_Male.RemoveAt(lstMale.SelectedIndex);
        }

        private void butFDelete_Click(object sender, EventArgs e)
        {
            // Deleta a textura
            if (lstFemale.SelectedIndex != -1)
                Selected.Tex_Female.RemoveAt(lstFemale.SelectedIndex);
        }

        private void cmbSpawn_Map_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Spawn_Map = (Map)cmbSpawn_Map.SelectedItem;
        }

        private void cmbSpawn_Direction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.Spawn_Direction = (byte)cmbSpawn_Direction.SelectedIndex;
        }

        private void numSpawn_X_ValueChanged(object sender, EventArgs e)
        {
            Selected.Spawn_X = (byte)numSpawn_X.Value;
        }

        private void numSpawn_Y_ValueChanged(object sender, EventArgs e)
        {
            Selected.Spawn_Y = (byte)numSpawn_Y.Value;
        }

        private void butItem_Add_Click(object sender, EventArgs e)
        {
            // Evita erros
            if (Item.List.Count == 0)
            {
                MessageBox.Show("It must have at least one item registered add initial items.");
                return;
            }

            // Abre a janela para adicionar o item
            grpItem_Add.Visible = true;
            cmbItems.SelectedIndex = 0;
            numItem_Amount.Value = 1;
        }

        private void butItem_Ok_Click(object sender, EventArgs e)
        {
            // Adiciona o item
            Selected.Item.Add(new ItemSlot((Item)cmbItems.SelectedItem, (short)numItem_Amount.Value));
            grpItem_Add.Visible = false;
        }

        private void butItem_Delete_Click(object sender, EventArgs e)
        {
            // Deleta a textura
            if (lstItems.SelectedIndex != -1)
                Selected.Item.RemoveAt(lstItems.SelectedIndex);
        }
    }
}