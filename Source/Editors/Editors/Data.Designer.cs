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
            this.label9 = new System.Windows.Forms.Label();
            this.numPoints = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numMax_Map_Items = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.numMax_Party_Members = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
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
            this.numMax_Name = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numMin_Name = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.numMax_Password = new System.Windows.Forms.NumericUpDown();
            this.label12 = new System.Windows.Forms.Label();
            this.numMin_Password = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Map_Items)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Party_Members)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Characters)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Players)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMin_Name)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Password)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMin_Password)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numMax_Map_Items);
            this.groupBox1.Controls.Add(this.numMax_Name);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.numMin_Name);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.numPoints);
            this.groupBox1.Controls.Add(this.label8);
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
            this.groupBox1.Size = new System.Drawing.Size(288, 358);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(163, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(116, 12);
            this.label9.TabIndex = 15;
            this.label9.Text = "(Need to restart the server)";
            // 
            // numPoints
            // 
            this.numPoints.Location = new System.Drawing.Point(9, 242);
            this.numPoints.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numPoints.Name = "numPoints";
            this.numPoints.Size = new System.Drawing.Size(132, 20);
            this.numPoints.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 226);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(141, 13);
            this.label8.TabIndex = 13;
            this.label8.Text = "Points earned when levelup:";
            // 
            // numMax_Map_Items
            // 
            this.numMax_Map_Items.Location = new System.Drawing.Point(147, 242);
            this.numMax_Map_Items.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numMax_Map_Items.Name = "numMax_Map_Items";
            this.numMax_Map_Items.Size = new System.Drawing.Size(132, 20);
            this.numMax_Map_Items.TabIndex = 12;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(144, 226);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(104, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Maximum map items:";
            // 
            // numMax_Party_Members
            // 
            this.numMax_Party_Members.Location = new System.Drawing.Point(147, 203);
            this.numMax_Party_Members.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numMax_Party_Members.Name = "numMax_Party_Members";
            this.numMax_Party_Members.Size = new System.Drawing.Size(132, 20);
            this.numMax_Party_Members.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(144, 187);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(125, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Maximum party members:";
            // 
            // txtWelcome
            // 
            this.txtWelcome.Location = new System.Drawing.Point(9, 80);
            this.txtWelcome.Name = "txtWelcome";
            this.txtWelcome.Size = new System.Drawing.Size(270, 20);
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
            this.numPort.Size = new System.Drawing.Size(270, 20);
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
            this.txtGame_Name.Size = new System.Drawing.Size(270, 20);
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
            this.numMax_Characters.Location = new System.Drawing.Point(9, 203);
            this.numMax_Characters.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMax_Characters.Name = "numMax_Characters";
            this.numMax_Characters.Size = new System.Drawing.Size(129, 20);
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
            this.label2.Location = new System.Drawing.Point(6, 187);
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
            this.numMax_Players.Size = new System.Drawing.Size(270, 20);
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
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Maximum players online:";
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(159, 376);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(141, 25);
            this.butCancel.TabIndex = 20;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSalve
            // 
            this.butSalve.Location = new System.Drawing.Point(12, 376);
            this.butSalve.Name = "butSalve";
            this.butSalve.Size = new System.Drawing.Size(141, 25);
            this.butSalve.TabIndex = 19;
            this.butSalve.Text = "Save";
            this.butSalve.UseVisualStyleBackColor = true;
            this.butSalve.Click += new System.EventHandler(this.butSave_Click);
            // 
            // numMax_Name
            // 
            this.numMax_Name.Location = new System.Drawing.Point(147, 282);
            this.numMax_Name.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMax_Name.Name = "numMax_Name";
            this.numMax_Name.Size = new System.Drawing.Size(132, 20);
            this.numMax_Name.TabIndex = 19;
            this.numMax_Name.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(144, 266);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(115, 13);
            this.label10.TabIndex = 18;
            this.label10.Text = "Maximum name length:";
            // 
            // numMin_Name
            // 
            this.numMin_Name.Location = new System.Drawing.Point(9, 282);
            this.numMin_Name.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMin_Name.Name = "numMin_Name";
            this.numMin_Name.Size = new System.Drawing.Size(129, 20);
            this.numMin_Name.TabIndex = 17;
            this.numMin_Name.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 266);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(112, 13);
            this.label11.TabIndex = 16;
            this.label11.Text = "Minimum name length:";
            // 
            // numMax_Password
            // 
            this.numMax_Password.Location = new System.Drawing.Point(159, 337);
            this.numMax_Password.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMax_Password.Name = "numMax_Password";
            this.numMax_Password.Size = new System.Drawing.Size(132, 20);
            this.numMax_Password.TabIndex = 23;
            this.numMax_Password.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(156, 321);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(134, 13);
            this.label12.TabIndex = 22;
            this.label12.Text = "Maximum password length:";
            // 
            // numMin_Password
            // 
            this.numMin_Password.Location = new System.Drawing.Point(21, 337);
            this.numMin_Password.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numMin_Password.Name = "numMin_Password";
            this.numMin_Password.Size = new System.Drawing.Size(129, 20);
            this.numMin_Password.TabIndex = 21;
            this.numMin_Password.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(18, 321);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(131, 13);
            this.label13.TabIndex = 20;
            this.label13.Text = "Minimum password length:";
            // 
            // Editor_Data
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(312, 414);
            this.ControlBox = false;
            this.Controls.Add(this.numMax_Password);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.numMin_Password);
            this.Controls.Add(this.butSalve);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Editor_Data";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Data Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Map_Items)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Party_Members)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Characters)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Players)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMin_Name)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMax_Password)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMin_Password)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
    public System.Windows.Forms.NumericUpDown numPoints;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    public System.Windows.Forms.NumericUpDown numMax_Name;
    private System.Windows.Forms.Label label10;
    public System.Windows.Forms.NumericUpDown numMin_Name;
    private System.Windows.Forms.Label label11;
    public System.Windows.Forms.NumericUpDown numMax_Password;
    private System.Windows.Forms.Label label12;
    public System.Windows.Forms.NumericUpDown numMin_Password;
    private System.Windows.Forms.Label label13;
}