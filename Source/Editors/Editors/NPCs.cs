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
        Send.Request_Classes();
        Send.Request_Items();
        Send.Request_Shops();
        Send.Request_NPCs();
    }

    public static void Open()
    {
        // Lista de itens
        Objects.cmbDrop_Item.Items.Clear();
        foreach (Lists.Structures.Item Item in Lists.Item.Values) Objects.cmbDrop_Item.Items.Add(Item);

        // Lista de lojas
        Objects.cmbShop.Items.Clear();
        Objects.cmbShop.Items.Add("None");
        foreach (Lists.Structures.Shop Shop in Lists.Shop.Values) Objects.cmbShop.Items.Add(Shop);

        // Define os limites
        Objects.numTexture.Maximum = Graphics.Tex_Character.GetUpperBound(0);

        // Lista os NPCs
        Objects.List.Nodes.Clear();
        foreach (Lists.Structures.NPC NPC in Lists.NPC.Values)
        {
            Objects.List.Nodes.Add(NPC.Name);
            Objects.List.Nodes[Objects.List.Nodes.Count - 1].Tag = NPC.ID;
        }
        if (Objects.List.Nodes.Count > 0) Objects.List.SelectedNode = Objects.List.Nodes[0];

        // Abre a janela
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private void Groups_Visibility()
    {
        // Atualiza a visiblidade dos paineis
        grpGeneral.Visible = grpAttributes.Visible = grpBehaviour.Visible = grpDrop.Visible = grpAllies.Visible=  List.SelectedNode != null;
        grpAllie_Add.Visible = grpDrop_Add.Visible = false;
        List.Focus();
    }

    private void List_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza o valor da loja selecionada
        Selected = Lists.NPC[(Guid)List.SelectedNode.Tag];

        // Altera a visibilidade dos grupos se necessário
        Groups_Visibility();

        // Reseta os dados necessários
        lstDrop.Items.Clear();
        grpDrop_Add.Visible = false;
        lstAllies.Items.Clear();
        grpAllie_Add.Visible = false;

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
        chkAttackNPC.Checked = Selected.AttackNPC;
        for (byte i = 0; i < Selected.Allie.Count; i++) lstAllies.Items.Add(Selected.Allie[i].Name);
        if (Selected.Shop != null) cmbShop.SelectedIndex = cmbShop.Items.IndexOf(Selected.Shop);
        else cmbShop.SelectedIndex = 0;

        // Seleciona os primeiros itens
        if (lstDrop.Items.Count > 0) lstDrop.SelectedIndex = 0;
    }

    private void butNew_Click(object sender, EventArgs e)
    {
        // Adiciona uma loja nova
        Lists.Structures.NPC NPC = new Lists.Structures.NPC(Guid.NewGuid());
        NPC.Name = "New NPC";
        Lists.NPC.Add(NPC.ID, NPC);

        // Adiciona na lista
        TreeNode Node = new TreeNode(NPC.Name);
        Node.Tag = NPC.ID;
        List.Nodes.Add(Node);
        List.SelectedNode = Node;
    }

    private void butRemove_Click(object sender, EventArgs e)
    {
        // Remove a loja selecionada
        if (List.SelectedNode != null)
        {
            Lists.Item.Remove(Selected.ID);
            List.SelectedNode.Remove();
            Groups_Visibility();
        }
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados
        Send.Write_NPCs();

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

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Selected.Name = txtName.Text;
        List.SelectedNode.Text = txtName.Text;
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

        // Eventos especificos
        cmbShop.Enabled = Selected.Behaviour == (byte)Globals.NPC_Behaviour.ShopKeeper;
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
            Lists.Structures.NPC_Drop Drop = new Lists.Structures.NPC_Drop((Lists.Structures.Item)cmbDrop_Item.SelectedItem, (short)numDrop_Amount.Value, (byte)numDrop_Chance.Value);
            Selected.Drop.Add(Drop);
            lstDrop.Items.Add(Drop_String(Drop));
            grpDrop_Add.Visible = false;
        }
    }

    private string Drop_String(Lists.Structures.NPC_Drop Drop)
    {
        return Drop.Item.Name + " [" + Drop.Amount + "x, " + Drop.Chance + "%]";
    }

    private void chkAttackNPC_CheckedChanged(object sender, EventArgs e)
    {
        Selected.AttackNPC = lstAllies.Enabled = chkAttackNPC.Checked;
    }

    private void butAllie_Add_Click(object sender, EventArgs e)
    {
        // Adiciona os NPCs
        cmbAllie_NPC.Items.Clear();
        foreach (Lists.Structures.NPC NPC in Lists.NPC.Values) Objects.cmbAllie_NPC.Items.Add(NPC);
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
            Selected.Allie.Add((Lists.Structures.NPC)cmbAllie_NPC.SelectedItem);
            lstAllies.Items.Add(((Lists.Structures.NPC)cmbAllie_NPC.SelectedItem).Name);
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

    private void cmbShop_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cmbShop.SelectedIndex == 0)
            Selected.Shop = null;
        else
            Selected.Shop = (Lists.Structures.Shop)cmbShop.SelectedItem;
    }
}