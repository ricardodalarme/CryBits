using System;
using System.Windows.Forms;

public partial class Editor_NPCs : Form
{
    // Usado para acessar os dados da janela
    public static Editor_NPCs Objects = new Editor_NPCs();

    // Index do item selecionado
    public byte Selected;

    public Editor_NPCs()
    {
        InitializeComponent();
    }

    public static void Open()
    {
        // Lê os dados  e lista os itens
        Send.Request_NPCs();
        Send.Request_Items();

        // Lista de itens
        Objects.cmbDrop_Item.Items.Clear();
        Objects.cmbDrop_Item.Items.Add("None");
        for (byte i = 1; i <= Lists.Item.GetUpperBound(0); i++) Objects.cmbDrop_Item.Items.Add(Lists.Item[i].Name);

        // Define os limites
        Objects.numTexture.Maximum = Graphics.Tex_Character.GetUpperBound(0);
        Objects.scrlDrop.Maximum = Globals.Max_NPC_Drop - 1;
        Update_List();

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private static void Update_List()
    {
        // Limpa a lista
        Objects.List.Items.Clear();

        // Adiciona os itens à lista
        for (byte i = 1; i <= Lists.NPC.GetUpperBound(0); i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[i].Name);

        // Seleciona o primeiro item
        Objects.List.SelectedIndex = 0;
    }

    private void Update_Data()
    {
        Selected = (byte)(List.SelectedIndex + 1);

        // Previne erros
        if (Selected == 0) return;

        // Lista os dados
        txtName.Text = Lists.NPC[Selected].Name;
        numTexture.Value = Lists.NPC[Selected].Texture;
        cmbBehavior.SelectedIndex = Lists.NPC[Selected].Behaviour;
        numSpawn.Value = Lists.NPC[Selected].SpawnTime;
        numRange.Value = Lists.NPC[Selected].Sight;
        numExperience.Value = Lists.NPC[Selected].Experience;
        numHP.Value = Lists.NPC[Selected].Vital[(byte)Globals.Vitals.HP];
        numMP.Value = Lists.NPC[Selected].Vital[(byte)Globals.Vitals.MP];
        numStrength.Value = Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Strength];
        numResistance.Value = Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Resistance];
        numIntelligence.Value = Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Intelligence];
        numAgility.Value = Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Agility];
        numVitality.Value = Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Vitality];
        if (cmbDrop_Item.Items.Count > 0) cmbDrop_Item.SelectedIndex = Lists.NPC[Selected].Drop[scrlDrop.Value].Item_Num;
        cmbDrop_Amount.Value = Lists.NPC[Selected].Drop[scrlDrop.Value].Amount;
        numDrop_Chance.Value = Lists.NPC[Selected].Drop[scrlDrop.Value].Chance;
    }

    public static void Change_Quantity()
    {
        int Quantity = (int)Editor_Quantity.Objects.numQuantity.Value;
        int Old = Lists.NPC.GetUpperBound(0);

        // Altera a quantidade de itens
        Array.Resize(ref Lists.NPC, Quantity + 1);

        // Limpa os novos itens
        if (Quantity > Old)
            for (byte i = (byte)(Old + 1); i <= Quantity; i++)
                Clear.NPC(i);

        Update_List();
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
        Lists.Server_Data.Num_NPCs = (byte)Lists.NPC.GetUpperBound(0);
        Send.Write_Server_Data();
        Send.Write_NPCs();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butClear_Click(object sender, EventArgs e)
    {
        // Limpa os dados
        Clear.NPC(Selected);

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
        Editor_Quantity.Open(Lists.NPC.GetUpperBound(0));
    }

    private void txtName_Validated(object sender, EventArgs e)
    {
        // Atualiza a lista
        if (Selected > 0)
        {
            Lists.NPC[Selected].Name = txtName.Text;
            List.Items[Selected - 1] = Globals.Numbering(Selected, List.Items.Count) + ":" + txtName.Text;
        }
    }

    private void butTexture_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        Lists.NPC[Selected].Texture = Preview.Select(Graphics.Tex_Character, Lists.NPC[Selected].Texture);
        numTexture.Value = Lists.NPC[Selected].Texture;
    }

    private void numTexture_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Texture = (byte)numTexture.Value;
    }

    private void numRange_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Sight = (byte)numRange.Value;
    }

    private void cmbBehavior_SelectedIndexChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Behaviour = (byte)cmbBehavior.SelectedIndex;
    }

    private void numHP_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Vital[(byte)Globals.Vitals.HP] = (short)numHP.Value;
    }

    private void numMP_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Vital[(byte)Globals.Vitals.MP] = (short)numMP.Value;
    }

    private void numStrength_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Strength] = (short)numStrength.Value;
    }

    private void numResistance_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Resistance] = (short)numResistance.Value;
    }

    private void numIntelligence_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Intelligence] = (short)numIntelligence.Value;
    }

    private void numAgility_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Agility] = (short)numAgility.Value;
    }

    private void numVitality_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Attribute[(byte)Globals.Attributes.Vitality] = (short)numVitality.Value;
    }

    private void numSpawn_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].SpawnTime = (byte)numSpawn.Value;
    }

    private void numExperience_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Experience = (byte)numExperience.Value;
    }

    private void scrlDrop_ValueChanged(object sender, EventArgs e)
    {
        // Previne erros
        if (Selected <= 0) return;

        // Atualiza os valores
        grpDrop.Text = "Drop - " + (scrlDrop.Value + 1);
        cmbDrop_Item.SelectedIndex = Lists.NPC[Selected].Drop[scrlDrop.Value].Item_Num;
        cmbDrop_Amount.Value = Lists.NPC[Selected].Drop[scrlDrop.Value].Amount;
        numDrop_Chance.Value = Lists.NPC[Selected].Drop[scrlDrop.Value].Chance;
    }

    private void cmbDrop_Item_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Selected > 0) Lists.NPC[Selected].Drop[scrlDrop.Value].Item_Num = (short)cmbDrop_Item.SelectedIndex;
    }

    private void cmbDrop_Amount_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Drop[scrlDrop.Value].Amount = (short)cmbDrop_Amount.Value;
    }

    private void numDrop_Chance_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Drop[scrlDrop.Value].Chance = (byte)numDrop_Chance.Value;
    }
    #endregion
}