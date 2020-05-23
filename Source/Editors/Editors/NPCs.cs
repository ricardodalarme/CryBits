using System;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class Editor_NPCs : Form
{
    // Usado para acessar os dados da janela
    public static Editor_NPCs Form;

    // NPC selecionado
    private Lists.Structures.NPC Selected;

    public Editor_NPCs()
    {
        // Inicializa os componentes
        InitializeComponent();

        // Define os limites
        numTexture.Maximum = Graphics.Tex_Character.GetUpperBound(0);

        // Lista os dados
        foreach (Lists.Structures.Item Item in Lists.Item.Values) cmbDrop_Item.Items.Add(Item);
        foreach (Lists.Structures.Shop Shop in Lists.Shop.Values) cmbShop.Items.Add(Shop);
        List_Update();

        // Abre a janela
        Editor_Maps.Form.Hide();
        Show();
    }

    private void Groups_Visibility()
    {
        // Atualiza a visiblidade dos paineis
        grpGeneral.Visible = grpAttributes.Visible = grpBehaviour.Visible = grpDrop.Visible = grpAllies.Visible = List.SelectedNode != null;
        grpAllie_Add.Visible = grpDrop_Add.Visible = false;
    }

    private void List_Update()
    {
        // Lista os NPCs
        List.Nodes.Clear();
        foreach (Lists.Structures.NPC NPC in Lists.NPC.Values)
            if (NPC.Name.StartsWith(txtFilter.Text))
            {
                List.Nodes.Add(NPC.Name);
                List.Nodes[List.Nodes.Count - 1].Tag = NPC.ID;
            }

        // Seleciona o primeiro
        if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
        Groups_Visibility();
    }

    private void List_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza o valor da loja selecionada
        Selected = Lists.NPC[(Guid)List.SelectedNode.Tag];

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
        for (byte i = 0; i < Selected.Drop.Count; i++) lstDrop.Items.Add(Selected.Drop[i]);
        cmbMovement.SelectedIndex = (byte)Selected.Movement;
        numFlee_Health.Value = Selected.Flee_Helth;
        chkAttackNPC.Checked = Selected.AttackNPC;
        foreach (var Allie in Selected.Allie) lstAllies.Items.Add(Allie);
        if (Selected.Shop != null) cmbShop.SelectedItem = Selected.Shop;
        else cmbShop.SelectedIndex = -1;

        // Seleciona os primeiros itens
        if (lstDrop.Items.Count > 0) lstDrop.SelectedIndex = 0;
    }

    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
        List_Update();
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

        // Altera a visiblidade dos grupos caso necessários
        Groups_Visibility();
    }

    private void butRemove_Click(object sender, EventArgs e)
    {
        // Remove a loja selecionada
        if (List.SelectedNode != null)
        {
            Lists.NPC.Remove(Selected.ID);
            List.SelectedNode.Remove();
            Groups_Visibility();
        }
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados e volta à janela principal
        Send.Write_NPCs();
        Close();
        Editor_Maps.Form.Show();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta à janela principal
        Close();
        Editor_Maps.Form.Show();
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
        // Evita erros
        cmbShop.Enabled = false;
        cmbShop.SelectedIndex = -1;
        if (cmbBehavior.SelectedIndex == (byte)Globals.NPC_Behaviour.ShopKeeper)
            if (Lists.Shop.Count == 0)
            {
                cmbBehavior.SelectedIndex = Selected.Behaviour;
                return;
            }
            else
            {
                cmbShop.Enabled = true;
                if (Selected.Shop == null) cmbShop.SelectedIndex = 0;
            }

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
        // Evita erros
        if (Lists.Item.Count == 0)
        {
            MessageBox.Show("It must have at least one item registered add inital items.");
            return;
        }

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
        Lists.Structures.NPC_Drop Drop = new Lists.Structures.NPC_Drop((Lists.Structures.Item)cmbDrop_Item.SelectedItem, (short)numDrop_Amount.Value, (byte)numDrop_Chance.Value);
        Selected.Drop.Add(Drop);
        lstDrop.Items.Add(Drop);
        grpDrop_Add.Visible = false;
    }

    private void chkAttackNPC_CheckedChanged(object sender, EventArgs e)
    {
        Selected.AttackNPC = lstAllies.Enabled = chkAttackNPC.Checked;
        if (!Selected.AttackNPC)
        {
            Selected.Allie = new List<Lists.Structures.NPC>();
            lstAllies.Items.Clear();
        }
    }

    private void butAllie_Add_Click(object sender, EventArgs e)
    {
        if (chkAttackNPC.Checked)
        {
            // Adiciona os NPCs
            cmbAllie_NPC.Items.Clear();
            foreach (Lists.Structures.NPC NPC in Lists.NPC.Values) cmbAllie_NPC.Items.Add(NPC);
            cmbAllie_NPC.SelectedIndex = 0;

            // Abre a janela para adicionar o aliado
            grpAllie_Add.Visible = true;
        }
    }

    private void butAllie_Delete_Click(object sender, EventArgs e)
    {
        // Deleta a aliado
        var Selected_NPC = (Lists.Structures.NPC)lstAllies.SelectedItem;
        if (Selected_NPC != null)
        {
            lstAllies.Items.Remove(Selected_NPC);
            Selected.Allie.Remove(Selected_NPC);
        }
    }

    private void butAllie_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o aliado
        var Allie = (Lists.Structures.NPC)cmbAllie_NPC.SelectedItem;
        if (!Selected.Allie.Contains(Allie))
        {
            Selected.Allie.Add(Allie);
            lstAllies.Items.Add(cmbAllie_NPC.SelectedItem);
        }
        grpAllie_Add.Visible = false;
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
        Selected.Shop = (Lists.Structures.Shop)cmbShop.SelectedItem;
    }
}