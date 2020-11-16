using System;
using System.Windows.Forms;
using CryBits.Editors.Entities.Tools;
using CryBits.Editors.Library;
using CryBits.Editors.Media;
using DarkUI.Forms;
using SFML.Graphics;
using Button = CryBits.Editors.Entities.Tools.Button;
using CheckBox = CryBits.Editors.Entities.Tools.CheckBox;
using Panel = CryBits.Editors.Entities.Tools.Panel;
using TextBox = CryBits.Editors.Entities.Tools.TextBox;

namespace CryBits.Editors.Forms
{
    internal partial class EditorInterface : DarkForm
    {
        // Usado para acessar os dados da janela
        public static EditorInterface Form;

        // Ferramenta selecionada
        private Tool _selected;

        public EditorInterface()
        {
            InitializeComponent();

            // Abre janela
            EditorMaps.Form.Hide();
            Show();

            // Inicializa a janela de renderização
            Graphics.WinInterface = new RenderWindow(picWindow.Handle);

            // Adiciona as janelas à lista
            cmbWindows.Items.AddRange(Enum.GetNames(typeof(WindowsTypes)));
            cmbWindows.SelectedIndex = 0;

            // Adiciona os tipos de ferramentas à lista
            for (byte i = 0; i < (byte)ToolType.Count; i++) cmbType.Items.Add((ToolType)i);
        }

        private void Editor_Interface_FormClosed(object sender, FormClosedEventArgs e)
        {
            Graphics.WinInterface = null;
            EditorMaps.Form.Show();
        }

        private void cmbWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Atualiza a lista de ordem
            treOrder.Nodes.Clear();
            treOrder.Nodes.Add(Tool.Tree.Nodes[cmbWindows.SelectedIndex]);
            treOrder.ExpandAll();
        }

        private void treOrder_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Atualiza as informações
            _selected = (Tool)treOrder.SelectedNode.Tag;
            prgProperties.SelectedObject = _selected;
        }

        private void prgProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (treOrder.SelectedNode != null)
            {
                byte window = (byte)((Tool)treOrder.SelectedNode.Tag).Window;

                // Troca a ferramenta de janela
                if (e.ChangedItem.Label == "Window")
                {
                    Tool.Tree.Nodes[window].Nodes.Add((TreeNode)treOrder.SelectedNode.Clone());
                    treOrder.SelectedNode.Remove();
                    cmbWindows.SelectedIndex = window;
                    treOrder.SelectedNode = Tool.Tree.Nodes[window].LastNode;
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
            Tool @new = new Tool();
            Tool.Tree.Nodes[cmbWindows.SelectedIndex].LastNode.Tag = @new;
            switch ((ToolType)cmbType.SelectedIndex)
            {
                case ToolType.Button: @new = new Button(); break;
                case ToolType.Panel: @new = new Panel(); break;
                case ToolType.CheckBox: @new = new CheckBox(); break;
                case ToolType.TextBox: @new = new TextBox(); break;
            }
            Tool.Tree.Nodes[cmbWindows.SelectedIndex].Nodes.Add(@new.ToString());
            @new.Window = (WindowsTypes)cmbWindows.SelectedIndex;
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
            TreeNode selectedNode = treOrder.SelectedNode;
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
            TreeNode selected = treOrder.SelectedNode;
            TreeNode parent = selected.Parent;
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
            TreeNode parent = treOrder.SelectedNode.Parent;
            TreeNode selected = treOrder.SelectedNode;
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
            TreeNode parent = treOrder.SelectedNode.Parent;
            TreeNode selected = treOrder.SelectedNode;
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
}