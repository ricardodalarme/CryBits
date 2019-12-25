using System;
using System.Windows.Forms;

public partial class Editor_Classes : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Classes Objects = new Editor_Classes();

    // Index do item selecionado
    public byte Selected;

    public Editor_Classes()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Lê os dados
        Send.Request_Classes(true);
    }

    public static void Open()
    {
        // Lista os dados
        List_Update();

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private static void List_Update()
    {
        // Limpa a lista
        Objects.List.Items.Clear();

        // Adiciona os itens à lista
        for (byte i = 1; i <= Lists.Class.GetUpperBound(0); i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.Class.GetUpperBound(0)) + ":" + Lists.Class[i].Name);

        // Seleciona o primeiro item
        Objects.List.SelectedIndex = 0;
    }

    private void Update_Data()
    {
        Selected = (byte)(List.SelectedIndex + 1);

        // Previne erros
        if (Selected == 0) return;

        // Lista os dados
        txtName.Text = Lists.Class[Selected].Name;
        lblMTexture.Text = "Male: " + Lists.Class[Selected].Texture_Male;
        lblFTexture.Text = "Female: " + Lists.Class[Selected].Texture_Female;
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

    #region 
    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Update_Data();
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva a dimensão da estrutura
        Lists.Server_Data.Num_Classes = (byte)Lists.Class.GetUpperBound(0);
        Send.Write_Server_Data();
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
            List.Items[Selected - 1] = Globals.Numbering(Selected, List.Items.Count) + ":" + txtName.Text;
        }
    }

    private void numHP_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Vital[(byte)Globals.Vitals.HP] = (short)numHP.Value;
    }

    private void numMP_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Vital[(byte)Globals.Vitals.MP] = (short)numMP.Value;
    }

    private void numStrength_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Strength] = (short)numStrength.Value;
    }

    private void numResistance_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Resistance] = (short)numResistance.Value;
    }

    private void numIntelligence_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Intelligence] = (short)numIntelligence.Value;
    }

    private void numAgility_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Agility] = (short)numAgility.Value;
    }

    private void numVitality_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Attribute[(byte)Globals.Attributes.Vitality] = (short)numVitality.Value;
    }

    private void butMTexture_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        Lists.Class[Selected].Texture_Male = Preview.Select(Graphics.Tex_Character, Lists.Class[Selected].Texture_Male);
        lblMTexture.Text = "Male: " + Lists.Class[Selected].Texture_Male;
    }

    private void butFTexture_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        Lists.Class[Selected].Texture_Female = Preview.Select(Graphics.Tex_Character, Lists.Class[Selected].Texture_Female);
        lblFTexture.Text = "Female: " + Lists.Class[Selected].Texture_Female;
    }

    private void numSpawn_Map_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Spawn_Map = (short)numSpawn_Map.Value;
    }

    private void cmbSpawn_Direction_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Spawn_Direction = (byte)cmbSpawn_Direction.SelectedIndex;
    }

    private void numSpawn_X_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Spawn_X = (byte)numSpawn_X.Value;
    }

    private void numSpawn_Y_ValueChanged(object sender, EventArgs e)
    {
        // Define os valores
        Lists.Class[Selected].Spawn_Y = (byte)numSpawn_Y.Value;
    }
    #endregion
}