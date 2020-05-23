partial class Editor_Classes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor_Classes));
            this.grpGeneral = new System.Windows.Forms.GroupBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.butSave = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.grpAttributes = new System.Windows.Forms.GroupBox();
            this.numAgility = new System.Windows.Forms.NumericUpDown();
            this.numVitality = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.numIntelligence = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.numResistance = new System.Windows.Forms.NumericUpDown();
            this.numStrength = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numMP = new System.Windows.Forms.NumericUpDown();
            this.numHP = new System.Windows.Forms.NumericUpDown();
            this.lblMP = new System.Windows.Forms.Label();
            this.lblHP = new System.Windows.Forms.Label();
            this.grpTexture = new System.Windows.Forms.GroupBox();
            this.butFDelete = new System.Windows.Forms.Button();
            this.butMDelete = new System.Windows.Forms.Button();
            this.lstFemale = new System.Windows.Forms.ListBox();
            this.lstMale = new System.Windows.Forms.ListBox();
            this.lblFTexture = new System.Windows.Forms.Label();
            this.butFTexture = new System.Windows.Forms.Button();
            this.lblMTexture = new System.Windows.Forms.Label();
            this.butMTexture = new System.Windows.Forms.Button();
            this.grpSpawn = new System.Windows.Forms.GroupBox();
            this.cmbSpawn_Map = new System.Windows.Forms.ComboBox();
            this.numSpawn_Y = new System.Windows.Forms.NumericUpDown();
            this.numSpawn_X = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbSpawn_Direction = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.grpDrop = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.butItem_Delete = new System.Windows.Forms.Button();
            this.lstItems = new System.Windows.Forms.ListBox();
            this.butItem_Add = new System.Windows.Forms.Button();
            this.grpItem_Add = new System.Windows.Forms.GroupBox();
            this.numItem_Amount = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.cmbItems = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.butItem_Ok = new System.Windows.Forms.Button();
            this.butRemove = new System.Windows.Forms.Button();
            this.butNew = new System.Windows.Forms.Button();
            this.List = new System.Windows.Forms.TreeView();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.grpGeneral.SuspendLayout();
            this.grpAttributes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAgility)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVitality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntelligence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numResistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHP)).BeginInit();
            this.grpTexture.SuspendLayout();
            this.grpSpawn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawn_Y)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawn_X)).BeginInit();
            this.grpDrop.SuspendLayout();
            this.grpItem_Add.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numItem_Amount)).BeginInit();
            this.SuspendLayout();
            // 
            // grpGeneral
            // 
            this.grpGeneral.Controls.Add(this.txtDescription);
            this.grpGeneral.Controls.Add(this.label11);
            this.grpGeneral.Controls.Add(this.txtName);
            this.grpGeneral.Controls.Add(this.label3);
            this.grpGeneral.Location = new System.Drawing.Point(221, 12);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(304, 161);
            this.grpGeneral.TabIndex = 14;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(9, 81);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(285, 69);
            this.txtDescription.TabIndex = 12;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 13);
            this.label11.TabIndex = 11;
            this.label11.Text = "Description:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(9, 37);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(285, 20);
            this.txtName.TabIndex = 10;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Name:";
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(219, 438);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(306, 25);
            this.butSave.TabIndex = 16;
            this.butSave.Text = "Save All";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(531, 438);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(304, 25);
            this.butCancel.TabIndex = 17;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // grpAttributes
            // 
            this.grpAttributes.Controls.Add(this.numAgility);
            this.grpAttributes.Controls.Add(this.numVitality);
            this.grpAttributes.Controls.Add(this.label4);
            this.grpAttributes.Controls.Add(this.label6);
            this.grpAttributes.Controls.Add(this.numIntelligence);
            this.grpAttributes.Controls.Add(this.label5);
            this.grpAttributes.Controls.Add(this.numResistance);
            this.grpAttributes.Controls.Add(this.numStrength);
            this.grpAttributes.Controls.Add(this.label1);
            this.grpAttributes.Controls.Add(this.label2);
            this.grpAttributes.Controls.Add(this.numMP);
            this.grpAttributes.Controls.Add(this.numHP);
            this.grpAttributes.Controls.Add(this.lblMP);
            this.grpAttributes.Controls.Add(this.lblHP);
            this.grpAttributes.Location = new System.Drawing.Point(221, 179);
            this.grpAttributes.Name = "grpAttributes";
            this.grpAttributes.Size = new System.Drawing.Size(304, 140);
            this.grpAttributes.TabIndex = 20;
            this.grpAttributes.TabStop = false;
            this.grpAttributes.Text = "Base Attributes";
            // 
            // numAgility
            // 
            this.numAgility.Location = new System.Drawing.Point(9, 113);
            this.numAgility.Name = "numAgility";
            this.numAgility.Size = new System.Drawing.Size(90, 20);
            this.numAgility.TabIndex = 32;
            this.numAgility.ValueChanged += new System.EventHandler(this.numAgility_ValueChanged);
            // 
            // numVitality
            // 
            this.numVitality.Location = new System.Drawing.Point(106, 113);
            this.numVitality.Name = "numVitality";
            this.numVitality.Size = new System.Drawing.Size(90, 20);
            this.numVitality.TabIndex = 34;
            this.numVitality.ValueChanged += new System.EventHandler(this.numVitality_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(103, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 33;
            this.label4.Text = "Vitality:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 97);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 31;
            this.label6.Text = "Agility:";
            // 
            // numIntelligence
            // 
            this.numIntelligence.Location = new System.Drawing.Point(203, 71);
            this.numIntelligence.Name = "numIntelligence";
            this.numIntelligence.Size = new System.Drawing.Size(90, 20);
            this.numIntelligence.TabIndex = 29;
            this.numIntelligence.ValueChanged += new System.EventHandler(this.numIntelligence_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(200, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 27;
            this.label5.Text = "Intelligence:";
            // 
            // numResistance
            // 
            this.numResistance.Location = new System.Drawing.Point(106, 71);
            this.numResistance.Name = "numResistance";
            this.numResistance.Size = new System.Drawing.Size(90, 20);
            this.numResistance.TabIndex = 26;
            this.numResistance.ValueChanged += new System.EventHandler(this.numResistance_ValueChanged);
            // 
            // numStrength
            // 
            this.numStrength.Location = new System.Drawing.Point(10, 72);
            this.numStrength.Name = "numStrength";
            this.numStrength.Size = new System.Drawing.Size(90, 20);
            this.numStrength.TabIndex = 25;
            this.numStrength.ValueChanged += new System.EventHandler(this.numStrength_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(103, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 13);
            this.label1.TabIndex = 24;
            this.label1.Text = "Resistance:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Strength:";
            // 
            // numMP
            // 
            this.numMP.Location = new System.Drawing.Point(154, 32);
            this.numMP.Name = "numMP";
            this.numMP.Size = new System.Drawing.Size(139, 20);
            this.numMP.TabIndex = 22;
            this.numMP.ValueChanged += new System.EventHandler(this.numMP_ValueChanged);
            // 
            // numHP
            // 
            this.numHP.Location = new System.Drawing.Point(9, 32);
            this.numHP.Name = "numHP";
            this.numHP.Size = new System.Drawing.Size(139, 20);
            this.numHP.TabIndex = 21;
            this.numHP.ValueChanged += new System.EventHandler(this.numHP_ValueChanged);
            // 
            // lblMP
            // 
            this.lblMP.AutoSize = true;
            this.lblMP.Location = new System.Drawing.Point(156, 16);
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
            // grpTexture
            // 
            this.grpTexture.Controls.Add(this.butFDelete);
            this.grpTexture.Controls.Add(this.butMDelete);
            this.grpTexture.Controls.Add(this.lstFemale);
            this.grpTexture.Controls.Add(this.lstMale);
            this.grpTexture.Controls.Add(this.lblFTexture);
            this.grpTexture.Controls.Add(this.butFTexture);
            this.grpTexture.Controls.Add(this.lblMTexture);
            this.grpTexture.Controls.Add(this.butMTexture);
            this.grpTexture.Location = new System.Drawing.Point(531, 12);
            this.grpTexture.Name = "grpTexture";
            this.grpTexture.Size = new System.Drawing.Size(304, 201);
            this.grpTexture.TabIndex = 19;
            this.grpTexture.TabStop = false;
            this.grpTexture.Text = "Textures:";
            // 
            // butFDelete
            // 
            this.butFDelete.Location = new System.Drawing.Point(225, 172);
            this.butFDelete.Name = "butFDelete";
            this.butFDelete.Size = new System.Drawing.Size(68, 20);
            this.butFDelete.TabIndex = 35;
            this.butFDelete.Text = "Delete";
            this.butFDelete.UseVisualStyleBackColor = true;
            this.butFDelete.Click += new System.EventHandler(this.butFDelete_Click);
            // 
            // butMDelete
            // 
            this.butMDelete.Location = new System.Drawing.Point(79, 172);
            this.butMDelete.Name = "butMDelete";
            this.butMDelete.Size = new System.Drawing.Size(68, 20);
            this.butMDelete.TabIndex = 34;
            this.butMDelete.Text = "Delete";
            this.butMDelete.UseVisualStyleBackColor = true;
            this.butMDelete.Click += new System.EventHandler(this.butMDelete_Click);
            // 
            // lstFemale
            // 
            this.lstFemale.FormattingEnabled = true;
            this.lstFemale.Location = new System.Drawing.Point(155, 40);
            this.lstFemale.Name = "lstFemale";
            this.lstFemale.Size = new System.Drawing.Size(139, 121);
            this.lstFemale.TabIndex = 33;
            // 
            // lstMale
            // 
            this.lstMale.FormattingEnabled = true;
            this.lstMale.Location = new System.Drawing.Point(9, 40);
            this.lstMale.Name = "lstMale";
            this.lstMale.Size = new System.Drawing.Size(139, 121);
            this.lstMale.TabIndex = 32;
            // 
            // lblFTexture
            // 
            this.lblFTexture.AutoSize = true;
            this.lblFTexture.Location = new System.Drawing.Point(152, 24);
            this.lblFTexture.Name = "lblFTexture";
            this.lblFTexture.Size = new System.Drawing.Size(47, 13);
            this.lblFTexture.TabIndex = 31;
            this.lblFTexture.Text = "Female: ";
            // 
            // butFTexture
            // 
            this.butFTexture.Location = new System.Drawing.Point(154, 172);
            this.butFTexture.Name = "butFTexture";
            this.butFTexture.Size = new System.Drawing.Size(68, 20);
            this.butFTexture.TabIndex = 30;
            this.butFTexture.Text = "Add";
            this.butFTexture.UseVisualStyleBackColor = true;
            this.butFTexture.Click += new System.EventHandler(this.butFTexture_Click);
            // 
            // lblMTexture
            // 
            this.lblMTexture.AutoSize = true;
            this.lblMTexture.Location = new System.Drawing.Point(7, 24);
            this.lblMTexture.Name = "lblMTexture";
            this.lblMTexture.Size = new System.Drawing.Size(33, 13);
            this.lblMTexture.TabIndex = 29;
            this.lblMTexture.Text = "Male:";
            // 
            // butMTexture
            // 
            this.butMTexture.Location = new System.Drawing.Point(8, 172);
            this.butMTexture.Name = "butMTexture";
            this.butMTexture.Size = new System.Drawing.Size(68, 20);
            this.butMTexture.TabIndex = 28;
            this.butMTexture.Text = "Add";
            this.butMTexture.UseVisualStyleBackColor = true;
            this.butMTexture.Click += new System.EventHandler(this.butMTexture_Click);
            // 
            // grpSpawn
            // 
            this.grpSpawn.Controls.Add(this.cmbSpawn_Map);
            this.grpSpawn.Controls.Add(this.numSpawn_Y);
            this.grpSpawn.Controls.Add(this.numSpawn_X);
            this.grpSpawn.Controls.Add(this.label9);
            this.grpSpawn.Controls.Add(this.cmbSpawn_Direction);
            this.grpSpawn.Controls.Add(this.label10);
            this.grpSpawn.Controls.Add(this.label8);
            this.grpSpawn.Controls.Add(this.label7);
            this.grpSpawn.Location = new System.Drawing.Point(221, 325);
            this.grpSpawn.Name = "grpSpawn";
            this.grpSpawn.Size = new System.Drawing.Size(304, 106);
            this.grpSpawn.TabIndex = 35;
            this.grpSpawn.TabStop = false;
            this.grpSpawn.Text = "Spawn";
            // 
            // cmbSpawn_Map
            // 
            this.cmbSpawn_Map.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpawn_Map.FormattingEnabled = true;
            this.cmbSpawn_Map.Items.AddRange(new object[] {
            "Up",
            "Down",
            "Left",
            "Rigth"});
            this.cmbSpawn_Map.Location = new System.Drawing.Point(9, 35);
            this.cmbSpawn_Map.Name = "cmbSpawn_Map";
            this.cmbSpawn_Map.Size = new System.Drawing.Size(139, 21);
            this.cmbSpawn_Map.TabIndex = 29;
            this.cmbSpawn_Map.SelectedIndexChanged += new System.EventHandler(this.cmbSpawn_Map_SelectedIndexChanged);
            // 
            // numSpawn_Y
            // 
            this.numSpawn_Y.Location = new System.Drawing.Point(155, 78);
            this.numSpawn_Y.Name = "numSpawn_Y";
            this.numSpawn_Y.Size = new System.Drawing.Size(138, 20);
            this.numSpawn_Y.TabIndex = 27;
            this.numSpawn_Y.ValueChanged += new System.EventHandler(this.numSpawn_Y_ValueChanged);
            // 
            // numSpawn_X
            // 
            this.numSpawn_X.Location = new System.Drawing.Point(9, 78);
            this.numSpawn_X.Name = "numSpawn_X";
            this.numSpawn_X.Size = new System.Drawing.Size(139, 20);
            this.numSpawn_X.TabIndex = 27;
            this.numSpawn_X.ValueChanged += new System.EventHandler(this.numSpawn_X_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(17, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "X:";
            // 
            // cmbSpawn_Direction
            // 
            this.cmbSpawn_Direction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSpawn_Direction.FormattingEnabled = true;
            this.cmbSpawn_Direction.Items.AddRange(new object[] {
            "Up",
            "Down",
            "Left",
            "Rigth"});
            this.cmbSpawn_Direction.Location = new System.Drawing.Point(154, 35);
            this.cmbSpawn_Direction.Name = "cmbSpawn_Direction";
            this.cmbSpawn_Direction.Size = new System.Drawing.Size(139, 21);
            this.cmbSpawn_Direction.TabIndex = 28;
            this.cmbSpawn_Direction.SelectedIndexChanged += new System.EventHandler(this.cmbSpawn_Direction_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(152, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 13);
            this.label10.TabIndex = 26;
            this.label10.Text = "Y:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(151, 20);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Direction:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 20);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 13);
            this.label7.TabIndex = 22;
            this.label7.Text = "Map:";
            // 
            // grpDrop
            // 
            this.grpDrop.Controls.Add(this.label12);
            this.grpDrop.Controls.Add(this.butItem_Delete);
            this.grpDrop.Controls.Add(this.lstItems);
            this.grpDrop.Controls.Add(this.butItem_Add);
            this.grpDrop.Location = new System.Drawing.Point(531, 219);
            this.grpDrop.Name = "grpDrop";
            this.grpDrop.Size = new System.Drawing.Size(304, 212);
            this.grpDrop.TabIndex = 36;
            this.grpDrop.TabStop = false;
            this.grpDrop.Text = "Initial Items";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(61, 166);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(184, 12);
            this.label12.TabIndex = 36;
            this.label12.Text = "(Equipments will be automatically equipped)";
            // 
            // butItem_Delete
            // 
            this.butItem_Delete.Location = new System.Drawing.Point(152, 184);
            this.butItem_Delete.Name = "butItem_Delete";
            this.butItem_Delete.Size = new System.Drawing.Size(139, 20);
            this.butItem_Delete.TabIndex = 35;
            this.butItem_Delete.Text = "Delete";
            this.butItem_Delete.UseVisualStyleBackColor = true;
            this.butItem_Delete.Click += new System.EventHandler(this.butItem_Delete_Click);
            // 
            // lstItems
            // 
            this.lstItems.FormattingEnabled = true;
            this.lstItems.Location = new System.Drawing.Point(10, 16);
            this.lstItems.Name = "lstItems";
            this.lstItems.Size = new System.Drawing.Size(284, 147);
            this.lstItems.TabIndex = 34;
            // 
            // butItem_Add
            // 
            this.butItem_Add.Location = new System.Drawing.Point(8, 184);
            this.butItem_Add.Name = "butItem_Add";
            this.butItem_Add.Size = new System.Drawing.Size(138, 20);
            this.butItem_Add.TabIndex = 33;
            this.butItem_Add.Text = "Add";
            this.butItem_Add.UseVisualStyleBackColor = true;
            this.butItem_Add.Click += new System.EventHandler(this.butItem_Add_Click);
            // 
            // grpItem_Add
            // 
            this.grpItem_Add.Controls.Add(this.numItem_Amount);
            this.grpItem_Add.Controls.Add(this.label13);
            this.grpItem_Add.Controls.Add(this.cmbItems);
            this.grpItem_Add.Controls.Add(this.label16);
            this.grpItem_Add.Controls.Add(this.butItem_Ok);
            this.grpItem_Add.Location = new System.Drawing.Point(531, 219);
            this.grpItem_Add.Name = "grpItem_Add";
            this.grpItem_Add.Size = new System.Drawing.Size(304, 212);
            this.grpItem_Add.TabIndex = 38;
            this.grpItem_Add.TabStop = false;
            this.grpItem_Add.Text = "Add Item";
            this.grpItem_Add.Visible = false;
            // 
            // numItem_Amount
            // 
            this.numItem_Amount.Location = new System.Drawing.Point(29, 117);
            this.numItem_Amount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numItem_Amount.Name = "numItem_Amount";
            this.numItem_Amount.Size = new System.Drawing.Size(251, 20);
            this.numItem_Amount.TabIndex = 32;
            this.numItem_Amount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(26, 103);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(46, 13);
            this.label13.TabIndex = 31;
            this.label13.Text = "Amount:";
            // 
            // cmbItems
            // 
            this.cmbItems.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItems.FormattingEnabled = true;
            this.cmbItems.Location = new System.Drawing.Point(29, 79);
            this.cmbItems.Name = "cmbItems";
            this.cmbItems.Size = new System.Drawing.Size(251, 21);
            this.cmbItems.TabIndex = 0;
            this.cmbItems.SelectedIndexChanged += new System.EventHandler(this.cmbItems_SelectedIndexChanged);
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
            // butRemove
            // 
            this.butRemove.Location = new System.Drawing.Point(115, 438);
            this.butRemove.Name = "butRemove";
            this.butRemove.Size = new System.Drawing.Size(98, 25);
            this.butRemove.TabIndex = 40;
            this.butRemove.Text = "Remove";
            this.butRemove.UseVisualStyleBackColor = true;
            this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
            // 
            // butNew
            // 
            this.butNew.Location = new System.Drawing.Point(11, 438);
            this.butNew.Name = "butNew";
            this.butNew.Size = new System.Drawing.Size(98, 25);
            this.butNew.TabIndex = 39;
            this.butNew.Text = "New";
            this.butNew.UseVisualStyleBackColor = true;
            this.butNew.Click += new System.EventHandler(this.butNew_Click);
            // 
            // List
            // 
            this.List.HideSelection = false;
            this.List.Location = new System.Drawing.Point(12, 39);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(201, 393);
            this.List.TabIndex = 41;
            this.List.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.List_AfterSelect);
            // 
            // txtFilter
            // 
            this.txtFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilter.Location = new System.Drawing.Point(12, 13);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(201, 20);
            this.txtFilter.TabIndex = 42;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // Editor_Classes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(847, 475);
            this.ControlBox = false;
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.List);
            this.Controls.Add(this.butRemove);
            this.Controls.Add(this.butNew);
            this.Controls.Add(this.grpItem_Add);
            this.Controls.Add(this.grpDrop);
            this.Controls.Add(this.grpSpawn);
            this.Controls.Add(this.grpAttributes);
            this.Controls.Add(this.grpTexture);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.grpGeneral);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Editor_Classes";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Class Editor";
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.grpAttributes.ResumeLayout(false);
            this.grpAttributes.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAgility)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numVitality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numIntelligence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numResistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStrength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numHP)).EndInit();
            this.grpTexture.ResumeLayout(false);
            this.grpTexture.PerformLayout();
            this.grpSpawn.ResumeLayout(false);
            this.grpSpawn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawn_Y)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSpawn_X)).EndInit();
            this.grpDrop.ResumeLayout(false);
            this.grpDrop.PerformLayout();
            this.grpItem_Add.ResumeLayout(false);
            this.grpItem_Add.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numItem_Amount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.GroupBox grpGeneral;
    public System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button butSave;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.GroupBox grpAttributes;
    private System.Windows.Forms.Label lblMP;
    private System.Windows.Forms.Label lblHP;
    private System.Windows.Forms.GroupBox grpTexture;
    private System.Windows.Forms.NumericUpDown numAgility;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown numIntelligence;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown numResistance;
    private System.Windows.Forms.NumericUpDown numStrength;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.NumericUpDown numMP;
    private System.Windows.Forms.NumericUpDown numHP;
    private System.Windows.Forms.NumericUpDown numVitality;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label lblFTexture;
    private System.Windows.Forms.Button butFTexture;
    private System.Windows.Forms.Label lblMTexture;
    private System.Windows.Forms.Button butMTexture;
    private System.Windows.Forms.GroupBox grpSpawn;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.NumericUpDown numSpawn_X;
    private System.Windows.Forms.ComboBox cmbSpawn_Direction;
    private System.Windows.Forms.NumericUpDown numSpawn_Y;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label7;
    public System.Windows.Forms.TextBox txtDescription;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Button butFDelete;
    private System.Windows.Forms.Button butMDelete;
    public System.Windows.Forms.ListBox lstFemale;
    public System.Windows.Forms.ListBox lstMale;
    private System.Windows.Forms.GroupBox grpDrop;
    private System.Windows.Forms.Button butItem_Delete;
    public System.Windows.Forms.ListBox lstItems;
    private System.Windows.Forms.Button butItem_Add;
    private System.Windows.Forms.GroupBox grpItem_Add;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Button butItem_Ok;
    private System.Windows.Forms.ComboBox cmbItems;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.NumericUpDown numItem_Amount;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Button butRemove;
    private System.Windows.Forms.Button butNew;
    private System.Windows.Forms.TreeView List;
    private System.Windows.Forms.ComboBox cmbSpawn_Map;
    public System.Windows.Forms.TextBox txtFilter;
}