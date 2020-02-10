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
            this.butStop = new System.Windows.Forms.Button();
            this.butPlay = new System.Windows.Forms.Button();
            this.cmbSound = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbAlignment = new System.Windows.Forms.ComboBox();
            this.numStartY = new System.Windows.Forms.NumericUpDown();
            this.numStartX = new System.Windows.Forms.NumericUpDown();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.numFrames = new System.Windows.Forms.NumericUpDown();
            this.cmbMovement = new System.Windows.Forms.ComboBox();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numHeight = new System.Windows.Forms.NumericUpDown();
            this.numWidth = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.grpStyle_Save = new System.Windows.Forms.GroupBox();
            this.butStyle_Confirm = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.txtStyle_Name = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStartX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrames)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).BeginInit();
            this.grpStyle_Save.SuspendLayout();
            this.SuspendLayout();
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(406, 524);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(180, 25);
            this.butCancel.TabIndex = 21;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(220, 524);
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
            this.List.Size = new System.Drawing.Size(202, 537);
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
            this.picTexture.Location = new System.Drawing.Point(221, 369);
            this.picTexture.Name = "picTexture";
            this.picTexture.Size = new System.Drawing.Size(365, 149);
            this.picTexture.TabIndex = 24;
            this.picTexture.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.butStop);
            this.groupBox2.Controls.Add(this.butPlay);
            this.groupBox2.Controls.Add(this.cmbSound);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.cmbAlignment);
            this.groupBox2.Controls.Add(this.numStartY);
            this.groupBox2.Controls.Add(this.numStartX);
            this.groupBox2.Controls.Add(this.numDuration);
            this.groupBox2.Controls.Add(this.numFrames);
            this.groupBox2.Controls.Add(this.cmbMovement);
            this.groupBox2.Controls.Add(this.picPreview);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(221, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(365, 203);
            this.groupBox2.TabIndex = 25;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Movement";
            // 
            // butStop
            // 
            this.butStop.Location = new System.Drawing.Point(305, 174);
            this.butStop.Name = "butStop";
            this.butStop.Size = new System.Drawing.Size(55, 23);
            this.butStop.TabIndex = 29;
            this.butStop.Text = "Stop";
            this.butStop.UseVisualStyleBackColor = true;
            this.butStop.Click += new System.EventHandler(this.butStop_Click);
            // 
            // butPlay
            // 
            this.butPlay.Location = new System.Drawing.Point(249, 174);
            this.butPlay.Name = "butPlay";
            this.butPlay.Size = new System.Drawing.Size(55, 23);
            this.butPlay.TabIndex = 28;
            this.butPlay.Text = "Play";
            this.butPlay.UseVisualStyleBackColor = true;
            this.butPlay.Click += new System.EventHandler(this.butPlay_Click);
            // 
            // cmbSound
            // 
            this.cmbSound.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSound.FormattingEnabled = true;
            this.cmbSound.Location = new System.Drawing.Point(10, 59);
            this.cmbSound.Name = "cmbSound";
            this.cmbSound.Size = new System.Drawing.Size(234, 21);
            this.cmbSound.TabIndex = 26;
            this.cmbSound.SelectedIndexChanged += new System.EventHandler(this.cmbSound_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 43);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "Sound:";
            // 
            // cmbAlignment
            // 
            this.cmbAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAlignment.FormattingEnabled = true;
            this.cmbAlignment.Items.AddRange(new object[] {
            "Horizontal",
            "Vertical"});
            this.cmbAlignment.Location = new System.Drawing.Point(10, 98);
            this.cmbAlignment.Name = "cmbAlignment";
            this.cmbAlignment.Size = new System.Drawing.Size(233, 21);
            this.cmbAlignment.TabIndex = 9;
            this.cmbAlignment.SelectedIndexChanged += new System.EventHandler(this.cmbAlignment_SelectedIndexChanged);
            // 
            // numStartY
            // 
            this.numStartY.Location = new System.Drawing.Point(132, 138);
            this.numStartY.Name = "numStartY";
            this.numStartY.Size = new System.Drawing.Size(112, 20);
            this.numStartY.TabIndex = 7;
            this.numStartY.ValueChanged += new System.EventHandler(this.numStartY_ValueChanged);
            // 
            // numStartX
            // 
            this.numStartX.Location = new System.Drawing.Point(10, 138);
            this.numStartX.Name = "numStartX";
            this.numStartX.Size = new System.Drawing.Size(112, 20);
            this.numStartX.TabIndex = 5;
            this.numStartX.ValueChanged += new System.EventHandler(this.numStartX_ValueChanged);
            // 
            // numDuration
            // 
            this.numDuration.Location = new System.Drawing.Point(132, 177);
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(112, 20);
            this.numDuration.TabIndex = 3;
            this.numDuration.ValueChanged += new System.EventHandler(this.numDuration_ValueChanged);
            // 
            // numFrames
            // 
            this.numFrames.Location = new System.Drawing.Point(10, 177);
            this.numFrames.Name = "numFrames";
            this.numFrames.Size = new System.Drawing.Size(112, 20);
            this.numFrames.TabIndex = 1;
            this.numFrames.ValueChanged += new System.EventHandler(this.numFrames_ValueChanged);
            // 
            // cmbMovement
            // 
            this.cmbMovement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMovement.FormattingEnabled = true;
            this.cmbMovement.Items.AddRange(new object[] {
            "uno",
            "dos"});
            this.cmbMovement.Location = new System.Drawing.Point(10, 19);
            this.cmbMovement.Name = "cmbMovement";
            this.cmbMovement.Size = new System.Drawing.Size(349, 21);
            this.cmbMovement.TabIndex = 0;
            this.cmbMovement.SelectedIndexChanged += new System.EventHandler(this.cmbMovement_SelectedIndexChanged);
            // 
            // picPreview
            // 
            this.picPreview.Location = new System.Drawing.Point(249, 59);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(111, 111);
            this.picPreview.TabIndex = 25;
            this.picPreview.TabStop = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Alignment:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(129, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Start Y:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Start X:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(129, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Duration (ms):";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Frames:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numHeight);
            this.groupBox3.Controls.Add(this.numWidth);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(221, 94);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(365, 60);
            this.groupBox3.TabIndex = 26;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Frame Size";
            // 
            // numHeight
            // 
            this.numHeight.Location = new System.Drawing.Point(190, 34);
            this.numHeight.Name = "numHeight";
            this.numHeight.Size = new System.Drawing.Size(169, 20);
            this.numHeight.TabIndex = 13;
            this.numHeight.ValueChanged += new System.EventHandler(this.numHeight_ValueChanged);
            // 
            // numWidth
            // 
            this.numWidth.Location = new System.Drawing.Point(10, 34);
            this.numWidth.Name = "numWidth";
            this.numWidth.Size = new System.Drawing.Size(169, 20);
            this.numWidth.TabIndex = 9;
            this.numWidth.ValueChanged += new System.EventHandler(this.numWidth_ValueChanged);
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
            // Editor_Sprites
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(596, 560);
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
            this.Name = "Editor_Sprites";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sprite Editor";
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStartX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numFrames)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWidth)).EndInit();
            this.grpStyle_Save.ResumeLayout(false);
            this.grpStyle_Save.PerformLayout();
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
    private System.Windows.Forms.PictureBox picTexture;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.ComboBox cmbMovement;
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
    private System.Windows.Forms.NumericUpDown numHeight;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown numWidth;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.ComboBox cmbAlignment;
    private System.Windows.Forms.PictureBox picPreview;
    private System.Windows.Forms.GroupBox grpStyle_Save;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox txtStyle_Name;
    private System.Windows.Forms.Button butStyle_Confirm;
    private System.Windows.Forms.ComboBox cmbSound;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Button butStop;
    private System.Windows.Forms.Button butPlay;
}