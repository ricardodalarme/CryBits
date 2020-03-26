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
        Objects.cmbItems.Items.Clear();
        for (short i = 1; i < Lists.Item.Length; i++)
        {
            Objects.cmbCurrency.Items.Add(Lists.Item[i].Name);
            Objects.cmbItems.Items.Add(Lists.Item[i].Name);
        }

        // Lista os dados
        List_Update();

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private static void List_Update()
    {
        // Adiciona as classes às listas
        Objects.List.Items.Clear();
        for (byte i = 1; i < Lists.Shop.Length; i++) Objects.List.Items.Add(Globals.Numbering(i, Lists.Shop.GetUpperBound(0), Lists.Shop[i].Name));
        Objects.List.SelectedIndex = 0;
    }

    private void Update_Data()
    {
        // Previne erros
        if (List.SelectedIndex == -1) return;

        // Limpa os dados necessários
        lstSold.Items.Clear();
        lstBought.Items.Clear();
        grpAddItem.Visible = false;

        // Lista os dados
        txtName.Text = Selected.Name;
        cmbCurrency.SelectedIndex = Selected.Currency - 1;
        for (byte i = 0; i < Selected.Sold.Count; i++) lstSold.Items.Add(List_Text(Selected.Sold[i]));
        for (byte i = 0; i < Selected.Bought.Count; i++) lstBought.Items.Add(List_Text(Selected.Bought[i]));
    }

    public static void Change_Quantity()
    {
        int Quantity = (int)Editor_Quantity.Objects.numQuantity.Value;
        int Old = Lists.Shop.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.Shop, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.Shop(i);

        List_Update();
    }

    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Selected = Lists.Shop[List.SelectedIndex + 1];
        Update_Data();
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva a dimensão da estrutura
        Send.Write_Shops();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butClear_Click(object sender, EventArgs e)
    {
        // Limpa os dados
        Clear.Shop((byte)(List.SelectedIndex + 1));

        // Atualiza os valores
        List.Items[List.SelectedIndex] = Globals.Numbering(List.SelectedIndex + 1, List.Items.Count, string.Empty);
        Update_Data();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta ao menu
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butQuantity_Click(object sender, EventArgs e)
    {
        // Abre a janela de alteração
        Editor_Quantity.Open(Lists.Shop.GetUpperBound(0));
    }

    private void txtName_Validated(object sender, EventArgs e)
    {
        // Atualiza a lista
        Selected.Name = txtName.Text;
        List.Items[List.SelectedIndex] = Globals.Numbering(List.SelectedIndex + 1, List.Items.Count, txtName.Text);
    }

    private void cmbCurrency_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Currency = (short)(cmbCurrency.SelectedIndex + 1);
    }

    private void butSold_Add_Click(object sender, EventArgs e)
    {
        // Abre o painel para adicionar o item
        cmbItems.SelectedIndex = 0;
        numAmount.Value = 1;
        numPrice.Value = 0;
        grpAddItem.Tag = lstSold;
        grpAddItem.Visible = true;
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
        numPrice.Value = 0;
        grpAddItem.Tag = lstBought;
        grpAddItem.Visible = true;
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