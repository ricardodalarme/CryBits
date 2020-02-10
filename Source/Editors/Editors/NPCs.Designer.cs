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
            this.numTexture = new System.Windows.Forms.NumericUpDown();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTexture = new System.Windows.Forms.Label();
            this.butTexture = new System.Windows.Forms.Button();
            this.txtSayMsg = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.numSpawn = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.numRange = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbBehavior = new System.Windows.Forms.ComboBox();
            this.butSave = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.butClear = new System.Windows.Forms.Button();
            this.butQuantity = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numResistance = new System.Windows.Forms.NumericUpDown();
            this.numExperience = new System.Windows.Forms.NumericUpDown();
            this.numIntelligence = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.numVitality = new System.Windows.Forms.NumericUpDown();
            this.numMP = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.numHP = new System.Windows.Forms.NumericUpDown();
            this.numStrength = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.lblMP = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblHP = new System.Windows.Forms.Label();
            this.numAgility = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.butAllie_Delete = new System.Windows.Forms.Button();
            this.butAllie_Add = new System.Windows.Forms.Button();
            this.lstAllies = new System.Windows.Forms.ListBox();
            this.label12 = new System.Windows.Forms.Label();
            this.chkAttackNPC = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.cmbMovement = new System.Windows.Forms.ComboBox();
            this.numFlee_Health = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.grpAllie_Add = new System.Windows.Forms.GroupBox();
            this.cmbAllie_NPC = new System.Windows.Forms.ComboBox();
            this.label17 = new System.Windows.Forms.Label();
            this.butAllie_Ok = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTexture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRange)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numResistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExperience)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntelligence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVitality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAgility)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Amount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Chance)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.grpDrop_Add.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFlee_Health)).BeginInit();
            this.grpAllie_Add.SuspendLayout();
            this.SuspendLayout();
            // 
            // List
            // 
            this.List.FormattingEnabled = true;
            this.List.Location = new System.Drawing.Point(11, 12);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(202, 472);
            this.List.TabIndex = 9;
            this.List.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numTexture);
            this.groupBox1.Controls.Add(this.txtName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblTexture);
            this.groupBox1.Controls.Add(this.butTexture);
            this.groupBox1.Location = new System.Drawing.Point(219, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(304, 103);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "General";
            // 
            // numTexture
            // 
            this.numTexture.Location = new System.Drawing.Point(14, 75);
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
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(14, 36);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(279, 20);
            this.txtName.TabIndex = 10;
            this.txtName.Validated += new System.EventHandler(this.txtName_Validated);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Name:";
            // 
            // lblTexture
            // 
            this.lblTexture.AutoSize = true;
            this.lblTexture.Location = new System.Drawing.Point(11, 59);
            this.lblTexture.Name = "lblTexture";
            this.lblTexture.Size = new System.Drawing.Size(46, 13);
            this.lblTexture.TabIndex = 29;
            this.lblTexture.Text = "Texture:";
            // 
            // butTexture
            // 
            this.butTexture.Location = new System.Drawing.Point(207, 75);
            this.butTexture.Name = "butTexture";
            this.butTexture.Size = new System.Drawing.Size(86, 19);
            this.butTexture.TabIndex = 28;
            this.butTexture.Text = "Select";
            this.butTexture.UseVisualStyleBackColor = true;
            this.butTexture.Click += new System.EventHandler(this.butTexture_Click);
            // 
            // txtSayMsg
            // 
            this.txtSayMsg.Location = new System.Drawing.Point(13, 108);
            this.txtSayMsg.Name = "txtSayMsg";
            this.txtSayMsg.Size = new System.Drawing.Size(279, 20);
            this.txtSayMsg.TabIndex = 38;
            this.txtSayMsg.TextChanged += new System.EventHandler(this.txtSayMsg_TextChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(10, 92);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 13);
            this.label14.TabIndex = 37;
            this.label14.Text = "Say message:";
            // 
            // numSpawn
            // 
            this.numSpawn.Location = new System.Drawing.Point(157, 147);
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
            this.label8.Location = new System.Drawing.Point(154, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(90, 13);
            this.label8.TabIndex = 34;
            this.label8.Text = "Spawn rate (sec):";
            // 
            // numRange
            // 
            this.numRange.Location = new System.Drawing.Point(13, 147);
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
            this.label2.Location = new System.Drawing.Point(10, 131);
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
            this.cmbBehavior.Location = new System.Drawing.Point(13, 18);
            this.cmbBehavior.Name = "cmbBehavior";
            this.cmbBehavior.Size = new System.Drawing.Size(279, 21);
            this.cmbBehavior.TabIndex = 31;
            this.cmbBehavior.SelectedIndexChanged += new System.EventHandler(this.cmbBehavior_SelectedIndexChanged);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(219, 492);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(201, 25);
            this.butSave.TabIndex = 16;
            this.butSave.Text = "Save All";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(632, 492);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(201, 25);
            this.butCancel.TabIndex = 17;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butClear
            // 
            this.butClear.Location = new System.Drawing.Point(425, 492);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(201, 25);
            this.butClear.TabIndex = 18;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // butQuantity
            // 
            this.butQuantity.Location = new System.Drawing.Point(11, 492);
            this.butQuantity.Name = "butQuantity";
            this.butQuantity.Size = new System.Drawing.Size(202, 25);
            this.butQuantity.TabIndex = 15;
            this.butQuantity.Text = "Change Quantity";
            this.butQuantity.UseVisualStyleBackColor = true;
            this.butQuantity.Click += new System.EventHandler(this.butQuantity_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numResistance);
            this.groupBox2.Controls.Add(this.numExperience);
            this.groupBox2.Controls.Add(this.numIntelligence);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.numVitality);
            this.groupBox2.Controls.Add(this.numMP);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.numHP);
            this.groupBox2.Controls.Add(this.numStrength);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.lblMP);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.lblHP);
            this.groupBox2.Controls.Add(this.numAgility);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(219, 348);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 137);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Base Attributes";
            // 
            // numResistance
            // 
            this.numResistance.Location = new System.Drawing.Point(109, 70);
            this.numResistance.Name = "numResistance";
            this.numResistance.Size = new System.Drawing.Size(88, 20);
            this.numResistance.TabIndex = 26;
            this.numResistance.ValueChanged += new System.EventHandler(this.numResistance_ValueChanged);
            // 
            // numExperience
            // 
            this.numExperience.Location = new System.Drawing.Point(203, 32);
            this.numExperience.Name = "numExperience";
            this.numExperience.Size = new System.Drawing.Size(88, 20);
            this.numExperience.TabIndex = 34;
            this.numExperience.ValueChanged += new System.EventHandler(this.numExperience_ValueChanged);
            // 
            // numIntelligence
            // 
            this.numIntelligence.Location = new System.Drawing.Point(204, 70);
            this.numIntelligence.Name = "numIntelligence";
            this.numIntelligence.Size = new System.Drawing.Size(88, 20);
            this.numIntelligence.TabIndex = 29;
            this.numIntelligence.ValueChanged += new System.EventHandler(this.numIntelligence_ValueChanged);
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
            // numVitality
            // 
            this.numVitality.Location = new System.Drawing.Point(109, 110);
            this.numVitality.Name = "numVitality";
            this.numVitality.Size = new System.Drawing.Size(88, 20);
            this.numVitality.TabIndex = 36;
            this.numVitality.ValueChanged += new System.EventHandler(this.numVitality_ValueChanged);
            // 
            // numMP
            // 
            this.numMP.Location = new System.Drawing.Point(109, 32);
            this.numMP.Name = "numMP";
            this.numMP.Size = new System.Drawing.Size(88, 20);
            this.numMP.TabIndex = 22;
            this.numMP.ValueChanged += new System.EventHandler(this.numMP_ValueChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(106, 94);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(40, 13);
            this.label10.TabIndex = 35;
            this.label10.Text = "Vitality:";
            // 
            // numHP
            // 
            this.numHP.Location = new System.Drawing.Point(14, 32);
            this.numHP.Name = "numHP";
            this.numHP.Size = new System.Drawing.Size(88, 20);
            this.numHP.TabIndex = 21;
            this.numHP.ValueChanged += new System.EventHandler(this.numHP_ValueChanged);
            // 
            // numStrength
            // 
            this.numStrength.Location = new System.Drawing.Point(13, 70);
            this.numStrength.Name = "numStrength";
            this.numStrength.Size = new System.Drawing.Size(88, 20);
            this.numStrength.TabIndex = 25;
            this.numStrength.ValueChanged += new System.EventHandler(this.numStrength_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 54);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Strength:";
            // 
            // lblMP
            // 
            this.lblMP.AutoSize = true;
            this.lblMP.Location = new System.Drawing.Point(106, 18);
            this.lblMP.Name = "lblMP";
            this.lblMP.Size = new System.Drawing.Size(26, 13);
            this.lblMP.TabIndex = 3;
            this.lblMP.Text = "MP:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(106, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Resistance:";
            // 
            // lblHP
            // 
            this.lblHP.AutoSize = true;
            this.lblHP.Location = new System.Drawing.Point(11, 16);
            this.lblHP.Name = "lblHP";
            this.lblHP.Size = new System.Drawing.Size(25, 13);
            this.lblHP.TabIndex = 1;
            this.lblHP.Text = "HP:";
            // 
            // numAgility
            // 
            this.numAgility.Location = new System.Drawing.Point(13, 110);
            this.numAgility.Name = "numAgility";
            this.numAgility.Size = new System.Drawing.Size(88, 20);
            this.numAgility.TabIndex = 32;
            this.numAgility.ValueChanged += new System.EventHandler(this.numAgility_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(201, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Intelligence:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 94);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Agility:";
            // 
            // numDrop_Amount
            // 
            this.numDrop_Amount.Location = new System.Drawing.Point(28, 116);
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
            this.cmbDrop_Item.Location = new System.Drawing.Point(28, 77);
            this.cmbDrop_Item.Name = "cmbDrop_Item";
            this.cmbDrop_Item.Size = new System.Drawing.Size(251, 21);
            this.cmbDrop_Item.TabIndex = 35;
            // 
            // numDrop_Chance
            // 
            this.numDrop_Chance.Location = new System.Drawing.Point(157, 116);
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
            this.label11.Location = new System.Drawing.Point(154, 100);
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
            this.groupBox3.Location = new System.Drawing.Point(529, 271);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(304, 215);
            this.groupBox3.TabIndex = 37;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Drop items";
            // 
            // butDrop_Delete
            // 
            this.butDrop_Delete.Location = new System.Drawing.Point(155, 185);
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
            this.butDrop_Add.Location = new System.Drawing.Point(10, 185);
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
            this.grpDrop_Add.Location = new System.Drawing.Point(529, 271);
            this.grpDrop_Add.Name = "grpDrop_Add";
            this.grpDrop_Add.Size = new System.Drawing.Size(304, 215);
            this.grpDrop_Add.TabIndex = 38;
            this.grpDrop_Add.TabStop = false;
            this.grpDrop_Add.Text = "Add Item";
            this.grpDrop_Add.Visible = false;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(25, 100);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(46, 13);
            this.label15.TabIndex = 31;
            this.label15.Text = "Amount:";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(25, 61);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 30;
            this.label16.Text = "Item:";
            // 
            // butItem_Ok
            // 
            this.butItem_Ok.Location = new System.Drawing.Point(28, 140);
            this.butItem_Ok.Name = "butItem_Ok";
            this.butItem_Ok.Size = new System.Drawing.Size(251, 20);
            this.butItem_Ok.TabIndex = 29;
            this.butItem_Ok.Text = "Ok";
            this.butItem_Ok.UseVisualStyleBackColor = true;
            this.butItem_Ok.Click += new System.EventHandler(this.butItem_Ok_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.butAllie_Delete);
            this.groupBox5.Controls.Add(this.butAllie_Add);
            this.groupBox5.Controls.Add(this.lstAllies);
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.chkAttackNPC);
            this.groupBox5.Location = new System.Drawing.Point(531, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(304, 253);
            this.groupBox5.TabIndex = 39;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "NPC vs NPC";
            // 
            // butAllie_Delete
            // 
            this.butAllie_Delete.Location = new System.Drawing.Point(155, 221);
            this.butAllie_Delete.Name = "butAllie_Delete";
            this.butAllie_Delete.Size = new System.Drawing.Size(139, 20);
            this.butAllie_Delete.TabIndex = 36;
            this.butAllie_Delete.Text = "Delete";
            this.butAllie_Delete.UseVisualStyleBackColor = true;
            this.butAllie_Delete.Click += new System.EventHandler(this.butAllie_Delete_Click);
            // 
            // butAllie_Add
            // 
            this.butAllie_Add.Location = new System.Drawing.Point(10, 221);
            this.butAllie_Add.Name = "butAllie_Add";
            this.butAllie_Add.Size = new System.Drawing.Size(139, 20);
            this.butAllie_Add.TabIndex = 35;
            this.butAllie_Add.Text = "Add";
            this.butAllie_Add.UseVisualStyleBackColor = true;
            this.butAllie_Add.Click += new System.EventHandler(this.butAllie_Add_Click);
            // 
            // lstAllies
            // 
            this.lstAllies.FormattingEnabled = true;
            this.lstAllies.Location = new System.Drawing.Point(10, 55);
            this.lstAllies.Name = "lstAllies";
            this.lstAllies.Size = new System.Drawing.Size(284, 160);
            this.lstAllies.TabIndex = 2;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 39);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(34, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Allies:";
            // 
            // chkAttackNPC
            // 
            this.chkAttackNPC.AutoSize = true;
            this.chkAttackNPC.Location = new System.Drawing.Point(10, 19);
            this.chkAttackNPC.Name = "chkAttackNPC";
            this.chkAttackNPC.Size = new System.Drawing.Size(71, 17);
            this.chkAttackNPC.TabIndex = 0;
            this.chkAttackNPC.Text = "Enabled?";
            this.chkAttackNPC.UseVisualStyleBackColor = true;
            this.chkAttackNPC.CheckedChanged += new System.EventHandler(this.chkAttackNPC_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtSayMsg);
            this.groupBox6.Controls.Add(this.cmbMovement);
            this.groupBox6.Controls.Add(this.numFlee_Health);
            this.groupBox6.Controls.Add(this.cmbBehavior);
            this.groupBox6.Controls.Add(this.numSpawn);
            this.groupBox6.Controls.Add(this.numRange);
            this.groupBox6.Controls.Add(this.label14);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Location = new System.Drawing.Point(219, 121);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(304, 220);
            this.groupBox6.TabIndex = 40;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Behaviour";
            // 
            // cmbMovement
            // 
            this.cmbMovement.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMovement.FormattingEnabled = true;
            this.cmbMovement.Items.AddRange(new object[] {
            "Move Randomly",
            "Turn Randomly",
            "Stand Still"});
            this.cmbMovement.Location = new System.Drawing.Point(13, 63);
            this.cmbMovement.Name = "cmbMovement";
            this.cmbMovement.Size = new System.Drawing.Size(279, 21);
            this.cmbMovement.TabIndex = 40;
            this.cmbMovement.SelectedIndexChanged += new System.EventHandler(this.cmbMovement_SelectedIndexChanged);
            // 
            // numFlee_Health
            // 
            this.numFlee_Health.Location = new System.Drawing.Point(13, 190);
            this.numFlee_Health.Name = "numFlee_Health";
            this.numFlee_Health.Size = new System.Drawing.Size(279, 20);
            this.numFlee_Health.TabIndex = 37;
            this.numFlee_Health.ValueChanged += new System.EventHandler(this.numFlee_Health_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(10, 47);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 13);
            this.label13.TabIndex = 39;
            this.label13.Text = "Movement:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Flee health (%):";
            // 
            // grpAllie_Add
            // 
            this.grpAllie_Add.Controls.Add(this.cmbAllie_NPC);
            this.grpAllie_Add.Controls.Add(this.label17);
            this.grpAllie_Add.Controls.Add(this.butAllie_Ok);
            this.grpAllie_Add.Location = new System.Drawing.Point(531, 12);
            this.grpAllie_Add.Name = "grpAllie_Add";
            this.grpAllie_Add.Size = new System.Drawing.Size(304, 253);
            this.grpAllie_Add.TabIndex = 41;
            this.grpAllie_Add.TabStop = false;
            this.grpAllie_Add.Text = "Add Allie";
            this.grpAllie_Add.Visible = false;
            // 
            // cmbAllie_NPC
            // 
            this.cmbAllie_NPC.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAllie_NPC.FormattingEnabled = true;
            this.cmbAllie_NPC.Location = new System.Drawing.Point(26, 118);
            this.cmbAllie_NPC.Name = "cmbAllie_NPC";
            this.cmbAllie_NPC.Size = new System.Drawing.Size(251, 21);
            this.cmbAllie_NPC.TabIndex = 38;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(23, 102);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 13);
            this.label17.TabIndex = 37;
            this.label17.Text = "NPC:";
            // 
            // butAllie_Ok
            // 
            this.butAllie_Ok.Location = new System.Drawing.Point(26, 144);
            this.butAllie_Ok.Name = "butAllie_Ok";
            this.butAllie_Ok.Size = new System.Drawing.Size(251, 20);
            this.butAllie_Ok.TabIndex = 36;
            this.butAllie_Ok.Text = "Ok";
            this.butAllie_Ok.UseVisualStyleBackColor = true;
            this.butAllie_Ok.Click += new System.EventHandler(this.butAllie_Ok_Click);
            // 
            // Editor_NPCs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 527);
            this.ControlBox = false;
            this.Controls.Add(this.grpAllie_Add);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
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
            ((System.ComponentModel.ISupportInitialize)(this.numResistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numExperience)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntelligence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVitality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAgility)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Amount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDrop_Chance)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.grpDrop_Add.ResumeLayout(false);
            this.grpDrop_Add.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numFlee_Health)).EndInit();
            this.grpAllie_Add.ResumeLayout(false);
            this.grpAllie_Add.PerformLayout();
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
    private System.Windows.Forms.GroupBox groupBox5;
    private System.Windows.Forms.ListBox lstAllies;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.CheckBox chkAttackNPC;
    private System.Windows.Forms.Button butAllie_Delete;
    private System.Windows.Forms.Button butAllie_Add;
    private System.Windows.Forms.GroupBox groupBox6;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.NumericUpDown numFlee_Health;
    private System.Windows.Forms.ComboBox cmbMovement;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.GroupBox grpAllie_Add;
    private System.Windows.Forms.ComboBox cmbAllie_NPC;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.Button butAllie_Ok;
}