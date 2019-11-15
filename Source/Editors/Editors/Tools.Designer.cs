partial class Editor_Tools
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
            this.panButton = new System.Windows.Forms.GroupBox();
            this.butButton_Texture = new System.Windows.Forms.Button();
            this.lblButton_Texture = new System.Windows.Forms.Label();
            this.List = new System.Windows.Forms.ListBox();
            this.cmbTools = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.panTextBox = new System.Windows.Forms.GroupBox();
            this.chkTextBox_Password = new System.Windows.Forms.CheckBox();
            this.scrlTextBox_Width = new System.Windows.Forms.HScrollBar();
            this.lblTextBox_Width = new System.Windows.Forms.Label();
            this.scrlTextBox_Max_Characters = new System.Windows.Forms.HScrollBar();
            this.lblTextBox_Max_Characters = new System.Windows.Forms.Label();
            this.panPanel = new System.Windows.Forms.GroupBox();
            this.butPanel_Texture = new System.Windows.Forms.Button();
            this.lblPanel_Texture = new System.Windows.Forms.Label();
            this.panCheckBox = new System.Windows.Forms.GroupBox();
            this.txtCheckBox_Text = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.butQuantity = new System.Windows.Forms.Button();
            this.butClear = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.lblY = new System.Windows.Forms.Label();
            this.lblX = new System.Windows.Forms.Label();
            this.panButton.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panTextBox.SuspendLayout();
            this.panPanel.SuspendLayout();
            this.panCheckBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            this.SuspendLayout();
            // 
            // panButton
            // 
            this.panButton.Controls.Add(this.butButton_Texture);
            this.panButton.Controls.Add(this.lblButton_Texture);
            this.panButton.Location = new System.Drawing.Point(220, 174);
            this.panButton.Name = "panButton";
            this.panButton.Size = new System.Drawing.Size(306, 70);
            this.panButton.TabIndex = 3;
            this.panButton.TabStop = false;
            this.panButton.Text = "Button";
            // 
            // butButton_Texture
            // 
            this.butButton_Texture.Location = new System.Drawing.Point(16, 34);
            this.butButton_Texture.Name = "butButton_Texture";
            this.butButton_Texture.Size = new System.Drawing.Size(273, 20);
            this.butButton_Texture.TabIndex = 27;
            this.butButton_Texture.Text = "Select";
            this.butButton_Texture.UseVisualStyleBackColor = true;
            this.butButton_Texture.Click += new System.EventHandler(this.butButton_Texture_Click);
            // 
            // lblButton_Texture
            // 
            this.lblButton_Texture.AutoSize = true;
            this.lblButton_Texture.Location = new System.Drawing.Point(13, 18);
            this.lblButton_Texture.Name = "lblButton_Texture";
            this.lblButton_Texture.Size = new System.Drawing.Size(55, 13);
            this.lblButton_Texture.TabIndex = 5;
            this.lblButton_Texture.Text = "Texture: 0";
            // 
            // List
            // 
            this.List.FormattingEnabled = true;
            this.List.Location = new System.Drawing.Point(12, 43);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(202, 212);
            this.List.TabIndex = 4;
            this.List.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // cmbTools
            // 
            this.cmbTools.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTools.Location = new System.Drawing.Point(12, 12);
            this.cmbTools.Name = "cmbTools";
            this.cmbTools.Size = new System.Drawing.Size(202, 21);
            this.cmbTools.TabIndex = 9;
            this.cmbTools.SelectedIndexChanged += new System.EventHandler(this.cmbFerramentas_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.chkVisible);
            this.groupBox2.Location = new System.Drawing.Point(220, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 91);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(16, 42);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(273, 20);
            this.txtName.TabIndex = 8;
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Name:";
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.Location = new System.Drawing.Point(16, 68);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(62, 17);
            this.chkVisible.TabIndex = 5;
            this.chkVisible.Text = "Visible?";
            this.chkVisible.UseVisualStyleBackColor = true;
            this.chkVisible.CheckedChanged += new System.EventHandler(this.chkVisible_CheckedChanged);
            // 
            // panTextBox
            // 
            this.panTextBox.Controls.Add(this.chkTextBox_Password);
            this.panTextBox.Controls.Add(this.scrlTextBox_Width);
            this.panTextBox.Controls.Add(this.lblTextBox_Width);
            this.panTextBox.Controls.Add(this.scrlTextBox_Max_Characters);
            this.panTextBox.Controls.Add(this.lblTextBox_Max_Characters);
            this.panTextBox.Location = new System.Drawing.Point(220, 174);
            this.panTextBox.Name = "panTextBox";
            this.panTextBox.Size = new System.Drawing.Size(306, 82);
            this.panTextBox.TabIndex = 11;
            this.panTextBox.TabStop = false;
            this.panTextBox.Text = "TextBox";
            // 
            // chkTextBox_Password
            // 
            this.chkTextBox_Password.AutoSize = true;
            this.chkTextBox_Password.Location = new System.Drawing.Point(9, 60);
            this.chkTextBox_Password.Name = "chkTextBox_Password";
            this.chkTextBox_Password.Size = new System.Drawing.Size(78, 17);
            this.chkTextBox_Password.TabIndex = 8;
            this.chkTextBox_Password.Text = "Password?";
            this.chkTextBox_Password.UseVisualStyleBackColor = true;
            this.chkTextBox_Password.CheckedChanged += new System.EventHandler(this.chkTextBox_Password_CheckedChanged);
            // 
            // scrlTextBox_Width
            // 
            this.scrlTextBox_Width.Location = new System.Drawing.Point(152, 37);
            this.scrlTextBox_Width.Maximum = 500;
            this.scrlTextBox_Width.Name = "scrlTextBox_Width";
            this.scrlTextBox_Width.Size = new System.Drawing.Size(137, 20);
            this.scrlTextBox_Width.TabIndex = 7;
            this.scrlTextBox_Width.ValueChanged += new System.EventHandler(this.scrlTextBox_Width_ValueChanged);
            // 
            // lblTextBox_Width
            // 
            this.lblTextBox_Width.AutoSize = true;
            this.lblTextBox_Width.Location = new System.Drawing.Point(149, 18);
            this.lblTextBox_Width.Name = "lblTextBox_Width";
            this.lblTextBox_Width.Size = new System.Drawing.Size(47, 13);
            this.lblTextBox_Width.TabIndex = 6;
            this.lblTextBox_Width.Text = "Width: 0";
            // 
            // scrlTextBox_Max_Characters
            // 
            this.scrlTextBox_Max_Characters.Location = new System.Drawing.Point(7, 37);
            this.scrlTextBox_Max_Characters.Name = "scrlTextBox_Max_Characters";
            this.scrlTextBox_Max_Characters.Size = new System.Drawing.Size(137, 20);
            this.scrlTextBox_Max_Characters.TabIndex = 5;
            this.scrlTextBox_Max_Characters.ValueChanged += new System.EventHandler(this.scrlTextBox_Max_Characters_ValueChanged);
            // 
            // lblTextBox_Max_Characters
            // 
            this.lblTextBox_Max_Characters.AutoSize = true;
            this.lblTextBox_Max_Characters.Location = new System.Drawing.Point(4, 18);
            this.lblTextBox_Max_Characters.Name = "lblTextBox_Max_Characters";
            this.lblTextBox_Max_Characters.Size = new System.Drawing.Size(140, 13);
            this.lblTextBox_Max_Characters.TabIndex = 4;
            this.lblTextBox_Max_Characters.Text = "Maximum characters: Infinity";
            // 
            // panPanel
            // 
            this.panPanel.Controls.Add(this.butPanel_Texture);
            this.panPanel.Controls.Add(this.lblPanel_Texture);
            this.panPanel.Location = new System.Drawing.Point(220, 174);
            this.panPanel.Name = "panPanel";
            this.panPanel.Size = new System.Drawing.Size(306, 71);
            this.panPanel.TabIndex = 12;
            this.panPanel.TabStop = false;
            this.panPanel.Text = "Panel";
            // 
            // butPanel_Texture
            // 
            this.butPanel_Texture.Location = new System.Drawing.Point(16, 37);
            this.butPanel_Texture.Name = "butPanel_Texture";
            this.butPanel_Texture.Size = new System.Drawing.Size(273, 20);
            this.butPanel_Texture.TabIndex = 26;
            this.butPanel_Texture.Text = "Select";
            this.butPanel_Texture.UseVisualStyleBackColor = true;
            this.butPanel_Texture.Click += new System.EventHandler(this.butPanel_Texture_Click);
            // 
            // lblPanel_Texture
            // 
            this.lblPanel_Texture.AutoSize = true;
            this.lblPanel_Texture.Location = new System.Drawing.Point(13, 18);
            this.lblPanel_Texture.Name = "lblPanel_Texture";
            this.lblPanel_Texture.Size = new System.Drawing.Size(55, 13);
            this.lblPanel_Texture.TabIndex = 2;
            this.lblPanel_Texture.Text = "Texture: 0";
            // 
            // panCheckBox
            // 
            this.panCheckBox.Controls.Add(this.txtCheckBox_Text);
            this.panCheckBox.Controls.Add(this.label1);
            this.panCheckBox.Location = new System.Drawing.Point(220, 174);
            this.panCheckBox.Name = "panCheckBox";
            this.panCheckBox.Size = new System.Drawing.Size(306, 70);
            this.panCheckBox.TabIndex = 13;
            this.panCheckBox.TabStop = false;
            this.panCheckBox.Text = "CheckBox";
            // 
            // txtCheckBox_Text
            // 
            this.txtCheckBox_Text.Location = new System.Drawing.Point(9, 34);
            this.txtCheckBox_Text.Name = "txtCheckBox_Text";
            this.txtCheckBox_Text.Size = new System.Drawing.Size(284, 20);
            this.txtCheckBox_Text.TabIndex = 9;
            this.txtCheckBox_Text.Validated += new System.EventHandler(this.txtCheckBox_Text_Validated);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Text:";
            // 
            // butQuantity
            // 
            this.butQuantity.Location = new System.Drawing.Point(12, 262);
            this.butQuantity.Name = "butQuantity";
            this.butQuantity.Size = new System.Drawing.Size(202, 25);
            this.butQuantity.TabIndex = 19;
            this.butQuantity.Text = "Change Quantity";
            this.butQuantity.UseVisualStyleBackColor = true;
            this.butQuantity.Click += new System.EventHandler(this.butQuantity_Click);
            // 
            // butClear
            // 
            this.butClear.Location = new System.Drawing.Point(323, 262);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(97, 25);
            this.butClear.TabIndex = 22;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(428, 262);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(97, 25);
            this.butCancel.TabIndex = 21;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(220, 262);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(97, 25);
            this.butSave.TabIndex = 20;
            this.butSave.Text = "Save";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numY);
            this.groupBox1.Controls.Add(this.numX);
            this.groupBox1.Controls.Add(this.lblY);
            this.groupBox1.Controls.Add(this.lblX);
            this.groupBox1.Location = new System.Drawing.Point(220, 109);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(305, 59);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Position:";
            // 
            // numY
            // 
            this.numY.Location = new System.Drawing.Point(181, 26);
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(108, 20);
            this.numY.TabIndex = 8;
            this.numY.ValueChanged += new System.EventHandler(this.numY_ValueChanged);
            // 
            // numX
            // 
            this.numX.Location = new System.Drawing.Point(36, 28);
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(108, 20);
            this.numX.TabIndex = 7;
            this.numX.ValueChanged += new System.EventHandler(this.numX_ValueChanged);
            // 
            // lblY
            // 
            this.lblY.AutoSize = true;
            this.lblY.Location = new System.Drawing.Point(158, 28);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(17, 13);
            this.lblY.TabIndex = 4;
            this.lblY.Text = "Y:";
            // 
            // lblX
            // 
            this.lblX.AutoSize = true;
            this.lblX.Location = new System.Drawing.Point(13, 28);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(17, 13);
            this.lblX.TabIndex = 3;
            this.lblX.Text = "X:";
            // 
            // Editor_Tools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(537, 299);
            this.ControlBox = false;
            this.Controls.Add(this.panButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.butQuantity);
            this.Controls.Add(this.panPanel);
            this.Controls.Add(this.panCheckBox);
            this.Controls.Add(this.butClear);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.panTextBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmbTools);
            this.Controls.Add(this.List);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Editor_Tools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tool Editor";
            this.panButton.ResumeLayout(false);
            this.panButton.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panTextBox.ResumeLayout(false);
            this.panTextBox.PerformLayout();
            this.panPanel.ResumeLayout(false);
            this.panPanel.PerformLayout();
            this.panCheckBox.ResumeLayout(false);
            this.panCheckBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion
    public System.Windows.Forms.ListBox List;
    public System.Windows.Forms.ComboBox cmbTools;
    private System.Windows.Forms.GroupBox groupBox2;
    public System.Windows.Forms.CheckBox chkVisible;
    public System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label label3;
    public System.Windows.Forms.CheckBox chkTextBox_Password;
    public System.Windows.Forms.HScrollBar scrlTextBox_Width;
    private System.Windows.Forms.Label lblTextBox_Width;
    public System.Windows.Forms.HScrollBar scrlTextBox_Max_Characters;
    private System.Windows.Forms.Label lblTextBox_Max_Characters;
    public System.Windows.Forms.GroupBox panTextBox;
    public System.Windows.Forms.GroupBox panButton;
    public System.Windows.Forms.GroupBox panPanel;
    private System.Windows.Forms.Label lblPanel_Texture;
    public System.Windows.Forms.GroupBox panCheckBox;
    public System.Windows.Forms.TextBox txtCheckBox_Text;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button butQuantity;
    private System.Windows.Forms.Button butClear;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.Button butSave;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label lblY;
    private System.Windows.Forms.Label lblX;
    public System.Windows.Forms.NumericUpDown numY;
    public System.Windows.Forms.NumericUpDown numX;
    private System.Windows.Forms.Label lblButton_Texture;
    private System.Windows.Forms.Button butPanel_Texture;
    private System.Windows.Forms.Button butButton_Texture;
}