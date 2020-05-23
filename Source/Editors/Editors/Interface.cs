using System;
using System.Windows.Forms;

partial class Editor_Interface : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Interface Form;

    // Ferramenta selecionada
    private Lists.Structures.Tool Selected;

    public Editor_Interface()
    {
        // Inicializa os componentes
        InitializeComponent();
        Graphics.Win_Interface = new SFML.Graphics.RenderWindow(picWindow.Handle);

        // Adiciona as janelas à lista
        cmbWindows.Items.AddRange(Enum.GetNames(typeof(Globals.Windows)));
        cmbWindows.SelectedIndex = 0;

        // Adiciona os tipos de ferramentas à lista
        for (byte i = 0; i < (byte)Globals.Tools_Types.Count; i++) cmbType.Items.Add((Globals.Tools_Types)i);

        // Abre a janela
        Editor_Maps.Form.Hide();
        Show();
    }

    private void cmbWindows_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista de ordem
        treOrder.Nodes.Clear();
        treOrder.Nodes.Add(Lists.Tool.Nodes[cmbWindows.SelectedIndex]);
        treOrder.ExpandAll();
    }

    private void treOrder_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza as informações
        Selected = (Lists.Structures.Tool)treOrder.SelectedNode.Tag;
        prgProperties.SelectedObject = Selected;
    }

    private void prgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        if (treOrder.SelectedNode != null)
        {
            byte Window = (byte)((Lists.Structures.Tool)treOrder.SelectedNode.Tag).Window;

            // Troca a ferramenta de janela
            if (e.ChangedItem.Label == "Window")
            {
                Lists.Tool.Nodes[Window].Nodes.Add((TreeNode)treOrder.SelectedNode.Clone());
                treOrder.SelectedNode.Remove();
                cmbWindows.SelectedIndex = Window;
                treOrder.SelectedNode = Lists.Tool.Nodes[Window].LastNode;
            }
            // Troca o nome da ferramenta
            else if (e.ChangedItem.Label == "Name") treOrder.SelectedNode.Text = treOrder.SelectedNode.Tag.ToString();
        }
    }

    private void butSaveAll_Click(object sender, EventArgs e)
    {
        // Salva os dados e volta à janela principal
        Write.Tools();
        Close();
        Editor_Maps.Form.Show();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta à janela principal
        Close();
        Editor_Maps.Form.Show();
    }

    private void butNew_Click(object sender, EventArgs e)
    {
        // Abre o painel para seleção da ferramenta
        cmbType.SelectedIndex = 0;
        grpNew.BringToFront();
        grpNew.Visible = true;
    }

    private void butConfirm_Click(object sender, EventArgs e)
    {
        // Adiciona uma nova ferramenta
        Lists.Structures.Tool Tool = new Lists.Structures.Tool();
        Lists.Tool.Nodes[cmbWindows.SelectedIndex].LastNode.Tag = Tool;
        switch ((Globals.Tools_Types)cmbType.SelectedIndex)
        {
            case Globals.Tools_Types.Button: Tool = new Lists.Structures.Button(); break;
            case Globals.Tools_Types.Panel: Tool = new Lists.Structures.Panel(); break;
            case Globals.Tools_Types.CheckBox: Tool = new Lists.Structures.CheckBox(); break;
            case Globals.Tools_Types.TextBox: Tool = new Lists.Structures.TextBox(); break;
        }
        Lists.Tool.Nodes[cmbWindows.SelectedIndex].Nodes.Add(Tool.ToString());
        Tool.Window = (Globals.Windows)cmbWindows.SelectedIndex;
        grpNew.Visible = false;
    }

    private void butRemove_Click(object sender, EventArgs e)
    {
        // Remove a ferramenta
        if (treOrder.SelectedNode != null && treOrder.SelectedNode.Parent != null)
            treOrder.SelectedNode.Remove();
    }

    private void butOrder_Pin_Click(object sender, EventArgs e)
    {
        // Dados
        TreeNode Selected_Node = treOrder.SelectedNode;
        if (treOrder.SelectedNode != null)
            if (Selected_Node.PrevNode != null)
            {
                // Fixa o nó
                Selected_Node.PrevNode.Nodes.Add((TreeNode)Selected_Node.Clone());
                treOrder.SelectedNode = Selected_Node.PrevNode.LastNode;
                Selected_Node.Remove();
            }

        // Foca o componente
        treOrder.Focus();
    }

    private void butOrder_Unpin_Click(object sender, EventArgs e)
    {
        // Evita erros 
        if (treOrder.SelectedNode == null) return;

        // Dados
        TreeNode Selected = treOrder.SelectedNode;
        TreeNode Parent = Selected.Parent;
        if (Parent != null && Parent.Parent != null)
        {
            // Desfixa o nó
            Parent.Parent.Nodes.Insert(Parent.Index + 1, (TreeNode)Selected.Clone());
            treOrder.SelectedNode = Selected.Parent.NextNode;
            Selected.Remove();
        }

        // Foca o componente
        treOrder.Focus();
    }

    private void butOrder_Up_Click(object sender, EventArgs e)
    {
        // Evita erros 
        if (treOrder.SelectedNode == null) return;

        // Dados
        TreeNode Parent = treOrder.SelectedNode.Parent;
        TreeNode Selected = treOrder.SelectedNode;
        if (Parent != null && Selected != Parent.FirstNode && Parent.Nodes.Count > 1)
        {
            // Altera a posição dos nós
            Parent.Nodes.Insert(Selected.Index - 1, (TreeNode)Selected.Clone());
            Selected.Remove();
            treOrder.SelectedNode = Parent.Nodes[Selected.Index - 2];
        }

        // Foca o componente
        treOrder.Focus();
    }

    private void butOrder_Down_Click(object sender, EventArgs e)
    {
        // Evita erros 
        if (treOrder.SelectedNode == null) return;

        // Dados
        TreeNode Parent = treOrder.SelectedNode.Parent;
        TreeNode Selected = treOrder.SelectedNode;
        if (Parent != null && Selected != Parent.LastNode && Parent.Nodes.Count > 1)
        {
            // Altera a posição dos nós
            Parent.Nodes.Insert(Selected.Index + 2, (TreeNode)Selected.Clone());
            Selected.Remove();
            treOrder.SelectedNode = Parent.Nodes[Selected.Index + 1];
        }

        // Foca o componente
        treOrder.Focus();
    }
}