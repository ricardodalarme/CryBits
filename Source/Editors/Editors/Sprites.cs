using System;
using System.Windows.Forms;

partial class Editor_Sprites : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Sprites Objects = new Editor_Sprites();

    // Índice do item selecionado
    public static Lists.Structures.Sprite Selected;
    public static Lists.Structures.Sprite_Movement_Direction Selected_Movement;

    public Editor_Sprites()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Solicita os dados
        Send.Request_Sprites(true);
    }

    public static void Open()
    {
        // Reseta todos os valores
        Lists.Sprite = new Lists.Structures.Sprite[Graphics.Tex_Character.Length];
        for (short i = 0; i < Graphics.Tex_Character.Length; i++) Clear.Sprite(i);

        // Limpa todas as listas
        Objects.List.Items.Clear();
        Objects.cmbSound.Items.Clear();
        Objects.cmbSound.Items.Add("None");
        Objects.cmbMovement.Items.Clear();
        Objects.cmbDirection.Items.Clear();
        Objects.cmbAlignment.Items.Clear();

        // Lista todos os itens
        for (short i = 1; i < Graphics.Tex_Character.Length; i++) Objects.List.Items.Add(i);
        for (byte i = 1; i < (byte)Audio.Sounds.Count; i++) Objects.cmbSound.Items.Add(((Audio.Sounds)i).ToString());
        for (byte i = 0; i < (byte)Globals.Movements.Count; i++) Objects.cmbMovement.Items.Add(((Globals.Movements)i).ToString());
        for (byte i = 0; i < (byte)Globals.Directions.Count; i++) Objects.cmbDirection.Items.Add(((Globals.Directions)i).ToString());
        for (byte i = 0; i < (byte)Globals.Alignments.Count; i++) Objects.cmbAlignment.Items.Add(((Globals.Alignments)i).ToString());

        // Seleciona os primeiros indices
        Objects.List.SelectedIndex = 0;
        Objects.cmbMovement.SelectedIndex = 0;
        Objects.cmbDirection.SelectedIndex = 0;

        // Abre o editor
        Selection.Objects.Visible = false;
        Objects.Visible = true;
    }

    private void Update_Data()
    {
        // Reseta os valores necessários
        cmbMovement.SelectedIndex = 0;
        Update_Movement_Data();

        // Atualiza os dados
        numWidth.Value = Selected.Frame_Width;
        numHeight.Value = Selected.Frame_Height;
    }

    private void Update_Movement_Data()
    {
        // Previne erros
        if (cmbMovement.SelectedIndex == -1 || cmbDirection.SelectedIndex == -1) return;

        // Atualiza os dados dos movimentos
        Selected_Movement = Selected.Movement[cmbMovement.SelectedIndex].Direction[cmbDirection.SelectedIndex];
        cmbSound.SelectedIndex = Selected.Movement[cmbMovement.SelectedIndex].Sound;
        cmbAlignment.SelectedIndex = Selected_Movement.Alignment;
        numStartX.Value = Selected_Movement.StartX;
        numStartY.Value = Selected_Movement.StartY;
        numFrames.Value = Selected_Movement.Frames;
        numDuration.Value = Selected_Movement.Duration;
    }

    private void List_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza os dados
        Selected = Lists.Sprite[List.SelectedIndex + 1];
        Update_Data();
    }

    private void butSave_Click(object sender, EventArgs e)
    {
        // Salva os dados
        Send.Write_Sprites();

        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butCancel_Click(object sender, EventArgs e)
    {
        // Volta à janela de seleção
        Visible = false;
        Selection.Objects.Visible = true;
    }

    private void butStyle_Use_Click(object sender, EventArgs e)
    {

    }

    private void butStyle_Remove_Click(object sender, EventArgs e)
    {

    }

    private void butStyle_Save_Click(object sender, EventArgs e)
    {
        // Abre a janela para salvar o estilo
        txtStyle_Name.Text = string.Empty;
        grpStyle_Save.Visible = true;
    }

    private void butStyle_Confirm_Click(object sender, EventArgs e)
    {

    }

    private void numWidth_ValueChanged(object sender, EventArgs e)
    {
        Selected.Frame_Width = (byte)numWidth.Value;
    }

    private void numHeight_ValueChanged(object sender, EventArgs e)
    {
        Selected.Frame_Height = (byte)numHeight.Value;
    }

    private void cmbMovement_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza os dados dos movimentos
        Update_Movement_Data();
    }

    private void cmbDirection_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Atualiza os dados dos movimentos
        Update_Movement_Data();
    }

    private void cmbSound_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected.Movement[cmbMovement.SelectedIndex].Sound = (byte)cmbSound.SelectedIndex;
    }

    private void cmbAlignment_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected_Movement.Alignment = (byte)cmbAlignment.SelectedIndex;
    }

    private void numStartX_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement.StartX = (byte)numStartX.Value;
    }

    private void numStartY_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement.StartY = (byte)numStartY.Value;
    }

    private void numFrames_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement.Frames = (byte)numFrames.Value;
    }

    private void numDuration_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement.Duration = (short)numDuration.Value;
    }

    private void butPlay_Click(object sender, EventArgs e)
    {

    }

    private void butStop_Click(object sender, EventArgs e)
    {

    }
}