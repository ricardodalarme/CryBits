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

    public static void Request()
    {
        // Lê os dados
        Send.Request_Items();
        Send.Request_NPCs(true);
    }

    public static void Open()
    {
        // Lista de itens
        Objects.cmbDrop_Item.Items.Clear();
        for (byte i = 1; i < Lists.Item.Length; i++) Objects.cmbDrop_Item.Items.Add(Lists.Item[i].Name);

        // Define os limites
        Objects.numTexture.Maximum = Graphics.Tex_Character.GetUpperBound(0);
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
        for (byte i = 1; i < Lists.NPC.Length; i++)
            Objects.List.Items.Add(Globals.Numbering(i, Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[i].Name);

        // Seleciona o primeiro item
        Objects.List.SelectedIndex = 0;
    }

    private void Update_Data()
    {
        Selected = (byte)(List.SelectedIndex + 1);

        // Previne erros
        if (Selected == 0) return;

        // Reseta os dados necessários
        lstDrop.Items.Clear();
        grpDrop_Add.Visible = false;
        lstAllies.Items.Clear();
        grpAllie_Add.Visible = false;

        // Remove o NPC dos aliados caso ele não existir
        for (byte i = 0; i < Lists.NPC[Selected].Allie.Count; i++)
            if (Lists.NPC[Selected].Allie[i] >= Lists.NPC.Length)
                Lists.NPC[Selected].Allie.RemoveAt(i);

        // Lista os dados
        txtName.Text = Lists.NPC[Selected].Name;
        txtSayMsg.Text = Lists.NPC[Selected].SayMsg;
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
        for (byte i = 0; i < Lists.NPC[Selected].Drop.Count; i++) lstDrop.Items.Add(Drop_String(Lists.NPC[Selected].Drop[i]));
        cmbMovement.SelectedIndex = (byte)Lists.NPC[Selected].Movement;
        numFlee_Health.Value = Lists.NPC[Selected].Flee_Helth;
        for (byte i = 0; i < Lists.NPC[Selected].Allie.Count; i++) lstAllies.Items.Add(Globals.Numbering(Lists.NPC[Selected].Allie[i], Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[Lists.NPC[Selected].Allie[i]].Name);

        // Seleciona os primeiros itens
        if (lstDrop.Items.Count > 0) lstDrop.SelectedIndex = 0;
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

    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Update_Data();
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados
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
            // Edita o nome
            Lists.NPC[Selected].Name = txtName.Text;
            List.Items[Selected - 1] = Globals.Numbering(Selected, List.Items.Count) + ":" + txtName.Text;

            // Altera o nome na lista de aliados
            short Position = (short)(Lists.NPC[Selected].Allie.Find(x => x == Selected) - 1);
            if (Position != -1)
            {
                lstAllies.Items.Insert(Position, Globals.Numbering(Lists.NPC[Selected].Allie[Position], List.Items.Count) + ":" + Lists.NPC[Lists.NPC[Selected].Allie[Position]].Name);
                lstAllies.Items.RemoveAt(Position);
            }
        }
    }

    private void txtSayMsg_Validated(object sender, EventArgs e)
    {
        Lists.NPC[Selected].SayMsg = txtSayMsg.Text;
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

    private void butDrop_Add_Click(object sender, EventArgs e)
    {
        // Reseta os valores e abre a janela para adicionar o item
        cmbDrop_Item.SelectedIndex = 0;
        numDrop_Amount.Value = 1;
        numDrop_Chance.Value = 100;
        grpDrop_Add.Visible = true;
    }

    private void butDrop_Delete_Click(object sender, EventArgs e)
    {
        // Deleta a item
        short Selected_Item = (short)lstDrop.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstDrop.Items.RemoveAt(Selected_Item);
            Lists.NPC[Selected].Drop.RemoveAt(Selected_Item);
        }
    }

    private void butItem_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o item
        if (cmbDrop_Item.SelectedIndex >= 0)
        {
            Lists.Structures.NPC_Drop Drop = new Lists.Structures.NPC_Drop((short)(cmbDrop_Item.SelectedIndex + 1), (short)numDrop_Amount.Value, (byte)numDrop_Chance.Value);
            Lists.NPC[Selected].Drop.Add(Drop);
            lstDrop.Items.Add(Drop_String(Drop));
            grpDrop_Add.Visible = false;
        }
    }

    private string Drop_String(Lists.Structures.NPC_Drop Drop)
    {
        return Globals.Numbering(Drop.Item_Num, Lists.Item.GetUpperBound(0)) + ":" + Lists.Item[Drop.Item_Num].Name + " [" + Drop.Amount + "x, " + Drop.Chance + "%]";
    }

    private void chkAttackNPC_CheckedChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].AttackNPC = chkAttackNPC.Checked;
        lstAllies.Enabled = chkAttackNPC.Checked;
    }

    private void butAllie_Add_Click(object sender, EventArgs e)
    {
        // Adiciona os NPCs
        cmbAllie_NPC.Items.Clear();
        for (short i = 1; i < Lists.NPC.Length; i++) cmbAllie_NPC.Items.Add(Globals.Numbering(i, Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[i].Name);
        cmbAllie_NPC.SelectedIndex = 0;

        // Abre a janela para adicionar o aliado
        grpAllie_Add.Visible = true;
    }

    private void butAllie_Delete_Click(object sender, EventArgs e)
    {
        // Deleta a aliado
        short Selected_Item = (short)lstAllies.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstAllies.Items.RemoveAt(Selected_Item);
            Lists.NPC[Selected].Allie.RemoveAt(Selected_Item);
        }
    }

    private void butAllie_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o aliado
        if (cmbAllie_NPC.SelectedIndex >= 0)
        {
            Lists.NPC[Selected].Allie.Add((short)(cmbAllie_NPC.SelectedIndex + 1));
            lstAllies.Items.Add(Globals.Numbering(cmbAllie_NPC.SelectedIndex + 1, Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[cmbAllie_NPC.SelectedIndex + 1].Name);
            grpAllie_Add.Visible = false;
        }
    }

    private void cmbMovement_SelectedIndexChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Movement = (Globals.NPC_Movements)cmbMovement.SelectedIndex;
    }

    private void numFlee_Health_ValueChanged(object sender, EventArgs e)
    {
        Lists.NPC[Selected].Flee_Helth = (byte)numFlee_Health.Value;
    }
}