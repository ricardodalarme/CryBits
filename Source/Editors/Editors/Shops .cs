using System;
using System.Linq;
using System.Windows.Forms;

partial class Editor_Shops : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Shops Form;

    // Loja selecionada
    private Lists.Structures.Shop Selected;

    public Editor_Shops()
    {
        // Verifica se é possível abrir
        if (Lists.Item.Count == 0)
        {
            MessageBox.Show("It must have at least one item registered to open the store editor.");
            return;
        }

        // Inicializa os componentes
        InitializeComponent();

        // Lista os dados
        foreach (Lists.Structures.Item Item in Lists.Item.Values)
        {
            cmbItems.Items.Add(Item);
            cmbCurrency.Items.Add(Item);
        }
        List_Update();

        // Abre a janela
        Editor_Maps.Form.Hide();
        Show();
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
        foreach (var Shop in Lists.Shop.Values)
            if (Shop.Name.StartsWith(txtFilter.Text))
            {
                List.Nodes.Add(Shop.Name);
                List.Nodes[List.Nodes.Count - 1].Tag = Shop.ID;
            }

        // Seleciona o primeiro
        if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
        Groups_Visibility();
    }

    private void List_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza o valor da loja selecionada
        Selected = Lists.Shop[(Guid)List.SelectedNode.Tag];

        // Conecta as listas com os componentes
        lstBought.DataSource = Selected.Bought;
        lstSold.DataSource = Selected.Sold;

        // Lista os dados
        txtName.Text = Selected.Name;
        cmbCurrency.SelectedItem = Selected.Currency;
        grpAddItem.Visible = false;
    }

    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
        List_Update();
    }

    private void butNew_Click(object sender, EventArgs e)
    {
        // Adiciona uma loja nova
        Lists.Structures.Shop Shop = new Lists.Structures.Shop(Guid.NewGuid());
        Shop.Name = "New shop";
        Shop.Currency = Lists.Item.ElementAt(0).Value;
        Lists.Shop.Add(Shop.ID, Shop);

        // Adiciona na lista
        TreeNode Node = new TreeNode(Shop.Name);
        Node.Tag = Shop.ID;
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
            Lists.Shop.Remove(Selected.ID);
            List.SelectedNode.Remove();
            Groups_Visibility();
        }
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados e volta à janela principal
        Send.Write_Shops();
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

    private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Currency = (Lists.Structures.Item)cmbCurrency.SelectedItem;
    }

    private void butSold_Add_Click(object sender, EventArgs e)
    {
        // Abre o painel para adicionar o item
        cmbItems.SelectedIndex = 0;
        numAmount.Value = 1;
        numPrice.Value = 0;
        grpAddItem.Tag = lstSold;
        grpAddItem.Visible = true;
        grpAddItem.BringToFront();
    }

    private void butSold_Remove_Click(object sender, EventArgs e)
    {
        // Remove o item
        if (lstSold.SelectedIndex >= 0) Selected.Sold.RemoveAt(lstSold.SelectedIndex);
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
        if (lstBought.SelectedIndex >= 0)  Selected.Bought.RemoveAt(lstSold.SelectedIndex);
    }

    private void butConfirm_Click(object sender, EventArgs e)
    {
        // Adiciona o item
        Lists.Structures.Shop_Item Data = new Lists.Structures.Shop_Item((Lists.Structures.Item)cmbItems.SelectedItem, (short)numAmount.Value, (short)numPrice.Value);
        if (grpAddItem.Tag == lstSold) Selected.Sold.Add(Data);
        else Selected.Bought.Add(Data);

        // Fecha o painel
        grpAddItem.Visible = false;
    }
}