using System;
using System.Windows.Forms;

public partial class Editor_NPCs : Form
{
    // Usado para acessar os dados da janela
    public static Editor_NPCs Objects = new Editor_NPCs();

    // Index do item selecionado
    private Lists.Structures.NPC Selected;

    public Editor_NPCs()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Lê os dados
        Globals.OpenEditor = Objects;
        Send.Request_Items();
        Send.Request_NPCs();
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
        // Adiciona os itens à lista
        Objects.List.Items.Clear();
        for (byte i = 1; i < Lists.NPC.Length; i++) Objects.List.Items.Add(Globals.Numbering(i, Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[i].Name);
        Objects.List.SelectedIndex = 0;
    }

    private void Update_Data()
    {
        // Previne erros
        if (List.SelectedIndex == -1) return;

        // Reseta os dados necessários
        lstDrop.Items.Clear();
        grpDrop_Add.Visible = false;
        lstAllies.Items.Clear();
        grpAllie_Add.Visible = false;

        // Remove o NPC dos aliados caso ele não existir
        for (byte i = 0; i < Selected.Allie.Count; i++)
            if (Selected.Allie[i] >= Lists.NPC.Length)
                Selected.Allie.RemoveAt(i);

        // Lista os dados
        txtName.Text = Selected.Name;
        txtSayMsg.Text = Selected.SayMsg;
        numTexture.Value = Selected.Texture;
        cmbBehavior.SelectedIndex = Selected.Behaviour;
        numSpawn.Value = Selected.SpawnTime;
        numRange.Value = Selected.Sight;
        numExperience.Value = Selected.Experience;
        numHP.Value = Selected.Vital[(byte)Globals.Vitals.HP];
        numMP.Value = Selected.Vital[(byte)Globals.Vitals.MP];
        numStrength.Value = Selected.Attribute[(byte)Globals.Attributes.Strength];
        numResistance.Value = Selected.Attribute[(byte)Globals.Attributes.Resistance];
        numIntelligence.Value = Selected.Attribute[(byte)Globals.Attributes.Intelligence];
        numAgility.Value = Selected.Attribute[(byte)Globals.Attributes.Agility];
        numVitality.Value = Selected.Attribute[(byte)Globals.Attributes.Vitality];
        for (byte i = 0; i < Selected.Drop.Count; i++) lstDrop.Items.Add(Drop_String(Selected.Drop[i]));
        cmbMovement.SelectedIndex = (byte)Selected.Movement;
        numFlee_Health.Value = Selected.Flee_Helth;
        for (byte i = 0; i < Selected.Allie.Count; i++) lstAllies.Items.Add(Globals.Numbering(Selected.Allie[i], Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[Selected.Allie[i]].Name);

        lstAllies.Update();
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
        Selected = Lists.NPC[List.SelectedIndex +1];
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
        Clear.NPC((short)(List.SelectedIndex + 1));

        // Atualiza os valores
        List.Items[List.SelectedIndex] = Globals.Numbering(List.SelectedIndex +1, List.Items.Count) + ":";
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
        Selected.Name = txtName.Text;
        List.Items[List.SelectedIndex] = Globals.Numbering(List.SelectedIndex + 1, List.Items.Count) + ":" + txtName.Text;
    }

    private void txtSayMsg_TextChanged(object sender, EventArgs e)
    {
        Selected.SayMsg = txtSayMsg.Text;
    }

    private void butTexture_Click(object sender, EventArgs e)
    {
        // Abre a pré visualização
        Selected.Texture = Preview.Select(Graphics.Tex_Character, Selected.Texture);
        numTexture.Value = Selected.Texture;
    }

    private void numTexture_ValueChanged(object sender, EventArgs e)
    {
        Selected.Texture = (byte)numTexture.Value;
    }

    private void numRange_ValueChanged(object sender, EventArgs e)
    {
        Selected.Sight = (byte)numRange.Value;
    }

    private void cmbBehavior_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Behaviour = (byte)cmbBehavior.SelectedIndex;
    }

    private void numHP_ValueChanged(object sender, EventArgs e)
    {
        Selected.Vital[(byte)Globals.Vitals.HP] = (short)numHP.Value;
    }

    private void numMP_ValueChanged(object sender, EventArgs e)
    {
        Selected.Vital[(byte)Globals.Vitals.MP] = (short)numMP.Value;
    }

    private void numStrength_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Strength] = (short)numStrength.Value;
    }

    private void numResistance_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Resistance] = (short)numResistance.Value;
    }

    private void numIntelligence_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Intelligence] = (short)numIntelligence.Value;
    }

    private void numAgility_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Agility] = (short)numAgility.Value;
    }

    private void numVitality_ValueChanged(object sender, EventArgs e)
    {
        Selected.Attribute[(byte)Globals.Attributes.Vitality] = (short)numVitality.Value;
    }

    private void numSpawn_ValueChanged(object sender, EventArgs e)
    {
        Selected.SpawnTime = (byte)numSpawn.Value;
    }

    private void numExperience_ValueChanged(object sender, EventArgs e)
    {
        Selected.Experience = (int)numExperience.Value;
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
            Selected.Drop.RemoveAt(Selected_Item);
        }
    }

    private void butItem_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o item
        if (cmbDrop_Item.SelectedIndex >= 0)
        {
            Lists.Structures.NPC_Drop Drop = new Lists.Structures.NPC_Drop((short)(cmbDrop_Item.SelectedIndex + 1), (short)numDrop_Amount.Value, (byte)numDrop_Chance.Value);
            Selected.Drop.Add(Drop);
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
        Selected.AttackNPC = chkAttackNPC.Checked;
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
            Selected.Allie.RemoveAt(Selected_Item);
        }
    }

    private void butAllie_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o aliado
        if (cmbAllie_NPC.SelectedIndex >= 0)
        {
            Selected.Allie.Add((short)(cmbAllie_NPC.SelectedIndex + 1));
            lstAllies.Items.Add(Globals.Numbering(cmbAllie_NPC.SelectedIndex + 1, Lists.NPC.GetUpperBound(0)) + ":" + Lists.NPC[cmbAllie_NPC.SelectedIndex + 1].Name);
            grpAllie_Add.Visible = false;
        }
    }

    private void cmbMovement_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Movement = (Globals.NPC_Movements)cmbMovement.SelectedIndex;
    }

    private void numFlee_Health_ValueChanged(object sender, EventArgs e)
    {
        Selected.Flee_Helth = (byte)numFlee_Health.Value;
    }
}