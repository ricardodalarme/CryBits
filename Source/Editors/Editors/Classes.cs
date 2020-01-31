using System;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class Editor_Classes : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Classes Objects = new Editor_Classes();

    // Índice do item selecionado
    public byte Selected;

    public Editor_Classes()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Lê os dados
        Send.Request_Items();
        Send.Request_Classes(true);
    }

    public static void Open()
    {
        // Lista de itens
        Objects.cmbItems.Items.Clear();
        for (byte i = 1; i < Lists.Item.Length; i++) Objects.cmbItems.Items.Add(Globals.Numbering(i, Lists.Item.GetUpperBound(0)) + ":" + Lists.Item[i].Name);

        // Lista os dados
        List_Update();

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private static void List_Update()
    {
        // Limpa as listas
        Objects.List.Items.Clear();

        // Adiciona os itens às listas
        for (byte i = 1; i < Lists.Class.Length; i++)
        {
            string Text = Globals.Numbering(i, Lists.Class.GetUpperBound(0)) + ":" + Lists.Class[i].Name;
            Objects.List.Items.Add(Text);
        }

        // Seleciona os primeiros itens
        Objects.List.SelectedIndex = 0;
    }

    private void Update_Data()
    {
        Selected = (byte)(List.SelectedIndex + 1);

        // Previne erros
        if (Selected == 0) return;

        // Limpa os dados necessários
        lstMale.Items.Clear();
        lstFemale.Items.Clear();
        lstItems.Items.Clear();

        // Lista os dados
        txtName.Text = Lists.Class[Selected].Name;
        txtDescription.Text = Lists.Class[Selected].Description;
        numHP.Value = Lists.Class[Selected].Vital[(byte)Globals.Vitals.HP];
        numMP.Value = Lists.Class[Selected].Vital[(byte)Globals.Vitals.MP];
        numStrength.Value = Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Strength];
        numResistance.Value = Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Resistance];
        numIntelligence.Value = Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Intelligence];
        numAgility.Value = Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Agility];
        numVitality.Value = Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Vitality];
        numSpawn_Map.Value = Lists.Class[Selected].Spawn_Map;
        cmbSpawn_Direction.SelectedIndex = Lists.Class[Selected].Spawn_Direction;
        numSpawn_X.Value = Lists.Class[Selected].Spawn_X;
        numSpawn_Y.Value = Lists.Class[Selected].Spawn_Y;
        for (byte i = 0; i < Lists.Class[Selected].Tex_Male.Count; i++) lstMale.Items.Add(Lists.Class[Selected].Tex_Male[i]);
        for (byte i = 0; i < Lists.Class[Selected].Tex_Female.Count; i++) lstFemale.Items.Add(Lists.Class[Selected].Tex_Female[i]);
        for (byte i = 0; i < Lists.Class[Selected].Item.Count; i++) lstItems.Items.Add(Globals.Numbering(Lists.Class[Selected].Item[i], Lists.Item.GetUpperBound(0)) + ":" + Lists.Item[Lists.Class[Selected].Item[i]].Name);

        // Seleciona os primeiros itens
        if (lstMale.Items.Count > 0) lstMale.SelectedIndex = 0;
        if (lstFemale.Items.Count > 0) lstFemale.SelectedIndex = 0;
    }

    public static void Change_Quantity()
    {
        int Quantity = (int)Editor_Quantity.Objects.numQuantity.Value;
        int Old = Lists.Class.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.Class, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.Class(i);

        List_Update();
    }

    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Update_Data();
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva a dimensão da estrutura
        Send.Write_Classes();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butClear_Click(object sender, EventArgs e)
    {
        // Limpa os dados
        Clear.Class(Selected);

        // Atualiza os valores
        List.Items[Selected - 1] = Globals.Numbering(Selected, List.Items.Count) + ":";
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
        Editor_Quantity.Open(Lists.Class.GetUpperBound(0));
    }

    private void txtName_Validated(object sender, EventArgs e)
    {
        // Atualiza a lista
        if (Selected > 0)
        {
            Lists.Class[Selected].Name = txtName.Text;
            string Text = Globals.Numbering(Selected, List.Items.Count) + ":" + txtName.Text;
            List.Items[Selected - 1] = Text;
        }
    }

    private void numHP_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Vital[(byte)Globals.Vitals.HP] = (short)numHP.Value;
    }

    private void numMP_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Vital[(byte)Globals.Vitals.MP] = (short)numMP.Value;
    }

    private void numStrength_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Strength] = (short)numStrength.Value;
    }

    private void numResistance_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Resistance] = (short)numResistance.Value;
    }

    private void numIntelligence_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Intelligence] = (short)numIntelligence.Value;
    }

    private void numAgility_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Agility] = (short)numAgility.Value;
    }

    private void numVitality_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Vitality] = (short)numVitality.Value;
    }

    private void butMTexture_Click(object sender, EventArgs e)
    {
        // Adiciona a textura
        short Texture_Num = Preview.Select(Graphics.Tex_Character, 0);
        if (Texture_Num != 0)
        {
            Lists.Class[Selected].Tex_Male.Add(Texture_Num);
            lstMale.Items.Add(Texture_Num);
        }
    }

    private void butFTexture_Click(object sender, EventArgs e)
    {
        // Adiciona a textura
        short Texture_Num = Preview.Select(Graphics.Tex_Character, 0);
        if (Texture_Num != 0)
        {
            Lists.Class[Selected].Tex_Female.Add(Texture_Num);
            lstFemale.Items.Add(Texture_Num);
        }
    }

    private void butMDelete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstMale.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstMale.Items.RemoveAt(Selected_Item);
            Lists.Class[Selected].Tex_Male.RemoveAt(Selected_Item);
        }
    }

    private void butFDelete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstFemale.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstFemale.Items.RemoveAt(Selected_Item);
            Lists.Class[Selected].Tex_Female.RemoveAt(Selected_Item);
        }
    }

    private void numSpawn_Map_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Spawn_Map = (short)numSpawn_Map.Value;
    }

    private void cmbSpawn_Direction_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Spawn_Direction = (byte)cmbSpawn_Direction.SelectedIndex;
    }

    private void numSpawn_X_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Spawn_X = (byte)numSpawn_X.Value;
    }

    private void numSpawn_Y_ValueChanged(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Spawn_Y = (byte)numSpawn_Y.Value;
    }

    private void txtDescription_Validated(object sender, EventArgs e)
    {
        // Define o valor
        Lists.Class[Selected].Description = txtDescription.Text;
    }

    private void butItem_Add_Click(object sender, EventArgs e)
    {
        // Abre a janela para adicionar o item
        cmbItems.SelectedIndex = 0;
        grpItem_Add.Visible = true;
    }

    private void butItem_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o item
        if (cmbItems.SelectedIndex >= 0)
        {
            lstItems.Items.Add(Globals.Numbering(cmbItems.SelectedIndex + 1, Lists.Item.GetUpperBound(0)) + ":" + Lists.Item[cmbItems.SelectedIndex + 1].Name);
            Lists.Class[Selected].Item.Add((byte)(cmbItems.SelectedIndex + 1));
            grpItem_Add.Visible = false;
        }
    }

    private void butItem_Delete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstItems.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstItems.Items.RemoveAt(Selected_Item);
            Lists.Class[Selected].Item.RemoveAt(Selected_Item);
        }
    }
}