partial class Editor_Sprites
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor_Sprites));
            this.butCancel = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.List = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.butStyle_Save = new System.Windows.Forms.Button();
            this.butStyle_Remove = new System.Windows.Forms.Button();
            this.butStyle_Use = new System.Windows.Forms.Button();
            this.cmbStyle = new System.Windows.Forms.ComboBox();
            this.picTexture = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbMovement = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbSound = new System.Windows.Forms.ComboBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkSound = new System.Windows.Forms.CheckBox();
            this.cmbAlignment = new System.Windows.Forms.ComboBox();
            this.cmbDirection = new System.Windows.Forms.ComboBox();
            this.numStartY = new System.Windows.Forms.NumericUpDown();
            this.numFrames = new System.Windows.Forms.NumericUpDown();
            this.numStartX = new System.Windows.Forms.NumericUpDown();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.grpStyle_Save = new System.Windows.Forms.GroupBox();
            this.butStyle_Confirm = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtStyle_Name = new System.Windows.Forms.TextBox();
            this.numColor_Blue = new System.Windows.Forms.NumericUpDown();
            this.numColor_Green = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numColor_Red = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStartX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            this.grpStyle_Save.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numColor_Blue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColor_Green)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColor_Red)).BeginInit();
            this.SuspendLayout();
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(406, 574);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(180, 25);
            this.butCancel.TabIndex = 21;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(220, 574);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(180, 25);
            this.butSave.TabIndex = 20;
            this.butSave.Text = "Save All";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // List
            // 
            this.List.FormattingEnabled = true;
            this.List.Location = new System.Drawing.Point(12, 12);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(202, 589);
            this.List.TabIndex = 19;
            this.List.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.butStyle_Save);
            this.groupBox1.Controls.Add(this.butStyle_Remove);
            this.groupBox1.Controls.Add(this.butStyle_Use);
            this.groupBox1.Controls.Add(this.cmbStyle);
            this.groupBox1.Location = new System.Drawing.Point(221, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 76);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Style";
            // 
            // butStyle_Save
            // 
            this.butStyle_Save.Location = new System.Drawing.Point(246, 46);
            this.butStyle_Save.Name = "butStyle_Save";
            this.butStyle_Save.Size = new System.Drawing.Size(113, 20);
            this.butStyle_Save.TabIndex = 23;
            this.butStyle_Save.Text = "Save current style";
            this.butStyle_Save.UseVisualStyleBackColor = true;
            this.butStyle_Save.Click += new System.EventHandler(this.butStyle_Save_Click);
            // 
            // butStyle_Remove
            // 
            this.butStyle_Remove.Location = new System.Drawing.Point(128, 46);
            this.butStyle_Remove.Name = "butStyle_Remove";
            this.butStyle_Remove.Size = new System.Drawing.Size(113, 20);
            this.butStyle_Remove.TabIndex = 22;
            this.butStyle_Remove.Text = "Remove";
            this.butStyle_Remove.UseVisualStyleBackColor = true;
            this.butStyle_Remove.Click += new System.EventHandler(this.butStyle_Remove_Click);
            // 
            // butStyle_Use
            // 
            this.butStyle_Use.Location = new System.Drawing.Point(10, 46);
            this.butStyle_Use.Name = "butStyle_Use";
            this.butStyle_Use.Size = new System.Drawing.Size(113, 20);
            this.butStyle_Use.TabIndex = 21;
            this.butStyle_Use.Text = "Use";
            this.butStyle_Use.UseVisualStyleBackColor = true;
            this.butStyle_Use.Click += new System.EventHandler(this.butStyle_Use_Click);
            // 
            // cmbStyle
            // 
            this.cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStyle.FormattingEnabled = true;
            this.cmbStyle.Location = new System.Drawing.Point(10, 19);
            this.cmbStyle.Name = "cmbStyle";
            this.cmbStyle.Size = new System.Drawing.Size(349, 21);
            this.cmbStyle.TabIndex = 0;
            // 
            // picTexture
            // 
            this.picTexture.Location = new System.Drawing.Point(221, 445);
            this.picTexture.Name = "picTexture";
            this.picTexture.Size = new System.Drawing.Size(365, 123);
            this.picTexture.TabIndex = 24;
            this.picTexture.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numColor_Blue);
            this.groupBox2.Controls.Add(this.numColor_Green);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.numColor_Red);
            this.groupBox2.Controls.Add(this.cmbMovement);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.cmbSound);
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Location = new System.Drawing.Point(221, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(365, 279);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Movement";
            // 
            // cmbMovement
            // 
            this.cmbMovement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMovement.FormattingEnabled = true;
            this.cmbMovement.Items.AddRange(new object[] {
            "uno",
            "dos"});
            this.cmbMovement.Location = new System.Drawing.Point(10, 39);
            this.cmbMovement.Name = "cmbMovement";
            this.cmbMovement.Size = new System.Drawing.Size(349, 21);
            this.cmbMovement.TabIndex = 0;
            this.cmbMovement.SelectedIndexChanged += new System.EventHandler(this.cmbMovement_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(34, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "Type:";
            // 
            // cmbSound
            // 
            this.cmbSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSound.FormattingEnabled = true;
            this.cmbSound.Location = new System.Drawing.Point(246, 79);
            this.cmbSound.Name = "cmbSound";
            this.cmbSound.Size = new System.Drawing.Size(113, 21);
            this.cmbSound.TabIndex = 26;
            this.cmbSound.SelectedIndexChanged += new System.EventHandler(this.cmbSound_SelectedIndexChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkSound);
            this.groupBox4.Controls.Add(this.cmbAlignment);
            this.groupBox4.Controls.Add(this.cmbDirection);
            this.groupBox4.Controls.Add(this.numStartY);
            this.groupBox4.Controls.Add(this.numFrames);
            this.groupBox4.Controls.Add(this.numStartX);
            this.groupBox4.Controls.Add(this.numDuration);
            this.groupBox4.Controls.Add(this.picPreview);
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Location = new System.Drawing.Point(9, 106);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(351, 166);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Direction";
            // 
            // chkSound
            // 
            this.chkSound.AutoSize = true;
            this.chkSound.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSound.Location = new System.Drawing.Point(282, 143);
            this.chkSound.Name = "chkSound";
            this.chkSound.Size = new System.Drawing.Size(63, 17);
            this.chkSound.TabIndex = 31;
            this.chkSound.Text = "Sound?";
            this.chkSound.UseVisualStyleBackColor = true;
            // 
            // cmbAlignment
            // 
            this.cmbAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlignment.FormattingEnabled = true;
            this.cmbAlignment.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical"});
            this.cmbAlignment.Location = new System.Drawing.Point(6, 58);
            this.cmbAlignment.Name = "cmbAlignment";
            this.cmbAlignment.Size = new System.Drawing.Size(237, 21);
            this.cmbAlignment.TabIndex = 9;
            this.cmbAlignment.SelectedIndexChanged += new System.EventHandler(this.cmbAlignment_SelectedIndexChanged);
            // 
            // cmbDirection
            // 
            this.cmbDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDirection.FormattingEnabled = true;
            this.cmbDirection.Items.AddRange(new object[] {
            "uno",
            "dos"});
            this.cmbDirection.Location = new System.Drawing.Point(6, 19);
            this.cmbDirection.Name = "cmbDirection";
            this.cmbDirection.Size = new System.Drawing.Size(339, 21);
            this.cmbDirection.TabIndex = 30;
            this.cmbDirection.SelectedIndexChanged += new System.EventHandler(this.cmbDirection_SelectedIndexChanged);
            // 
            // numStartY
            // 
            this.numStartY.Location = new System.Drawing.Point(121, 98);
            this.numStartY.Name = "numStartY";
            this.numStartY.Size = new System.Drawing.Size(106, 20);
            this.numStartY.TabIndex = 7;
            this.numStartY.ValueChanged += new System.EventHandler(this.numStartY_ValueChanged);
            // 
            // numFrames
            // 
            this.numFrames.Location = new System.Drawing.Point(6, 137);
            this.numFrames.Name = "numFrames";
            this.numFrames.Size = new System.Drawing.Size(106, 20);
            this.numFrames.TabIndex = 1;
            this.numFrames.ValueChanged += new System.EventHandler(this.numFrames_ValueChanged);
            // 
            // numStartX
            // 
            this.numStartX.Location = new System.Drawing.Point(6, 98);
            this.numStartX.Name = "numStartX";
            this.numStartX.Size = new System.Drawing.Size(106, 20);
            this.numStartX.TabIndex = 5;
            this.numStartX.ValueChanged += new System.EventHandler(this.numStartX_ValueChanged);
            // 
            // numDuration
            // 
            this.numDuration.Location = new System.Drawing.Point(121, 137);
            this.numDuration.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(106, 20);
            this.numDuration.TabIndex = 3;
            this.numDuration.ValueChanged += new System.EventHandler(this.numDuration_ValueChanged);
            // 
            // picPreview
            // 
            this.picPreview.Location = new System.Drawing.Point(249, 46);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(96, 96);
            this.picPreview.TabIndex = 25;
            this.picPreview.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(118, 121);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Frame Duration (ms):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Frames:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Start X:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(118, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Start Y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Alignment:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(244, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Sound:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numWidth);
            this.groupBox3.Controls.Add(this.numHeight);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(221, 94);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(365, 60);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Frame Size";
            // 
            // numWidth
            // 
            this.numWidth.Location = new System.Drawing.Point(10, 34);
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(169, 20);
            this.numWidth.TabIndex = 9;
            this.numWidth.ValueChanged += new System.EventHandler(this.numWidth_ValueChanged);
            // 
            // numHeight
            // 
            this.numHeight.Location = new System.Drawing.Point(190, 34);
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(169, 20);
            this.numHeight.TabIndex = 13;
            this.numHeight.ValueChanged += new System.EventHandler(this.numHeight_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(187, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Height";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Width:";
            // 
            // grpStyle_Save
            // 
            this.grpStyle_Save.Controls.Add(this.butStyle_Confirm);
            this.grpStyle_Save.Controls.Add(this.label8);
            this.grpStyle_Save.Controls.Add(this.txtStyle_Name);
            this.grpStyle_Save.Location = new System.Drawing.Point(221, 12);
            this.grpStyle_Save.Name = "grpStyle_Save";
            this.grpStyle_Save.Size = new System.Drawing.Size(365, 76);
            this.grpStyle_Save.TabIndex = 27;
            this.grpStyle_Save.TabStop = false;
            this.grpStyle_Save.Text = "Save style";
            this.grpStyle_Save.Visible = false;
            // 
            // butStyle_Confirm
            // 
            this.butStyle_Confirm.Location = new System.Drawing.Point(260, 37);
            this.butStyle_Confirm.Name = "butStyle_Confirm";
            this.butStyle_Confirm.Size = new System.Drawing.Size(100, 20);
            this.butStyle_Confirm.TabIndex = 2;
            this.butStyle_Confirm.Text = "Confirm";
            this.butStyle_Confirm.UseVisualStyleBackColor = true;
            this.butStyle_Confirm.Click += new System.EventHandler(this.butStyle_Confirm_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(38, 13);
            this.label8.TabIndex = 1;
            this.label8.Text = "Name:";
            // 
            // txtStyle_Name
            // 
            this.txtStyle_Name.Location = new System.Drawing.Point(12, 37);
            this.txtStyle_Name.Name = "txtStyle_Name";
            this.txtStyle_Name.Size = new System.Drawing.Size(241, 20);
            this.txtStyle_Name.TabIndex = 0;
            // 
            // numColor_Blue
            // 
            this.numColor_Blue.Location = new System.Drawing.Point(169, 80);
            this.numColor_Blue.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numColor_Blue.Name = "numColor_Blue";
            this.numColor_Blue.Size = new System.Drawing.Size(71, 20);
            this.numColor_Blue.TabIndex = 40;
            // 
            // numColor_Green
            // 
            this.numColor_Green.Location = new System.Drawing.Point(90, 80);
            this.numColor_Green.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numColor_Green.Name = "numColor_Green";
            this.numColor_Green.Size = new System.Drawing.Size(71, 20);
            this.numColor_Green.TabIndex = 39;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 64);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 13);
            this.label11.TabIndex = 38;
            this.label11.Text = "Colors (RGB):";
            // 
            // numColor_Red
            // 
            this.numColor_Red.Location = new System.Drawing.Point(12, 80);
            this.numColor_Red.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numColor_Red.Name = "numColor_Red";
            this.numColor_Red.Size = new System.Drawing.Size(71, 20);
            this.numColor_Red.TabIndex = 37;
            // 
            // Editor_Sprites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 609);
            this.ControlBox = false;
            this.Controls.Add(this.grpStyle_Save);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.picTexture);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.List);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Editor_Sprites";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sprite Editor";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStartX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            this.grpStyle_Save.ResumeLayout(false);
            this.grpStyle_Save.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numColor_Blue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColor_Green)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numColor_Red)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.Button butSave;
    public System.Windows.Forms.ListBox List;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button butStyle_Save;
    private System.Windows.Forms.Button butStyle_Remove;
    private System.Windows.Forms.Button butStyle_Use;
    private System.Windows.Forms.ComboBox cmbStyle;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numDuration;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.NumericUpDown numFrames;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown numStartY;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numStartX;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.ComboBox cmbAlignment;
    private System.Windows.Forms.GroupBox grpStyle_Save;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox txtStyle_Name;
    private System.Windows.Forms.Button butStyle_Confirm;
    private System.Windows.Forms.ComboBox cmbSound;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.GroupBox groupBox4;
    public System.Windows.Forms.PictureBox picTexture;
    public System.Windows.Forms.PictureBox picPreview;
    private System.Windows.Forms.ComboBox cmbDirection;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.NumericUpDown numHeight;
    private System.Windows.Forms.NumericUpDown numWidth;
    public System.Windows.Forms.CheckBox chkSound;
    public System.Windows.Forms.ComboBox cmbMovement;
    private System.Windows.Forms.NumericUpDown numColor_Blue;
    private System.Windows.Forms.NumericUpDown numColor_Green;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.NumericUpDown numColor_Red;
}