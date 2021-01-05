using System;
using System.Linq;
using System.Windows.Forms;
using CryBits.Editors.Logic;
using CryBits.Editors.Network;
using CryBits.Entities;
using DarkUI.Forms;

namespace CryBits.Editors.Forms
{
    internal partial class EditorShops : DarkForm
    {
        // Loja selecionada
        private Shop _selected;

        public EditorShops()
        {
            // Verifica se é possível abrir
            if (Item.List.Count == 0)
            {
                MessageBox.Show("It must have at least one item registered to open the store editor.");
                return;
            }

            // Inicializa os componentes
            InitializeComponent();

            // Abre a janela
            EditorMaps.Form.Hide();
            Show();

            // Lista os dados
            foreach (var item in Item.List.Values)
            {
                cmbItems.Items.Add(item);
                cmbCurrency.Items.Add(item);
            }
            List_Update();
        }

        private void Editor_Shops_FormClosed(object sender, FormClosedEventArgs e)
        {
            EditorMaps.Form.Show();
        }

        private void Groups_Visibility()
        {
            // Atualiza a visiblidade dos paineis
            grpGeneral.Visible = grpBought.Visible = grpSold.Visible = List.SelectedNode != null;
            grpAddItem.Visible = false;
        }

        private void List_Update()
        {
            // Lista as lojas
            foreach (var shop in Shop.List.Values)
                if (shop.Name.StartsWith(txtFilter.Text))
                    List.Nodes.Add(new TreeNode(shop.Name)
                    {
                        Tag = shop.ID
                    });

            // Seleciona o primeiro
            if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
            Groups_Visibility();
        }

        private void List_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Atualiza o valor da loja selecionada
            _selected = Shop.List[(Guid)List.SelectedNode.Tag];

            // Lista os dados
            txtName.Text = _selected.Name;
            cmbCurrency.SelectedItem = _selected.Currency;
            grpAddItem.Visible = false;

            // Conecta as listas com os componentes
            lstBought.Tag = _selected.Bought;
            lstSold.Tag = _selected.Sold;

            // Atualiza as listas
            lstBought.UpdateData();
            lstSold.UpdateData();
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            List_Update();
        }

        private void butNew_Click(object sender, EventArgs e)
        {
            // Adiciona uma loja nova
            Shop shop = new Shop();
            Shop.List.Add(shop.ID, shop);

            // Adiciona na lista
            TreeNode node = new TreeNode(shop.Name);
            node.Tag = shop.ID;
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
                Shop.List.Remove(_selected.ID);
                List.SelectedNode.Remove();
                Groups_Visibility();
            }
        }

        private void butSave_Click(object sender, EventArgs e)
        {
            // Salva os dados e volta à janela principal
            Send.WriteShops();
            Close();
        }

        private void butCancel_Click(object sender, EventArgs e)
        {
            // Volta à janela principal
            Send.RequestShops();
            Close();
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            // Atualiza a lista
            _selected.Name = txtName.Text;
            List.SelectedNode.Text = txtName.Text;
        }

        private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selected.Currency = (Item)cmbCurrency.SelectedItem;
        }

        private void butSold_Add_Click(object sender, EventArgs e)
        {
            // Abre o painel para adicionar o item
            grpAddItem.Visible = true;
            cmbItems.SelectedIndex = 0;
            numAmount.Value = 1;
            numPrice.Value = 0;
            grpAddItem.Tag = lstSold;
            grpAddItem.BringToFront();
        }

        private void butSold_Remove_Click(object sender, EventArgs e)
        {
            // Remove o item
            if (lstSold.SelectedIndex >= 0) _selected.Sold.RemoveAt(lstSold.SelectedIndex);
            lstSold.UpdateData();
        }

        private void butBought_Add_Click(object sender, EventArgs e)
        {
            // Abre o painel para adicionar o item
            cmbItems.SelectedIndex = 0;
            numAmount.Value = 1;
            numPrice.Value = 0;
            grpAddItem.Tag = lstBought;
            grpAddItem.Visible = true;
            grpAddItem.BringToFront();
        }

        private void butBought_Remove_Click(object sender, EventArgs e)
        {
            // Remove o item
            if (lstBought.SelectedIndex >= 0)
            {
                _selected.Bought.RemoveAt(lstSold.SelectedIndex);
                lstBought.UpdateData();
            }
        }

        private void butConfirm_Click(object sender, EventArgs e)
        {
            // Adiciona o item
            ShopItem data = new ShopItem((Item)cmbItems.SelectedItem, (short)numAmount.Value, (short)numPrice.Value);
            if (grpAddItem.Tag == lstSold) _selected.Sold.Add(data);
            else _selected.Bought.Add(data);
            ((ListBox)grpAddItem.Tag).UpdateData();

            // Fecha o painel
            grpAddItem.Visible = false;
        }
    }
}