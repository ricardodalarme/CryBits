partial class Editor_NPCs
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor_NPCs));
            this.List = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSayMsg = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.numTexture = new System.Windows.Forms.NumericUpDown();
            this.numSpawn = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numRange = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBehavior = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTexture = new System.Windows.Forms.Label();
            this.butTexture = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.butClear = new System.Windows.Forms.Button();
            this.butQuantity = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.numResistance = new System.Windows.Forms.NumericUpDown();
            this.numIntelligence = new System.Windows.Forms.NumericUpDown();
            this.numVitality = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numStrength = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numAgility = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numExperience = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numMP = new System.Windows.Forms.NumericUpDown();
            this.numHP = new System.Windows.Forms.NumericUpDown();
            this.lblMP = new System.Windows.Forms.Label();
            this.lblHP = new System.Windows.Forms.Label();
            this.numDrop_Amount = new System.Windows.Forms.NumericUpDown();
            this.cmbDrop_Item = new System.Windows.Forms.ComboBox();
            this.numDrop_Chance = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.butDrop_Delete = new System.Windows.Forms.Button();
            this.lstDrop = new System.Windows.Forms.ListBox();
            this.butDrop_Add = new System.Windows.Forms.Button();
            this.grpDrop_Add = new System.Windows.Forms.GroupBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.butItem_Ok = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTexture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRange)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numResistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntelligence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVitality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAgility)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExperience)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Amount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Chance)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.grpDrop_Add.SuspendLayout();
            this.SuspendLayout();
            // 
            // List
            // 
            this.List.FormattingEnabled = true;
            this.List.Location = new System.Drawing.Point(11, 12);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(202, 407);
            this.List.TabIndex = 9;
            this.List.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSayMsg);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.numTexture);
            this.groupBox1.Controls.Add(this.numSpawn);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.numRange);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbBehavior);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblTexture);
            this.groupBox1.Controls.Add(this.butTexture);
            this.groupBox1.Location = new System.Drawing.Point(219, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 239);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // txtSayMsg
            // 
            this.txtSayMsg.Location = new System.Drawing.Point(15, 81);
            this.txtSayMsg.Name = "txtSayMsg";
            this.txtSayMsg.Size = new System.Drawing.Size(279, 20);
            this.txtSayMsg.TabIndex = 38;
            this.txtSayMsg.Validated += new System.EventHandler(this.txtSayMsg_Validated);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(12, 63);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 13);
            this.label14.TabIndex = 37;
            this.label14.Text = "Say message:";
            // 
            // numTexture
            // 
            this.numTexture.Location = new System.Drawing.Point(15, 122);
            this.numTexture.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numTexture.Name = "numTexture";
            this.numTexture.Size = new System.Drawing.Size(187, 20);
            this.numTexture.TabIndex = 36;
            this.numTexture.ValueChanged += new System.EventHandler(this.numTexture_ValueChanged);
            // 
            // numSpawn
            // 
            this.numSpawn.Location = new System.Drawing.Point(159, 206);
            this.numSpawn.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numSpawn.Name = "numSpawn";
            this.numSpawn.Size = new System.Drawing.Size(135, 20);
            this.numSpawn.TabIndex = 35;
            this.numSpawn.ValueChanged += new System.EventHandler(this.numSpawn_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Location = new System.Drawing.Point(156, 190);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Spawn rate (sec):";
            // 
            // numRange
            // 
            this.numRange.Location = new System.Drawing.Point(15, 206);
            this.numRange.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numRange.Name = "numRange";
            this.numRange.Size = new System.Drawing.Size(135, 20);
            this.numRange.TabIndex = 33;
            this.numRange.ValueChanged += new System.EventHandler(this.numRange_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 190);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 32;
            this.label2.Text = "Range:";
            // 
            // cmbBehavior
            // 
            this.cmbBehavior.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBehavior.FormattingEnabled = true;
            this.cmbBehavior.Items.AddRange(new object[] {
            "Friendly",
            "Attack On Sight",
            "Attack When Attacked"});
            this.cmbBehavior.Location = new System.Drawing.Point(15, 163);
            this.cmbBehavior.Name = "cmbBehavior";
            this.cmbBehavior.Size = new System.Drawing.Size(279, 21);
            this.cmbBehavior.TabIndex = 31;
            this.cmbBehavior.SelectedIndexChanged += new System.EventHandler(this.cmbBehavior_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 147);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Behavior:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(14, 38);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(279, 20);
            this.txtName.TabIndex = 10;
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Name:";
            // 
            // lblTexture
            // 
            this.lblTexture.AutoSize = true;
            this.lblTexture.Location = new System.Drawing.Point(12, 106);
            this.lblTexture.Name = "lblTexture";
            this.lblTexture.Size = new System.Drawing.Size(46, 13);
            this.lblTexture.TabIndex = 29;
            this.lblTexture.Text = "Texture:";
            // 
            // butTexture
            // 
            this.butTexture.Location = new System.Drawing.Point(208, 122);
            this.butTexture.Name = "butTexture";
            this.butTexture.Size = new System.Drawing.Size(86, 19);
            this.butTexture.TabIndex = 28;
            this.butTexture.Text = "Select";
            this.butTexture.UseVisualStyleBackColor = true;
            this.butTexture.Click += new System.EventHandler(this.butTexture_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(219, 425);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(201, 25);
            this.butSave.TabIndex = 16;
            this.butSave.Text = "Save";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(632, 425);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(201, 25);
            this.butCancel.TabIndex = 17;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butClear
            // 
            this.butClear.Location = new System.Drawing.Point(426, 425);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(201, 25);
            this.butClear.TabIndex = 18;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // butQuantity
            // 
            this.butQuantity.Location = new System.Drawing.Point(12, 425);
            this.butQuantity.Name = "butQuantity";
            this.butQuantity.Size = new System.Drawing.Size(202, 25);
            this.butQuantity.TabIndex = 15;
            this.butQuantity.Text = "Change Quantity";
            this.butQuantity.UseVisualStyleBackColor = true;
            this.butQuantity.Click += new System.EventHandler(this.butQuantity_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox4);
            this.groupBox2.Controls.Add(this.numExperience);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.numMP);
            this.groupBox2.Controls.Add(this.numHP);
            this.groupBox2.Controls.Add(this.lblMP);
            this.groupBox2.Controls.Add(this.lblHP);
            this.groupBox2.Location = new System.Drawing.Point(219, 257);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 160);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Combat:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.numResistance);
            this.groupBox4.Controls.Add(this.numIntelligence);
            this.groupBox4.Controls.Add(this.numVitality);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.numStrength);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.numAgility);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Location = new System.Drawing.Point(9, 58);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(284, 99);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Attributes";
            // 
            // numResistance
            // 
            this.numResistance.Location = new System.Drawing.Point(96, 34);
            this.numResistance.Name = "numResistance";
            this.numResistance.Size = new System.Drawing.Size(85, 20);
            this.numResistance.TabIndex = 26;
            this.numResistance.ValueChanged += new System.EventHandler(this.numResistance_ValueChanged);
            // 
            // numIntelligence
            // 
            this.numIntelligence.Location = new System.Drawing.Point(187, 34);
            this.numIntelligence.Name = "numIntelligence";
            this.numIntelligence.Size = new System.Drawing.Size(85, 20);
            this.numIntelligence.TabIndex = 29;
            this.numIntelligence.ValueChanged += new System.EventHandler(this.numIntelligence_ValueChanged);
            // 
            // numVitality
            // 
            this.numVitality.Location = new System.Drawing.Point(96, 73);
            this.numVitality.Name = "numVitality";
            this.numVitality.Size = new System.Drawing.Size(85, 20);
            this.numVitality.TabIndex = 36;
            this.numVitality.ValueChanged += new System.EventHandler(this.numVitality_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(93, 57);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Vitality:";
            // 
            // numStrength
            // 
            this.numStrength.Location = new System.Drawing.Point(6, 34);
            this.numStrength.Name = "numStrength";
            this.numStrength.Size = new System.Drawing.Size(85, 20);
            this.numStrength.TabIndex = 25;
            this.numStrength.ValueChanged += new System.EventHandler(this.numStrength_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Strength:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(93, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Resistance:";
            // 
            // numAgility
            // 
            this.numAgility.Location = new System.Drawing.Point(6, 73);
            this.numAgility.Name = "numAgility";
            this.numAgility.Size = new System.Drawing.Size(85, 20);
            this.numAgility.TabIndex = 32;
            this.numAgility.ValueChanged += new System.EventHandler(this.numAgility_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Agility:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(184, 18);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Intelligence:";
            // 
            // numExperience
            // 
            this.numExperience.Location = new System.Drawing.Point(203, 32);
            this.numExperience.Name = "numExperience";
            this.numExperience.Size = new System.Drawing.Size(90, 20);
            this.numExperience.TabIndex = 34;
            this.numExperience.ValueChanged += new System.EventHandler(this.numExperience_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(200, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(63, 13);
            this.label9.TabIndex = 33;
            this.label9.Text = "Experience:";
            // 
            // numMP
            // 
            this.numMP.Location = new System.Drawing.Point(105, 32);
            this.numMP.Name = "numMP";
            this.numMP.Size = new System.Drawing.Size(90, 20);
            this.numMP.TabIndex = 22;
            this.numMP.ValueChanged += new System.EventHandler(this.numMP_ValueChanged);
            // 
            // numHP
            // 
            this.numHP.Location = new System.Drawing.Point(9, 32);
            this.numHP.Name = "numHP";
            this.numHP.Size = new System.Drawing.Size(90, 20);
            this.numHP.TabIndex = 21;
            this.numHP.ValueChanged += new System.EventHandler(this.numHP_ValueChanged);
            // 
            // lblMP
            // 
            this.lblMP.AutoSize = true;
            this.lblMP.Location = new System.Drawing.Point(102, 16);
            this.lblMP.Name = "lblMP";
            this.lblMP.Size = new System.Drawing.Size(26, 13);
            this.lblMP.TabIndex = 3;
            this.lblMP.Text = "MP:";
            // 
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(6, 16);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(25, 13);
            this.lblHP.TabIndex = 1;
            this.lblHP.Text = "HP:";
            // 
            // numDrop_Amount
            // 
            this.numDrop_Amount.Location = new System.Drawing.Point(29, 119);
            this.numDrop_Amount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDrop_Amount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDrop_Amount.Name = "numDrop_Amount";
            this.numDrop_Amount.Size = new System.Drawing.Size(122, 20);
            this.numDrop_Amount.TabIndex = 37;
            this.numDrop_Amount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cmbDrop_Item
            // 
            this.cmbDrop_Item.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDrop_Item.FormattingEnabled = true;
            this.cmbDrop_Item.Location = new System.Drawing.Point(29, 80);
            this.cmbDrop_Item.Name = "cmbDrop_Item";
            this.cmbDrop_Item.Size = new System.Drawing.Size(251, 21);
            this.cmbDrop_Item.TabIndex = 35;
            // 
            // numDrop_Chance
            // 
            this.numDrop_Chance.Location = new System.Drawing.Point(158, 119);
            this.numDrop_Chance.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDrop_Chance.Name = "numDrop_Chance";
            this.numDrop_Chance.Size = new System.Drawing.Size(122, 20);
            this.numDrop_Chance.TabIndex = 33;
            this.numDrop_Chance.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(155, 103);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(64, 13);
            this.label11.TabIndex = 32;
            this.label11.Text = "Chance: (%)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.butDrop_Delete);
            this.groupBox3.Controls.Add(this.lstDrop);
            this.groupBox3.Controls.Add(this.butDrop_Add);
            this.groupBox3.Location = new System.Drawing.Point(529, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(304, 211);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Drop items";
            // 
            // butDrop_Delete
            // 
            this.butDrop_Delete.Location = new System.Drawing.Point(152, 184);
            this.butDrop_Delete.Name = "butDrop_Delete";
            this.butDrop_Delete.Size = new System.Drawing.Size(139, 20);
            this.butDrop_Delete.TabIndex = 35;
            this.butDrop_Delete.Text = "Delete";
            this.butDrop_Delete.UseVisualStyleBackColor = true;
            this.butDrop_Delete.Click += new System.EventHandler(this.butDrop_Delete_Click);
            // 
            // lstDrop
            // 
            this.lstDrop.FormattingEnabled = true;
            this.lstDrop.Location = new System.Drawing.Point(10, 16);
            this.lstDrop.Name = "lstDrop";
            this.lstDrop.Size = new System.Drawing.Size(284, 160);
            this.lstDrop.TabIndex = 34;
            // 
            // butDrop_Add
            // 
            this.butDrop_Add.Location = new System.Drawing.Point(8, 184);
            this.butDrop_Add.Name = "butDrop_Add";
            this.butDrop_Add.Size = new System.Drawing.Size(138, 20);
            this.butDrop_Add.TabIndex = 33;
            this.butDrop_Add.Text = "Add";
            this.butDrop_Add.UseVisualStyleBackColor = true;
            this.butDrop_Add.Click += new System.EventHandler(this.butDrop_Add_Click);
            // 
            // grpDrop_Add
            // 
            this.grpDrop_Add.Controls.Add(this.numDrop_Amount);
            this.grpDrop_Add.Controls.Add(this.label15);
            this.grpDrop_Add.Controls.Add(this.numDrop_Chance);
            this.grpDrop_Add.Controls.Add(this.label11);
            this.grpDrop_Add.Controls.Add(this.cmbDrop_Item);
            this.grpDrop_Add.Controls.Add(this.label16);
            this.grpDrop_Add.Controls.Add(this.butItem_Ok);
            this.grpDrop_Add.Location = new System.Drawing.Point(529, 12);
            this.grpDrop_Add.Name = "grpDrop_Add";
            this.grpDrop_Add.Size = new System.Drawing.Size(304, 212);
            this.grpDrop_Add.TabIndex = 38;
            this.grpDrop_Add.TabStop = false;
            this.grpDrop_Add.Text = "Add Item";
            this.grpDrop_Add.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(26, 103);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(46, 13);
            this.label15.TabIndex = 31;
            this.label15.Text = "Amount:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(26, 64);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 30;
            this.label16.Text = "Item:";
            // 
            // butItem_Ok
            // 
            this.butItem_Ok.Location = new System.Drawing.Point(29, 143);
            this.butItem_Ok.Name = "butItem_Ok";
            this.butItem_Ok.Size = new System.Drawing.Size(251, 20);
            this.butItem_Ok.TabIndex = 29;
            this.butItem_Ok.Text = "Ok";
            this.butItem_Ok.UseVisualStyleBackColor = true;
            this.butItem_Ok.Click += new System.EventHandler(this.butItem_Ok_Click);
            // 
            // Editor_NPCs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(843, 460);
            this.ControlBox = false;
            this.Controls.Add(this.grpDrop_Add);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.butQuantity);
            this.Controls.Add(this.butClear);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.List);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Editor_NPCs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NPC Editor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTexture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRange)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numResistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntelligence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVitality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAgility)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExperience)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Amount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Chance)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.grpDrop_Add.ResumeLayout(false);
            this.grpDrop_Add.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion
    public System.Windows.Forms.ListBox List;
    private System.Windows.Forms.GroupBox groupBox1;
    public System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button butSave;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.Button butClear;
    private System.Windows.Forms.Button butQuantity;
    private System.Windows.Forms.Label lblTexture;
    private System.Windows.Forms.Button butTexture;
    private System.Windows.Forms.NumericUpDown numRange;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cmbBehavior;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.NumericUpDown numAgility;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown numIntelligence;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown numResistance;
    private System.Windows.Forms.NumericUpDown numStrength;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.NumericUpDown numMP;
    private System.Windows.Forms.NumericUpDown numHP;
    private System.Windows.Forms.Label lblMP;
    private System.Windows.Forms.Label lblHP;
    private System.Windows.Forms.NumericUpDown numSpawn;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.NumericUpDown numExperience;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.NumericUpDown numVitality;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.NumericUpDown numTexture;
    private System.Windows.Forms.NumericUpDown numDrop_Amount;
    private System.Windows.Forms.ComboBox cmbDrop_Item;
    private System.Windows.Forms.NumericUpDown numDrop_Chance;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.GroupBox groupBox4;
    public System.Windows.Forms.TextBox txtSayMsg;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.GroupBox grpDrop_Add;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Button butItem_Ok;
    private System.Windows.Forms.Button butDrop_Delete;
    public System.Windows.Forms.ListBox lstDrop;
    private System.Windows.Forms.Button butDrop_Add;
}