using System;
using System.Linq;
using System.Windows.Forms;

partial class Editor_Classes : Form
{
    // Classe selecionada
    private Lists.Structures.Class Selected;

    public Editor_Classes()
    {
        // Inicializa os componentes 
        InitializeComponent();

        // Lista os dados
        foreach (Lists.Structures.Item Item in Lists.Item.Values) cmbItems.Items.Add(Item);
        foreach (Lists.Structures.Map Map in Lists.Map.Values) cmbSpawn_Map.Items.Add(Map);
        List_Update();

        // Abre a janela
        Editor_Maps.Form.Hide();
        Show();
    }

    private void Groups_Visibility()
    {
        // Atualiza a visiblidade dos paineis
        grpGeneral.Visible = grpAttributes.Visible = grpDrop.Visible = grpSpawn.Visible = grpTexture.Visible = List.SelectedNode != null;
        grpItem_Add.Visible = false;
    }

    private void List_Update()
    {
        // Lista as classes
        List.Nodes.Clear();
        foreach (Lists.Structures.Class Class in Lists.Class.Values)
            if (Class.Name.StartsWith(txtFilter.Text))
            {
                List.Nodes.Add(Class.Name);
                List.Nodes[List.Nodes.Count - 1].Tag = Class.ID;
            }

        // Seleciona o primeiro
        if (List.Nodes.Count > 0) List.SelectedNode = List.Nodes[0];
        Groups_Visibility();
    }

    private void List_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza o valor da loja selecionada
        Selected = Lists.Class[(Guid)List.SelectedNode.Tag];

        // Limpa os dados necessários
        lstMale.Items.Clear();
        lstFemale.Items.Clear();
        lstItems.Items.Clear();
        grpItem_Add.Visible = false;

        // Lista os dados
        txtName.Text = Selected.Name;
        txtDescription.Text = Selected.Description;
        numHP.Value = Selected.Vital[(byte)Globals.Vitals.HP];
        numMP.Value = Selected.Vital[(byte)Globals.Vitals.MP];
        numStrength.Value = Selected.Attribute[(byte)Globals.Attributes.Strength];
        numResistance.Value = Selected.Attribute[(byte)Globals.Attributes.Resistance];
        numIntelligence.Value = Selected.Attribute[(byte)Globals.Attributes.Intelligence];
        numAgility.Value = Selected.Attribute[(byte)Globals.Attributes.Agility];
        numVitality.Value = Selected.Attribute[(byte)Globals.Attributes.Vitality];
        cmbSpawn_Map.SelectedItem = Selected.Spawn_Map;
        cmbSpawn_Direction.SelectedIndex = Selected.Spawn_Direction;
        numSpawn_X.Value = Selected.Spawn_X;
        numSpawn_Y.Value = Selected.Spawn_Y;
        for (byte i = 0; i < Selected.Tex_Male.Count; i++) lstMale.Items.Add(Selected.Tex_Male[i]);
        for (byte i = 0; i < Selected.Tex_Female.Count; i++) lstFemale.Items.Add(Selected.Tex_Female[i]);
        for (byte i = 0; i < Selected.Item.Count; i++) lstItems.Items.Add(Selected.Item[i].Item1.Name + " [" + Selected.Item[i].Item2 + "x]");

        // Seleciona os primeiros itens
        if (lstMale.Items.Count > 0) lstMale.SelectedIndex = 0;
        if (lstFemale.Items.Count > 0) lstFemale.SelectedIndex = 0;
        if (lstItems.Items.Count > 0) lstItems.SelectedIndex = 0;
    }

    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
        List_Update();
    }

    private void butNew_Click(object sender, EventArgs e)
    {
        // Adiciona uma loja nova
        Lists.Structures.Class Class = new Lists.Structures.Class(Guid.NewGuid());
        Lists.Class.Add(Class.ID, Class);
        Class.Name = "New class";
        Class.Spawn_Map = Lists.Map.ElementAt(0).Value;

        // Adiciona na lista
        TreeNode Node = new TreeNode(Class.Name);
        Node.Tag = Class.ID;
        List.Nodes.Add(Node);
        List.SelectedNode = Node;
    }

    private void butRemove_Click(object sender, EventArgs e)
    {
        if (List.SelectedNode != null)
        {
            // Garante que sempre vai ter pelo menos uma classse
            if (Lists.Class.Count == 1)
            {
                MessageBox.Show("It must have at least one class registered.");
                return;
            }

            // Remove a classes selecionada
            Lists.Class.Remove(Selected.ID);
            List.SelectedNode.Remove();
        }
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados e volta à janela principal
        Send.Write_Classes();
        Close();
        Editor_Maps.Form.Show();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta à janela principal
        Send.Request_Classes();
        Close();
        Editor_Maps.Form.Show();
    }

    private void txtName_TextChanged(object sender, EventArgs e)
    {
        // Atualiza a lista
        Selected.Name = txtName.Text;
        List.SelectedNode.Text = txtName.Text;
    }

    private void txtDescription_TextChanged(object sender, EventArgs e)
    {
        Selected.Description = txtDescription.Text;
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

    private void butMTexture_Click(object sender, EventArgs e)
    {
        // Adiciona a textura
        short Texture_Num = Preview.Select(Graphics.Tex_Character, 0);
        if (Texture_Num != 0)
        {
            Selected.Tex_Male.Add(Texture_Num);
            lstMale.Items.Add(Texture_Num);
        }
    }

    private void butFTexture_Click(object sender, EventArgs e)
    {
        // Adiciona a textura
        short Texture_Num = Preview.Select(Graphics.Tex_Character, 0);
        if (Texture_Num != 0)
        {
            Selected.Tex_Female.Add(Texture_Num);
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
            Selected.Tex_Male.RemoveAt(Selected_Item);
        }
    }

    private void butFDelete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstFemale.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstFemale.Items.RemoveAt(Selected_Item);
            Selected.Tex_Female.RemoveAt(Selected_Item);
        }
    }

    private void cmbSpawn_Map_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Spawn_Map = (Lists.Structures.Map)cmbSpawn_Map.SelectedItem;
        numSpawn_X.Maximum = Selected.Spawn_Map.Width - 1;
        numSpawn_Y.Maximum = Selected.Spawn_Map.Height - 1;
    }

    private void cmbSpawn_Direction_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Spawn_Direction = (byte)cmbSpawn_Direction.SelectedIndex;
    }

    private void numSpawn_X_ValueChanged(object sender, EventArgs e)
    {
        Selected.Spawn_X = (byte)numSpawn_X.Value;
    }

    private void numSpawn_Y_ValueChanged(object sender, EventArgs e)
    {
        Selected.Spawn_Y = (byte)numSpawn_Y.Value;
    }

    private void butItem_Add_Click(object sender, EventArgs e)
    {
        // Evita erros
        if (Lists.Item.Count == 0)
        {
            MessageBox.Show("It must have at least one item registered add inital items.");
            return;
        }

        // Abre a janela para adicionar o item
        cmbItems.SelectedIndex = 0;
        numItem_Amount.Value = 1;
        grpItem_Add.Visible = true;
    }

    private void butItem_Ok_Click(object sender, EventArgs e)
    {
        // Adiciona o item
        lstItems.Items.Add(((Lists.Structures.Item)cmbItems.SelectedItem).Name + " [" + numItem_Amount.Value + "x]");
        Selected.Item.Add(new Tuple<Lists.Structures.Item, short>((Lists.Structures.Item)cmbItems.SelectedItem, (short)numItem_Amount.Value));
        grpItem_Add.Visible = false;
    }

    private void butItem_Delete_Click(object sender, EventArgs e)
    {
        // Deleta a textura
        short Selected_Item = (short)lstItems.SelectedIndex;
        if (Selected_Item != -1)
        {
            lstItems.Items.RemoveAt(Selected_Item);
            Selected.Item.RemoveAt(Selected_Item);
        }
    }

    private void cmbItems_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Quantidade de itens
        numItem_Amount.Enabled = ((Lists.Structures.Item)cmbItems.SelectedItem).Stackable;
        numItem_Amount.Value = 1;
    }
}