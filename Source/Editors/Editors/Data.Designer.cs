partial class Editor_Data
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor_Data));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtWelcome = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtGame_Name = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numMax_Characters = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numMax_Players = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.butCancel = new System.Windows.Forms.Button();
            this.butSalve = new System.Windows.Forms.Button();
            this.numMax_Party_Members = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.numMax_Map_Items = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Characters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Players)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Party_Members)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Map_Items)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numMax_Map_Items);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.numMax_Party_Members);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtWelcome);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.numPort);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtGame_Name);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numMax_Characters);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numMax_Players);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(263, 233);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // txtWelcome
            // 
            this.txtWelcome.Location = new System.Drawing.Point(9, 80);
            this.txtWelcome.Name = "txtWelcome";
            this.txtWelcome.Size = new System.Drawing.Size(246, 20);
            this.txtWelcome.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 64);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Welcome message:";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(9, 119);
            this.numPort.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(246, 20);
            this.numPort.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Port:";
            // 
            // txtGame_Name
            // 
            this.txtGame_Name.Location = new System.Drawing.Point(9, 41);
            this.txtGame_Name.Name = "txtGame_Name";
            this.txtGame_Name.Size = new System.Drawing.Size(246, 20);
            this.txtGame_Name.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Game name:";
            // 
            // numMax_Characters
            // 
            this.numMax_Characters.Location = new System.Drawing.Point(135, 161);
            this.numMax_Characters.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMax_Characters.Name = "numMax_Characters";
            this.numMax_Characters.Size = new System.Drawing.Size(120, 20);
            this.numMax_Characters.TabIndex = 5;
            this.numMax_Characters.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(132, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Maximum characters:";
            // 
            // numMax_Players
            // 
            this.numMax_Players.Location = new System.Drawing.Point(9, 161);
            this.numMax_Players.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMax_Players.Name = "numMax_Players";
            this.numMax_Players.Size = new System.Drawing.Size(120, 20);
            this.numMax_Players.TabIndex = 4;
            this.numMax_Players.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 145);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Maximum players:";
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(151, 251);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(125, 25);
            this.butCancel.TabIndex = 20;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSalve
            // 
            this.butSalve.Location = new System.Drawing.Point(13, 251);
            this.butSalve.Name = "butSalve";
            this.butSalve.Size = new System.Drawing.Size(129, 25);
            this.butSalve.TabIndex = 19;
            this.butSalve.Text = "Save";
            this.butSalve.UseVisualStyleBackColor = true;
            this.butSalve.Click += new System.EventHandler(this.butSave_Click);
            // 
            // numMax_Party_Members
            // 
            this.numMax_Party_Members.Location = new System.Drawing.Point(9, 200);
            this.numMax_Party_Members.Name = "numMax_Party_Members";
            this.numMax_Party_Members.Size = new System.Drawing.Size(120, 20);
            this.numMax_Party_Members.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 184);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Maximum party members:";
            // 
            // numMax_Map_Items
            // 
            this.numMax_Map_Items.Location = new System.Drawing.Point(135, 200);
            this.numMax_Map_Items.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numMax_Map_Items.Name = "numMax_Map_Items";
            this.numMax_Map_Items.Size = new System.Drawing.Size(120, 20);
            this.numMax_Map_Items.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(132, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Maximum map items:";
            // 
            // Editor_Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(288, 288);
            this.ControlBox = false;
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSalve);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Editor_Data";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Characters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Players)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Party_Members)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Map_Items)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.Button butSalve;
    public System.Windows.Forms.NumericUpDown numMax_Players;
    public System.Windows.Forms.NumericUpDown numMax_Characters;
    public System.Windows.Forms.TextBox txtGame_Name;
    public System.Windows.Forms.NumericUpDown numPort;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    public System.Windows.Forms.TextBox txtWelcome;
    public System.Windows.Forms.NumericUpDown numMax_Party_Members;
    private System.Windows.Forms.Label label6;
    public System.Windows.Forms.NumericUpDown numMax_Map_Items;
    private System.Windows.Forms.Label label7;
}