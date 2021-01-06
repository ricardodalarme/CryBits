using System;
using System.Windows.Forms;
using CryBits.Editors.Logic;
using CryBits.Editors.Media.Graphics;
using CryBits.Editors.Network;
using CryBits.Entities;
using CryBits.Enums;
using DarkUI.Forms;
using SFML.Graphics;
using Attribute = CryBits.Enums.Attribute;

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
            Renders.WinClass = new RenderWindow(picTexture.Handle);

            // Define os limites
            numSpawn_X.Maximum = Map.Width - 1;
            numSpawn_Y.Maximum = Map.Height - 1;
            numTexture.Maximum = Textures.Characters.Count - 1;

            // Lista os dados
            foreach (var item in Item.List.Values) cmbItems.Items.Add(item);
            foreach (var map in Map.List.Values) cmbSpawn_Map.Items.Add(map);
            List_Update();
        }

        private void Editor_Classes_FormClosed(object sender, FormClosedEventArgs e)
        {
            Renders.WinClass = null;
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

            // Lista os dados
            txtName.Text = Selected.Name;
            txtDescription.Text = Selected.Description;
            numHP.Value = Selected.Vital[(byte)Vital.HP];
            numMP.Value = Selected.Vital[(byte)Vital.MP];
            numStrength.Value = Selected.Attribute[(byte)Attribute.Strength];
            numResistance.Value = Selected.Attribute[(byte)Attribute.Resistance];
            numIntelligence.Value = Selected.Attribute[(byte)Attribute.Intelligence];
            numAgility.Value = Selected.Attribute[(byte)Attribute.Agility];
            numVitality.Value = Selected.Attribute[(byte)Attribute.Vitality];
            cmbSpawn_Map.SelectedItem = Selected.SpawnMap;
            cmbSpawn_Direction.SelectedIndex = Selected.SpawnDirection;
            numSpawn_X.Value = Selected.SpawnX;
            numSpawn_Y.Value = Selected.SpawnY;
            grpItem_Add.Visible = false;

            // Conecta as listas com os componentes
            lstMale.Tag = Selected.TexMale;
            lstFemale.Tag = Selected.TexFemale;
            lstItems.Tag = Selected.Item;

            // Atualiza as listas
            lstMale.UpdateData();
            lstFemale.UpdateData();
            lstItems.UpdateData();
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
            Send.WriteClasses();
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Send.RequestClasses();
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

        private void butTexture_Ok_Click(object sender, EventArgs e)
        {
            // Adiciona a textura
            if (grpTexture_Add.Tag == lstMale)
                Selected.TexMale.Add((short)numTexture.Value);
            else
                Selected.TexFemale.Add((short)numTexture.Value);
            ((ListBox)grpTexture_Add.Tag).UpdateData();
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
            {
                Selected.TexMale.RemoveAt(lstMale.SelectedIndex);
                lstMale.UpdateData();
            }
        }

        private void butFDelete_Click(object sender, EventArgs e)
        {
            // Deleta a textura
            if (lstFemale.SelectedIndex != -1)
            {
                Selected.TexFemale.RemoveAt(lstFemale.SelectedIndex);
                lstFemale.UpdateData();
            }
        }

        private void cmbSpawn_Map_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.SpawnMap = (Map)cmbSpawn_Map.SelectedItem;
        }

        private void cmbSpawn_Direction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Selected.SpawnDirection = (byte)cmbSpawn_Direction.SelectedIndex;
        }

        private void numSpawn_X_ValueChanged(object sender, EventArgs e)
        {
            Selected.SpawnX = (byte)numSpawn_X.Value;
        }

        private void numSpawn_Y_ValueChanged(object sender, EventArgs e)
        {
            Selected.SpawnY = (byte)numSpawn_Y.Value;
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
            lstItems.UpdateData();
            grpItem_Add.Visible = false;
        }

        private void butItem_Delete_Click(object sender, EventArgs e)
        {
            // Deleta a textura
            if (lstItems.SelectedIndex != -1)
            {
                Selected.Item.RemoveAt(lstItems.SelectedIndex);
                lstItems.UpdateData();
            }
        }
    }
}