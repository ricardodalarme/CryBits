using System;
using System.Windows.Forms;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Editors.Graphics;
using CryBits.Editors.Library;
using DarkUI.Forms;
using SFML.Graphics;
using Button = CryBits.Client.Framework.Interfacily.Components.Button;
using CheckBox = CryBits.Client.Framework.Interfacily.Components.CheckBox;
using Panel = CryBits.Client.Framework.Interfacily.Components.Panel;
using TextBox = CryBits.Client.Framework.Interfacily.Components.TextBox;

namespace CryBits.Editors.Forms;

internal partial class EditorInterface : DarkForm
{
    // Usado para acessar os dados da janela
    public static EditorInterface Form;

    // Árvore de componentes
    public static TreeNode Tree = new();

    public EditorInterface()
    {
        InitializeComponent();

        // Abre janela
        EditorMaps.Form.Hide();
        Show();

        // Inicializa a janela de renderização
        // (rendering is now handled by the Avalonia editor via Renders.WinInterfaceRT)

        // Adiciona as janelas à lista
        for (byte i = 0; i < Tree.Nodes.Count; i++)
            cmbWindows.Items.Add(Tree.Nodes[i].Text);
        cmbWindows.SelectedIndex = 0;

        // Adiciona os tipos de ferramentas à lista
        for (byte i = 0; i < (byte)ToolType.Count; i++) cmbType.Items.Add((ToolType)i);
    }

    private void Editor_Interface_FormClosed(object sender, FormClosedEventArgs e)
    {
        EditorMaps.Form.Show();
    }

    private void cmbWindows_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza a lista de ordem
        treOrder.Nodes.Clear();
        treOrder.Nodes.Add((TreeNode)Tree.Nodes[cmbWindows.SelectedIndex].Clone());
        treOrder.ExpandAll();
    }

    private void treOrder_AfterSelect(object sender, TreeViewEventArgs e)
    {
        // Atualiza as informações
        prgProperties.SelectedObject = treOrder.SelectedNode.Tag;
    }

    private void prgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
    {
        if (treOrder.SelectedNode != null)
        {
            // Troca o nome da ferramenta
            if (e.ChangedItem.Label == "Name") treOrder.SelectedNode.Text = treOrder.SelectedNode.Tag.ToString();
        }
    }

    private void butSaveAll_Click(object sender, EventArgs e)
    {
        // Salva os dados e volta à janela principal
        Write.Tools();
        Close();
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta à janela principal
        Close();
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
        Component @new;
        switch ((ToolType)cmbType.SelectedIndex)
        {
            case ToolType.Button: @new = new Button(); break;
            case ToolType.Panel: @new = new Panel(); break;
            case ToolType.CheckBox: @new = new CheckBox(); break;
            case ToolType.TextBox: @new = new TextBox(); break;
            default: return;
        }
        Tree.Nodes[cmbWindows.SelectedIndex].LastNode.Tag = @new;
        Tree.Nodes[cmbWindows.SelectedIndex].Nodes.Add(@new.ToString());
        grpNew.Visible = false;
    }

    private void butRemove_Click(object sender, EventArgs e)
    {
        // Remove a ferramenta
        if (treOrder.SelectedNode?.Parent != null)
            treOrder.SelectedNode.Remove();
    }

    private void butOrder_Pin_Click(object sender, EventArgs e)
    {
        // Dados
        var selectedNode = treOrder.SelectedNode;
        if (treOrder.SelectedNode != null)
            if (selectedNode.PrevNode != null)
            {
                // Fixa o nó
                selectedNode.PrevNode.Nodes.Add((TreeNode)selectedNode.Clone());
                treOrder.SelectedNode = selectedNode.PrevNode.LastNode;
                selectedNode.Remove();
            }

        // Foca o componente
        treOrder.Focus();
    }

    private void butOrder_Unpin_Click(object sender, EventArgs e)
    {
        // Evita erros 
        if (treOrder.SelectedNode == null) return;

        // Dados
        var selected = treOrder.SelectedNode;
        var parent = selected.Parent;
        if (parent?.Parent != null)
        {
            // Desfixa o nó
            parent.Parent.Nodes.Insert(parent.Index + 1, (TreeNode)selected.Clone());
            treOrder.SelectedNode = selected.Parent.NextNode;
            selected.Remove();
        }

        // Foca o componente
        treOrder.Focus();
    }

    private void butOrder_Up_Click(object sender, EventArgs e)
    {
        // Evita erros 
        if (treOrder.SelectedNode == null) return;

        // Dados
        var parent = treOrder.SelectedNode.Parent;
        var selected = treOrder.SelectedNode;
        if (parent != null && selected != parent.FirstNode && parent.Nodes.Count > 1)
        {
            // Altera a posição dos nós
            parent.Nodes.Insert(selected.Index - 1, (TreeNode)selected.Clone());
            selected.Remove();
            treOrder.SelectedNode = parent.Nodes[selected.Index - 2];
        }

        // Foca o componente
        treOrder.Focus();
    }

    private void butOrder_Down_Click(object sender, EventArgs e)
    {
        // Evita erros 
        if (treOrder.SelectedNode == null) return;

        // Dados
        var parent = treOrder.SelectedNode.Parent;
        var selected = treOrder.SelectedNode;
        if (parent != null && selected != parent.LastNode && parent.Nodes.Count > 1)
        {
            // Altera a posição dos nós
            parent.Nodes.Insert(selected.Index + 2, (TreeNode)selected.Clone());
            selected.Remove();
            treOrder.SelectedNode = parent.Nodes[selected.Index + 1];
        }

        // Foca o componente
        treOrder.Focus();
    }
}