using System;
using System.Windows.Forms;

public partial class Editor_Tools : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Tools Objects = new Editor_Tools();

    // Index do item selecionado
    public static byte Selected;

    public Editor_Tools()
    {
        InitializeComponent();
    }

    public static void Open()
    {
        // Lê os dados
        Read.Tools();

        // Define os limites
        Objects.numX.Maximum = (Globals.Min_Map_Width + 1) * Globals.Grid;
        Objects.numY.Maximum = (Globals.Min_Map_Height + 1) * Globals.Grid;

        // Adiciona os tipos de ferramentas à lista
        Objects.cmbTools.Items.Clear();

        for (byte i = 0; i < (byte)Globals.Tools_Types.Amount; i++)
            Objects.cmbTools.Items.Add((Globals.Tools_Types)i);

        Objects.cmbTools.SelectedIndex = 0;

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private void cmbFerramentas_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        List_Update();
    }

    #region "List"
    private static void List_Buttons()
    {
        // Adiciona os itens à lista
        for (byte i = 1; i <= Lists.Button.GetUpperBound(0); i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.Button.GetUpperBound(0)) + ":" + Lists.Button[i].Name);

        // Abre o painel
        Objects.panButton.Visible = true;
    }

    private static void List_TextBoxes()
    {
        // Adiciona os itens à lista
        for (byte i = 1; i <= Lists.TextBox.GetUpperBound(0); i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.TextBox.GetUpperBound(0)) + ":" + Lists.TextBox[i].Name);

        // Abre o painel
        Objects.panTextBox.Visible = true;
    }

    private static void List_Panels()
    {
        // Adiciona os itens à lista
        for (byte i = 1; i <= Lists.Panel.GetUpperBound(0); i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.Panel.GetUpperBound(0)) + ":" + Lists.Panel[i].Name);

        // Abre o painel
        Objects.panPanel.Visible = true;
    }

    private static void List_CheckBoxes()
    {
        // Adiciona os itens à lista
        for (byte i = 1; i <= Lists.CheckBox.GetUpperBound(0); i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.CheckBox.GetUpperBound(0)) + ":" + Lists.CheckBox[i].Name);

        // Abre o painel
        Objects.panCheckBox.Visible = true;
    }
    #endregion

    private static void List_Update()
    {
        Selected = (byte)(Objects.List.SelectedIndex + 1);

        // Limpa a lista
        Objects.List.Items.Clear();

        // Fehca todos os paineis
        Objects.panTextBox.Visible = false;
        Objects.panButton.Visible = false;
        Objects.panPanel.Visible = false;
        Objects.panCheckBox.Visible = false;

        // Lista as ferramentas e suas propriedades
        switch ((Globals.Tools_Types)Objects.cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: List_Buttons(); break;
            case Globals.Tools_Types.TextBox: List_TextBoxes(); break;
            case Globals.Tools_Types.Panel: List_Panels(); break;
            case Globals.Tools_Types.CheckBox: List_CheckBoxes(); break;
        }

        // Seleciona o primeiro item
        if (Objects.List.Items.Count != 0) Objects.List.SelectedIndex = 0;
    }

    #region "Update"
    private void Update_Button()
    {
        // Lista as propriedades do botão
        txtName.Text = Lists.Button[Selected].Name;
        numX.Value = Lists.Button[Selected].Position.X;
        numY.Value = Lists.Button[Selected].Position.Y;
        chkVisible.Checked = Lists.Button[Selected].Visible;
        lblButton_Texture.Text = "Texture: " + Lists.Button[Selected].Texture;
    }

    private void Update_TextBox()
    {
        // Lista as propriedades do digitalizador
        txtName.Text = Lists.TextBox[Selected].Name;
        numX.Value = Lists.TextBox[Selected].Position.X;
        numY.Value = Lists.TextBox[Selected].Position.Y;
        chkVisible.Checked = Lists.TextBox[Selected].Visible;
        scrlTextBox_Max_Characters.Value = Lists.TextBox[Selected].Max_Chars;
        scrlTextBox_Width.Value = Lists.TextBox[Selected].Width;
    }

    private void Update_CheckBox()
    {
        // Lista as propriedades do marcador
        txtName.Text = Lists.CheckBox[Selected].Name;
        numX.Value = Lists.CheckBox[Selected].Position.X;
        numY.Value = Lists.CheckBox[Selected].Position.Y;
        chkVisible.Checked = Lists.CheckBox[Selected].Visible;
        txtCheckBox_Text.Text = Lists.CheckBox[Selected].Text;
    }

    private void Update_Panel()
    {
        // Lista as propriedades do painel
        txtName.Text = Lists.Panel[Selected].Name;
        numX.Value = Lists.Panel[Selected].Position.X;
        numY.Value = Lists.Panel[Selected].Position.Y;
        chkVisible.Checked = Lists.Panel[Selected].Visible;
        lblPanel_Texture.Text = "Texture: " + Lists.Panel[Selected].Texture;
    }

    private void Update_Data()
    {
        Selected = (byte)(List.SelectedIndex + 1);

        // Previne erros
        if (Selected == 0) return;

        // Lista as ferramentas e suas propriedades
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Update_Button(); break;
            case Globals.Tools_Types.TextBox: Update_TextBox(); break;
            case Globals.Tools_Types.Panel: Update_Panel(); break;
            case Globals.Tools_Types.CheckBox: Update_CheckBox(); break;
        }
    }
    #endregion

    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Update_Data();
    }

    #region "Change_Quantity"
    private static void Change_Quantity_Button()
    {
        byte Quantity = (byte)Editor_Quantity.Objects.numQuantity.Value;
        byte Old = (byte)Lists.Button.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.Button, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.Button(i);
    }

    private static void Change_Quantity_TextBox()
    {
        byte Quantity = (byte)Editor_Quantity.Objects.numQuantity.Value;
        byte Old = (byte)Lists.TextBox.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.TextBox, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.TextBox(i);
    }

    private static void Change_Quantity_CheckBox()
    {
        byte Quantity = (byte)Editor_Quantity.Objects.numQuantity.Value;
        byte Old = (byte)Lists.CheckBox.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.CheckBox, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.CheckBox(i);
    }

    private static void Change_Quantity_Panel()
    {
        byte Quantity = (byte)Editor_Quantity.Objects.numQuantity.Value;
        byte Old = (byte)Lists.Panel.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.Panel, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.Panel(i);
    }
    #endregion

    public static void Change_Quantity()
    {
        // Altera a quantidade de ferramentas
        switch ((Globals.Tools_Types)Objects.cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Change_Quantity_Button(); break;
            case Globals.Tools_Types.TextBox: Change_Quantity_TextBox(); break;
            case Globals.Tools_Types.Panel: Change_Quantity_Panel(); break;
            case Globals.Tools_Types.CheckBox: Change_Quantity_CheckBox(); break;
        }

        List_Update();
    }

    private void butQuantity_Click(object sender, EventArgs e)
    {
        // Abre a janela de alteração de quantidade
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Editor_Quantity.Open(Lists.Button.GetUpperBound(0)); break;
            case Globals.Tools_Types.TextBox: Editor_Quantity.Open(Lists.TextBox.GetUpperBound(0)); break;
            case Globals.Tools_Types.Panel: Editor_Quantity.Open(Lists.Panel.GetUpperBound(0)); break;
            case Globals.Tools_Types.CheckBox: Editor_Quantity.Open(Lists.CheckBox.GetUpperBound(0)); break;
        }
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva a dimensão da estrutura
        Lists.Client_Data.Num_Buttons = (byte)Lists.Button.GetUpperBound(0);
        Lists.Client_Data.Num_TextBoxes = (byte)Lists.TextBox.GetUpperBound(0);
        Lists.Client_Data.Num_Panels = (byte)Lists.Panel.GetUpperBound(0);
        Lists.Client_Data.Num_CheckBoxes = (byte)Lists.CheckBox.GetUpperBound(0);
        Write.Client_Data();
        Write.Tools();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butClear_Click(object sender, EventArgs e)
    {
        // Reseta os valores
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Clear.Button(Selected); break;
            case Globals.Tools_Types.TextBox: Clear.TextBox(Selected); break;
            case Globals.Tools_Types.Panel: Clear.Panel(Selected); break;
            case Globals.Tools_Types.CheckBox: Clear.CheckBox(Selected); break;
        }

        Update_Data();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta ao menu
        Visible = false;
        Selection.Objects.Visible = true;
    }

    #region "Values"
    private void txtName_Validated(object sender, EventArgs e)
    {
        // Previne erros
        if (Selected == 0) return;

        // Define os valores
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Lists.Button[Selected].Name = txtName.Text; break;
            case Globals.Tools_Types.TextBox: Lists.TextBox[Selected].Name = txtName.Text; break;
            case Globals.Tools_Types.CheckBox: Lists.CheckBox[Selected].Name = txtName.Text; break;
            case Globals.Tools_Types.Panel: Lists.Panel[Selected].Name = txtName.Text; break;
        }

        List.Items[Selected - 1] = Globals.Numbering(Selected, List.Items.Count) + ":" + txtName.Text;
    }

    private void numX_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Lists.Button[Selected].Position.X = (short)numX.Value; break;
            case Globals.Tools_Types.TextBox: Lists.TextBox[Selected].Position.X = (short)numX.Value; break;
            case Globals.Tools_Types.CheckBox: Lists.CheckBox[Selected].Position.X = (short)numX.Value; break;
            case Globals.Tools_Types.Panel: Lists.Panel[Selected].Position.X = (short)numX.Value; break;
        }
    }

    private void numY_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Lists.Button[Selected].Position.Y = (short)numY.Value; break;
            case Globals.Tools_Types.TextBox: Lists.TextBox[Selected].Position.Y = (short)numY.Value; break;
            case Globals.Tools_Types.CheckBox: Lists.CheckBox[Selected].Position.Y = (short)numY.Value; break;
            case Globals.Tools_Types.Panel: Lists.Panel[Selected].Position.Y = (short)numY.Value; break;
        }
    }

    private void chkVisible_CheckedChanged(object sender, EventArgs e)
    {
        // Define os valores
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Lists.Button[Selected].Visible = chkVisible.Checked; break;
            case Globals.Tools_Types.TextBox: Lists.TextBox[Selected].Visible = chkVisible.Checked; break;
            case Globals.Tools_Types.CheckBox: Lists.CheckBox[Selected].Visible = chkVisible.Checked; break;
            case Globals.Tools_Types.Panel: Lists.Panel[Selected].Visible = chkVisible.Checked; break;
        }
    }

    private void scrlTextBox_Max_Characters_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        if (scrlTextBox_Max_Characters.Value > 0)
            lblTextBox_Max_Characters.Text = "Maximum characters: " + scrlTextBox_Max_Characters.Value;
        else
            lblTextBox_Max_Characters.Text = "Maximum characters: Infinity";

        Lists.TextBox[Selected].Max_Chars = (short)scrlTextBox_Max_Characters.Value;
    }

    private void scrlTextBox_Width_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        lblTextBox_Width.Text = "Width: " + scrlTextBox_Width.Value;
        Lists.TextBox[Selected].Width = (short)scrlTextBox_Width.Value;
    }

    private void chkTextBox_Password_CheckedChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.TextBox[Selected].Password = chkTextBox_Password.Checked;
    }

    private void txtCheckBox_Text_Validated(object sender, EventArgs e)
    {
        // Define os valores
        Lists.CheckBox[Selected].Text = txtCheckBox_Text.Text;
    }

    private void butPanel_Texture_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        Lists.Panel[Selected].Texture = (byte)Preview.Select(Graphics.Tex_Panel, Lists.Panel[Selected].Texture);
        lblPanel_Texture.Text = "Texture: " + Lists.Panel[Selected].Texture;
    }

    private void butButton_Texture_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        Lists.Button[Selected].Texture = (byte)Preview.Select(Graphics.Tex_Button, Lists.Button[Selected].Texture);
        lblButton_Texture.Text = "Texture: " + Lists.Button[Selected].Texture;
    }
    #endregion
}