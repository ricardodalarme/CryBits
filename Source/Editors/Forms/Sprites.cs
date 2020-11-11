using System;
using System.Windows.Forms;
using System.Drawing;

partial class Editor_Sprites : Form
{
    // Usado para acessar os dados da janela
    public static Editor_Sprites Objects = new Editor_Sprites();

    // Itens selecionados
    public static Lists.Structures.Sprite Selected;
    public static Lists.Structures.Sprite_Movement Selected_Movement;
    public static Lists.Structures.Sprite_Movement_Direction Selected_Movement_Dir;

    public Editor_Sprites()
    {
        InitializeComponent();
    }

    public static void Request()
    {
        // Solicita os dados
        Globals.OpenEditor = Objects;
        Read.Sprites();
        Open();
    }

    public static void Open()
    {
        // Limpa todas as listas
        Objects.List.Items.Clear();
        Objects.cmbSound.Items.Clear();
        Objects.cmbSound.Items.Add("None");
        Objects.cmbMovement.Items.Clear();
        Objects.cmbDirection.Items.Clear();
        Objects.cmbAlignment.Items.Clear();

        // Lista todos os itens
        for (short i = 1; i < Graphics.Tex_Character.Length; i++) Objects.List.Items.Add(i + Graphics.Format); ;
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
        Selected_Movement = Selected.Movement[cmbMovement.SelectedIndex];
        Selected_Movement_Dir = Selected_Movement.Direction[cmbDirection.SelectedIndex];
        cmbSound.SelectedIndex = Selected_Movement.Sound;
        Color Render_Color = Color.FromArgb(Selected_Movement.Color);
        numColor_Red.Value = Render_Color.R;
        numColor_Green.Value = Render_Color.G;
        numColor_Blue.Value = Render_Color.B;
        chkBackwards.Checked = Selected_Movement_Dir.Backwards;
        cmbAlignment.SelectedIndex = Selected_Movement_Dir.Alignment;
        numStartX.Value = Selected_Movement_Dir.StartX;
        numStartY.Value = Selected_Movement_Dir.StartY;
        numFrames.Value = Selected_Movement_Dir.Frames;
        numDuration.Value = Selected_Movement_Dir.Duration;
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
       // Send.Write_Sprites();

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

    private void numColor_Red_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement.Color =  Color.FromArgb((byte)numColor_Red.Value, (byte)numColor_Green.Value, (byte)numColor_Blue.Value).ToArgb();
    }

    private void numColor_Green_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement.Color =  Color.FromArgb((byte)numColor_Red.Value, (byte)numColor_Green.Value, (byte)numColor_Blue.Value).ToArgb();
    }

    private void numColor_Blue_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement.Color = Color.FromArgb((byte)numColor_Red.Value, (byte)numColor_Green.Value, (byte)numColor_Blue.Value).ToArgb();
    }


    private void cmbAlignment_SelectedIndexChanged(object sender, EventArgs e)
    {
        Selected_Movement_Dir.Alignment = (byte)cmbAlignment.SelectedIndex;
    }

    private void numStartX_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement_Dir.StartX = (byte)numStartX.Value;
    }

    private void numStartY_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement_Dir.StartY = (byte)numStartY.Value;
    }

    private void numFrames_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement_Dir.Frames = (byte)numFrames.Value;
    }

    private void numDuration_ValueChanged(object sender, EventArgs e)
    {
        Selected_Movement_Dir.Duration = (short)numDuration.Value;
    }
    private void chkBackwards_CheckedChanged(object sender, EventArgs e)
    {
        Selected_Movement_Dir.Backwards = chkBackwards.Enabled;
    }
}