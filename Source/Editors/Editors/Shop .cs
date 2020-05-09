using System;
using System.Windows.Forms;

partial class Editor_Shops : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Shops Objects = new Editor_Shops();

    // Loja selecionada
    private Lists.Structures.Shop Selected;

    public Editor_Shops()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Lê os dados
        Globals.OpenEditor = Objects;
        Send.Request_Items();
        Send.Request_Shops();
    }

    public static void Open()
    {
        // Lista os itens
        Objects.cmbCurrency.Items.Clear();
        Objects.cmbCurrency.Items.Add("Free");
        Objects.cmbItems.Items.Clear();
        for (short i = 1; i < Lists.Item.Length; i++)
        {
            Objects.cmbCurrency.Items.Add(Lists.Item[i].Name);
            Objects.cmbItems.Items.Add(Lists.Item[i].Name);
        }

        // Lista as lojas
        Objects.List.Nodes.Clear();
        foreach (Lists.Structures.Shop Shop in Lists.Shop.Values)
        {
            Objects.List.Nodes.Add(Shop.Name);
            Objects.List.Nodes[Objects.List.Nodes.Count - 1].Tag = Shop.ID;
        }
        if (Objects.List.Nodes.Count > 0) Objects.List.SelectedNode = Objects.List.Nodes[0];

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private void Groups_Visibility()
    {
        // Atualiza a visiblidade dos paineis
        grpGeneral.Visible = grpBought.Visible = grpSold.Visible = List.SelectedNode != null;
        grpAddItem.Visible = false;
        List.Focus();
    }

    private void List_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza o valor da loja selecionada
        Selected = Lists.Shop[(Guid)List.SelectedNode.Tag];

        // Limpa os dados necessários
        lstSold.Items.Clear();
        lstBought.Items.Clear();
        grpAddItem.Visible = false;

        // Lista os dados
        txtName.Text = Selected.Name;
        cmbCurrency.SelectedIndex = Selected.Currency;
        for (byte i = 0; i < Selected.Sold.Count; i++) lstSold.Items.Add(List_Text(Selected.Sold[i]));
        for (byte i = 0; i < Selected.Bought.Count; i++) lstBought.Items.Add(List_Text(Selected.Bought[i]));

        // Altera a visibilidade dos grupos se necessário
        Groups_Visibility();
    }

    private void butNew_Click(object sender, EventArgs e)
    {
        // Adiciona uma loja nova
        Lists.Structures.Shop Shop = new Lists.Structures.Shop(Guid.NewGuid());
        Shop.Name = "New shop";
        Lists.Shop.Add(Shop.ID, Shop);

        // Adiciona na lista
        TreeNode Node = new TreeNode(Shop.Name);
        Node.Tag = Shop.ID;
        List.Nodes.Add(Node);
        List.SelectedNode = Node;
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
        // Salva a dimensão da estrutura
        Send.Write_Shops();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Limpa os dados
        Lists.Shop = null;

        // Volta ao menu
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Selected.Name = txtName.Text;
        List.SelectedNode.Text = txtName.Text;
    }

    private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Currency = (short)cmbCurrency.SelectedIndex;
    }

    private void butSold_Add_Click(object sender, EventArgs e)
    {
        // Abre o painel para adicionar o item
        cmbItems.SelectedIndex = 0;
        numAmount.Value = 1;
        numAmount.Enabled = true;
        numPrice.Value = 0;
        grpAddItem.Tag = lstSold;
        grpAddItem.Visible = true;
        grpAddItem.BringToFront();
    }

    private void butSold_Remove_Click(object sender, EventArgs e)
    {
        // Remove o item
        if (lstSold.SelectedIndex >= 0)
        {
            Selected.Sold.RemoveAt(lstSold.SelectedIndex);
            lstSold.Items.RemoveAt(lstSold.SelectedIndex);
        }
    }

    private void butBought_Add_Click(object sender, EventArgs e)
    {
        // Abre o painel para adicionar o item
        cmbItems.SelectedIndex = 0;
        numAmount.Value = 1;
        numAmount.Enabled = false;
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
            Selected.Bought.RemoveAt(lstSold.SelectedIndex);
            lstBought.Items.RemoveAt(lstSold.SelectedIndex);
        }
    }

    private void butConfirm_Click(object sender, EventArgs e)
    {
        // Adiciona o item
        Lists.Structures.Shop_Item Data = new Lists.Structures.Shop_Item((short)(cmbItems.SelectedIndex + 1), (short)numAmount.Value, (short)numPrice.Value);
        if (grpAddItem.Tag == lstSold)
        {
            Selected.Sold.Add(Data);
            lstSold.Items.Add(List_Text(Data));
        }
        else
        {
            Selected.Bought.Add(Data);
            lstBought.Items.Add(List_Text(Data));
        }

        // Fecha o painel
        grpAddItem.Visible = false;
    }

    private string List_Text(Lists.Structures.Shop_Item Data) => Lists.Item[Data.Item_Num].Name + " - " + Data.Amount + "x [$" + Data.Price + "]";
}