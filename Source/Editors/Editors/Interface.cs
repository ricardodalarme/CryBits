using System;
using System.Windows.Forms;
using System.ComponentModel;

public partial class Editor_Interface : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Interface Objects = new Editor_Interface();

    // Index do item selecionado
    public static byte Selected;

    public Editor_Interface()
    {
        InitializeComponent();
    }

    public static void Open()
    {
        // Lê os dados
        Read.Tools();

        // Adiciona os tipos de ferramentas à lista
        Objects.cmbTools.Items.Clear();
        for (byte i = 0; i < (byte)Globals.Tools_Types.Amount; i++)  Objects.cmbTools.Items.Add((Globals.Tools_Types)i);
        Objects.cmbTools.SelectedIndex = 0;

        // Adiciona as janelas à lista
        Objects.cmbWIndows.Items.AddRange(Enum.GetNames(typeof(Globals.Windows)));
        Objects.cmbWIndows.SelectedIndex = 0;

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private void cmbTools_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        List_Update();
    }

    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected = (byte)(List.SelectedIndex + 1);

        // Previne erros
        if (Selected == 0) return;

        // Lista as ferramentas e suas propriedades
        switch ((Globals.Tools_Types)cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Objects.prgProperties.SelectedObject = Lists.Button[Selected]; break;
            case Globals.Tools_Types.TextBox: Objects.prgProperties.SelectedObject = Lists.TextBox[Selected]; break;
            case Globals.Tools_Types.Panel: Objects.prgProperties.SelectedObject = Lists.Panel[Selected]; break;
            case Globals.Tools_Types.CheckBox: Objects.prgProperties.SelectedObject = Lists.CheckBox[Selected]; break;
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

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta ao menu
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

        // Update_Data();
    }

    private static void List_Update()
    {
        Selected = (byte)(Objects.List.SelectedIndex + 1);

        // Limpa a lista
        Objects.List.Items.Clear();

        // Lista as ferramentas e suas propriedades
        switch ((Globals.Tools_Types)Objects.cmbTools.SelectedIndex)
        {
            case Globals.Tools_Types.Button: 
                for (byte i = 1; i < Lists.Button.Length; i++) Objects.List.Items.Add(Globals.Numbering(i, Lists.Button.GetUpperBound(0)) + ":" + Lists.Button[i].Name);
                break;
            case Globals.Tools_Types.TextBox: 
                for (byte i = 1; i < Lists.TextBox.Length; i++) Objects.List.Items.Add(Globals.Numbering(i, Lists.TextBox.GetUpperBound(0)) + ":" + Lists.TextBox[i].Name);
                break;
            case Globals.Tools_Types.Panel: 
                for (byte i = 1; i < Lists.Panel.Length; i++)  Objects.List.Items.Add(Globals.Numbering(i, Lists.Panel.GetUpperBound(0)) + ":" + Lists.Panel[i].Name); 
                break;
            case Globals.Tools_Types.CheckBox:
                for (byte i = 1; i < Lists.CheckBox.Length; i++) Objects.List.Items.Add(Globals.Numbering(i, Lists.CheckBox.GetUpperBound(0)) + ":" + Lists.CheckBox[i].Name);
                break;
        }

        // Seleciona o primeiro item
        if (Objects.List.Items.Count != 0) Objects.List.SelectedIndex = 0;
    }
}