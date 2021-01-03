using DarkUI.Controls;

namespace CryBits.Editors.Forms
{
    partial class EditorItems
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorItems));
            this.grpGeneral = new DarkUI.Controls.DarkGroupBox();
            this.picTexture = new System.Windows.Forms.PictureBox();
            this.cmbRarity = new DarkUI.Controls.DarkComboBox();
            this.cmbBind = new DarkUI.Controls.DarkComboBox();
            this.label20 = new DarkUI.Controls.DarkLabel();
            this.label19 = new DarkUI.Controls.DarkLabel();
            this.txtDescription = new DarkUI.Controls.DarkTextBox();
            this.label15 = new DarkUI.Controls.DarkLabel();
            this.chkStackable = new DarkUI.Controls.DarkCheckBox();
            this.cmbType = new DarkUI.Controls.DarkComboBox();
            this.numTexture = new DarkUI.Controls.DarkNumericUpDown();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.label2 = new DarkUI.Controls.DarkLabel();
            this.label1 = new DarkUI.Controls.DarkLabel();
            this.label3 = new DarkUI.Controls.DarkLabel();
            this.butSave = new DarkUI.Controls.DarkButton();
            this.butCancel = new DarkUI.Controls.DarkButton();
            this.grpRequirements = new DarkUI.Controls.DarkGroupBox();
            this.cmbReq_Class = new DarkUI.Controls.DarkComboBox();
            this.numReq_Level = new DarkUI.Controls.DarkNumericUpDown();
            this.label5 = new DarkUI.Controls.DarkLabel();
            this.label4 = new DarkUI.Controls.DarkLabel();
            this.label12 = new DarkUI.Controls.DarkLabel();
            this.numPotion_Experience = new DarkUI.Controls.DarkNumericUpDown();
            this.label9 = new DarkUI.Controls.DarkLabel();
            this.numPotion_MP = new DarkUI.Controls.DarkNumericUpDown();
            this.numPotion_HP = new DarkUI.Controls.DarkNumericUpDown();
            this.lblMP = new DarkUI.Controls.DarkLabel();
            this.grpPotion = new DarkUI.Controls.DarkGroupBox();
            this.label14 = new DarkUI.Controls.DarkLabel();
            this.grpEquipment = new DarkUI.Controls.DarkGroupBox();
            this.numWeapon_Damage = new DarkUI.Controls.DarkNumericUpDown();
            this.numEquip_Strength = new DarkUI.Controls.DarkNumericUpDown();
            this.numEquip_Vitality = new DarkUI.Controls.DarkNumericUpDown();
            this.numEquip_Resistance = new DarkUI.Controls.DarkNumericUpDown();
            this.numEquip_Intelligence = new DarkUI.Controls.DarkNumericUpDown();
            this.numEquip_Agility = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbEquipment_Type = new DarkUI.Controls.DarkComboBox();
            this.label10 = new DarkUI.Controls.DarkLabel();
            this.lblWeapon_Damage = new DarkUI.Controls.DarkLabel();
            this.label8 = new DarkUI.Controls.DarkLabel();
            this.label7 = new DarkUI.Controls.DarkLabel();
            this.label6 = new DarkUI.Controls.DarkLabel();
            this.label13 = new DarkUI.Controls.DarkLabel();
            this.label11 = new DarkUI.Controls.DarkLabel();
            this.List = new System.Windows.Forms.TreeView();
            this.butRemove = new DarkUI.Controls.DarkButton();
            this.butNew = new DarkUI.Controls.DarkButton();
            this.txtFilter = new DarkUI.Controls.DarkTextBox();
            this.grpGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTexture)).BeginInit();
            this.grpRequirements.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReq_Level)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPotion_Experience)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPotion_MP)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPotion_HP)).BeginInit();
            this.grpPotion.SuspendLayout();
            this.grpEquipment.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWeapon_Damage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Strength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Vitality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Resistance)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Intelligence)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Agility)).BeginInit();
            this.SuspendLayout();
            // 
            // grpGeneral
            // 
            this.grpGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpGeneral.Controls.Add(this.picTexture);
            this.grpGeneral.Controls.Add(this.cmbRarity);
            this.grpGeneral.Controls.Add(this.cmbBind);
            this.grpGeneral.Controls.Add(this.label20);
            this.grpGeneral.Controls.Add(this.label19);
            this.grpGeneral.Controls.Add(this.txtDescription);
            this.grpGeneral.Controls.Add(this.label15);
            this.grpGeneral.Controls.Add(this.chkStackable);
            this.grpGeneral.Controls.Add(this.cmbType);
            this.grpGeneral.Controls.Add(this.numTexture);
            this.grpGeneral.Controls.Add(this.txtName);
            this.grpGeneral.Controls.Add(this.label2);
            this.grpGeneral.Controls.Add(this.label1);
            this.grpGeneral.Controls.Add(this.label3);
            this.grpGeneral.Location = new System.Drawing.Point(255, 8);
            this.grpGeneral.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpGeneral.Size = new System.Drawing.Size(355, 290);
            this.grpGeneral.TabIndex = 14;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General";
            // 
            // picTexture
            // 
            this.picTexture.Location = new System.Drawing.Point(307, 173);
            this.picTexture.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.picTexture.Name = "picTexture";
            this.picTexture.Size = new System.Drawing.Size(37, 37);
            this.picTexture.TabIndex = 30;
            this.picTexture.TabStop = false;
            // 
            // cmbRarity
            // 
            this.cmbRarity.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbRarity.FormattingEnabled = true;
            this.cmbRarity.Location = new System.Drawing.Point(12, 233);
            this.cmbRarity.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbRarity.Name = "cmbRarity";
            this.cmbRarity.Size = new System.Drawing.Size(162, 24);
            this.cmbRarity.TabIndex = 27;
            this.cmbRarity.SelectedIndexChanged += new System.EventHandler(this.cmbRarity_SelectedIndexChanged);
            // 
            // cmbBind
            // 
            this.cmbBind.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbBind.FormattingEnabled = true;
            this.cmbBind.Location = new System.Drawing.Point(182, 233);
            this.cmbBind.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbBind.Name = "cmbBind";
            this.cmbBind.Size = new System.Drawing.Size(162, 24);
            this.cmbBind.TabIndex = 26;
            this.cmbBind.SelectedIndexChanged += new System.EventHandler(this.cmbBind_SelectedIndexChanged);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label20.Location = new System.Drawing.Point(8, 215);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(40, 15);
            this.label20.TabIndex = 29;
            this.label20.Text = "Rarity:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label19.Location = new System.Drawing.Point(178, 216);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(34, 15);
            this.label19.TabIndex = 28;
            this.label19.Text = "Bind:";
            // 
            // txtDescription
            // 
            this.txtDescription.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDescription.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtDescription.Location = new System.Drawing.Point(12, 83);
            this.txtDescription.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(332, 79);
            this.txtDescription.TabIndex = 23;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label15.Location = new System.Drawing.Point(8, 66);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(70, 15);
            this.label15.TabIndex = 22;
            this.label15.Text = "Description:";
            // 
            // chkStackable
            // 
            this.chkStackable.AutoSize = true;
            this.chkStackable.Location = new System.Drawing.Point(12, 264);
            this.chkStackable.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.chkStackable.Name = "chkStackable";
            this.chkStackable.Size = new System.Drawing.Size(76, 19);
            this.chkStackable.TabIndex = 20;
            this.chkStackable.Text = "Stackable";
            this.chkStackable.CheckedChanged += new System.EventHandler(this.chkStackable_CheckedChanged);
            // 
            // cmbType
            // 
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "None",
            "Equipment",
            "Potion"});
            this.cmbType.Location = new System.Drawing.Point(12, 185);
            this.cmbType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(162, 24);
            this.cmbType.TabIndex = 18;
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            // 
            // numTexture
            // 
            this.numTexture.Location = new System.Drawing.Point(181, 187);
            this.numTexture.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numTexture.Name = "numTexture";
            this.numTexture.Size = new System.Drawing.Size(119, 23);
            this.numTexture.TabIndex = 12;
            this.numTexture.ValueChanged += new System.EventHandler(this.numTexture_ValueChanged);
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtName.Location = new System.Drawing.Point(12, 36);
            this.txtName.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(332, 23);
            this.txtName.TabIndex = 10;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label2.Location = new System.Drawing.Point(8, 166);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 15);
            this.label2.TabIndex = 19;
            this.label2.Text = "Type:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label1.Location = new System.Drawing.Point(177, 168);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 15);
            this.label1.TabIndex = 11;
            this.label1.Text = "Texture:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label3.Location = new System.Drawing.Point(8, 15);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "Name:";
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(253, 553);
            this.butSave.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butSave.Name = "butSave";
            this.butSave.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.butSave.Size = new System.Drawing.Size(176, 29);
            this.butSave.TabIndex = 16;
            this.butSave.Text = "Save All";
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(434, 553);
            this.butCancel.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butCancel.Name = "butCancel";
            this.butCancel.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.butCancel.Size = new System.Drawing.Size(176, 29);
            this.butCancel.TabIndex = 17;
            this.butCancel.Text = "Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // grpRequirements
            // 
            this.grpRequirements.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpRequirements.Controls.Add(this.cmbReq_Class);
            this.grpRequirements.Controls.Add(this.numReq_Level);
            this.grpRequirements.Controls.Add(this.label5);
            this.grpRequirements.Controls.Add(this.label4);
            this.grpRequirements.Location = new System.Drawing.Point(255, 305);
            this.grpRequirements.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpRequirements.Name = "grpRequirements";
            this.grpRequirements.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpRequirements.Size = new System.Drawing.Size(355, 73);
            this.grpRequirements.TabIndex = 19;
            this.grpRequirements.TabStop = false;
            this.grpRequirements.Text = "Requirements";
            // 
            // cmbReq_Class
            // 
            this.cmbReq_Class.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbReq_Class.FormattingEnabled = true;
            this.cmbReq_Class.Location = new System.Drawing.Point(183, 42);
            this.cmbReq_Class.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbReq_Class.Name = "cmbReq_Class";
            this.cmbReq_Class.Size = new System.Drawing.Size(162, 24);
            this.cmbReq_Class.TabIndex = 20;
            this.cmbReq_Class.SelectedIndexChanged += new System.EventHandler(this.cmbReq_Class_SelectedIndexChanged);
            // 
            // numReq_Level
            // 
            this.numReq_Level.Location = new System.Drawing.Point(13, 42);
            this.numReq_Level.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numReq_Level.Name = "numReq_Level";
            this.numReq_Level.Size = new System.Drawing.Size(162, 23);
            this.numReq_Level.TabIndex = 14;
            this.numReq_Level.ValueChanged += new System.EventHandler(this.numReq_Level_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label5.Location = new System.Drawing.Point(181, 24);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 15);
            this.label5.TabIndex = 21;
            this.label5.Text = "Class:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label4.Location = new System.Drawing.Point(9, 23);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Level:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label12.Location = new System.Drawing.Point(12, 24);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(26, 15);
            this.label12.TabIndex = 53;
            this.label12.Text = "HP:";
            // 
            // numPotion_Experience
            // 
            this.numPotion_Experience.Location = new System.Drawing.Point(241, 43);
            this.numPotion_Experience.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numPotion_Experience.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numPotion_Experience.Name = "numPotion_Experience";
            this.numPotion_Experience.Size = new System.Drawing.Size(105, 23);
            this.numPotion_Experience.TabIndex = 50;
            this.numPotion_Experience.ValueChanged += new System.EventHandler(this.numEquip_Experience_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label9.Location = new System.Drawing.Point(238, 24);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 15);
            this.label9.TabIndex = 49;
            this.label9.Text = "Experience:";
            // 
            // numPotion_MP
            // 
            this.numPotion_MP.Location = new System.Drawing.Point(127, 43);
            this.numPotion_MP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numPotion_MP.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numPotion_MP.Name = "numPotion_MP";
            this.numPotion_MP.Size = new System.Drawing.Size(105, 23);
            this.numPotion_MP.TabIndex = 40;
            this.numPotion_MP.ValueChanged += new System.EventHandler(this.numEquip_MP_ValueChanged);
            // 
            // numPotion_HP
            // 
            this.numPotion_HP.Location = new System.Drawing.Point(14, 43);
            this.numPotion_HP.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numPotion_HP.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numPotion_HP.Name = "numPotion_HP";
            this.numPotion_HP.Size = new System.Drawing.Size(105, 23);
            this.numPotion_HP.TabIndex = 39;
            this.numPotion_HP.ValueChanged += new System.EventHandler(this.numEquip_HP_ValueChanged);
            // 
            // lblMP
            // 
            this.lblMP.AutoSize = true;
            this.lblMP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblMP.Location = new System.Drawing.Point(124, 24);
            this.lblMP.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblMP.Name = "lblMP";
            this.lblMP.Size = new System.Drawing.Size(28, 15);
            this.lblMP.TabIndex = 38;
            this.lblMP.Text = "MP:";
            // 
            // grpPotion
            // 
            this.grpPotion.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpPotion.Controls.Add(this.numPotion_MP);
            this.grpPotion.Controls.Add(this.numPotion_Experience);
            this.grpPotion.Controls.Add(this.numPotion_HP);
            this.grpPotion.Controls.Add(this.label14);
            this.grpPotion.Controls.Add(this.lblMP);
            this.grpPotion.Controls.Add(this.label12);
            this.grpPotion.Controls.Add(this.label9);
            this.grpPotion.Location = new System.Drawing.Point(255, 384);
            this.grpPotion.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpPotion.Name = "grpPotion";
            this.grpPotion.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpPotion.Size = new System.Drawing.Size(355, 89);
            this.grpPotion.TabIndex = 21;
            this.grpPotion.TabStop = false;
            this.grpPotion.Text = "Potion";
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label14.Location = new System.Drawing.Point(12, 69);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(335, 16);
            this.label14.TabIndex = 55;
            this.label14.Text = "(Negative values are also valid)";
            this.label14.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // grpEquipment
            // 
            this.grpEquipment.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpEquipment.Controls.Add(this.numWeapon_Damage);
            this.grpEquipment.Controls.Add(this.numEquip_Strength);
            this.grpEquipment.Controls.Add(this.numEquip_Vitality);
            this.grpEquipment.Controls.Add(this.numEquip_Resistance);
            this.grpEquipment.Controls.Add(this.numEquip_Intelligence);
            this.grpEquipment.Controls.Add(this.numEquip_Agility);
            this.grpEquipment.Controls.Add(this.cmbEquipment_Type);
            this.grpEquipment.Controls.Add(this.label10);
            this.grpEquipment.Controls.Add(this.lblWeapon_Damage);
            this.grpEquipment.Controls.Add(this.label8);
            this.grpEquipment.Controls.Add(this.label7);
            this.grpEquipment.Controls.Add(this.label6);
            this.grpEquipment.Controls.Add(this.label13);
            this.grpEquipment.Controls.Add(this.label11);
            this.grpEquipment.Location = new System.Drawing.Point(255, 384);
            this.grpEquipment.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpEquipment.Name = "grpEquipment";
            this.grpEquipment.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.grpEquipment.Size = new System.Drawing.Size(355, 162);
            this.grpEquipment.TabIndex = 22;
            this.grpEquipment.TabStop = false;
            this.grpEquipment.Text = "Equipment";
            // 
            // numWeapon_Damage
            // 
            this.numWeapon_Damage.Location = new System.Drawing.Point(241, 113);
            this.numWeapon_Damage.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numWeapon_Damage.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numWeapon_Damage.Name = "numWeapon_Damage";
            this.numWeapon_Damage.Size = new System.Drawing.Size(105, 23);
            this.numWeapon_Damage.TabIndex = 58;
            this.numWeapon_Damage.ValueChanged += new System.EventHandler(this.numWeapon_Damage_ValueChanged);
            // 
            // numEquip_Strength
            // 
            this.numEquip_Strength.Location = new System.Drawing.Point(14, 66);
            this.numEquip_Strength.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numEquip_Strength.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numEquip_Strength.Name = "numEquip_Strength";
            this.numEquip_Strength.Size = new System.Drawing.Size(105, 23);
            this.numEquip_Strength.TabIndex = 43;
            this.numEquip_Strength.ValueChanged += new System.EventHandler(this.numEquip_Strength_ValueChanged);
            // 
            // numEquip_Vitality
            // 
            this.numEquip_Vitality.Location = new System.Drawing.Point(127, 113);
            this.numEquip_Vitality.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numEquip_Vitality.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numEquip_Vitality.Name = "numEquip_Vitality";
            this.numEquip_Vitality.Size = new System.Drawing.Size(105, 23);
            this.numEquip_Vitality.TabIndex = 52;
            this.numEquip_Vitality.ValueChanged += new System.EventHandler(this.numEquip_Vitality_ValueChanged);
            // 
            // numEquip_Resistance
            // 
            this.numEquip_Resistance.Location = new System.Drawing.Point(127, 66);
            this.numEquip_Resistance.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numEquip_Resistance.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numEquip_Resistance.Name = "numEquip_Resistance";
            this.numEquip_Resistance.Size = new System.Drawing.Size(105, 23);
            this.numEquip_Resistance.TabIndex = 44;
            this.numEquip_Resistance.ValueChanged += new System.EventHandler(this.numEquip_Resistance_ValueChanged);
            // 
            // numEquip_Intelligence
            // 
            this.numEquip_Intelligence.Location = new System.Drawing.Point(241, 66);
            this.numEquip_Intelligence.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numEquip_Intelligence.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numEquip_Intelligence.Name = "numEquip_Intelligence";
            this.numEquip_Intelligence.Size = new System.Drawing.Size(105, 23);
            this.numEquip_Intelligence.TabIndex = 46;
            this.numEquip_Intelligence.ValueChanged += new System.EventHandler(this.numEquip_Intelligence_ValueChanged);
            // 
            // numEquip_Agility
            // 
            this.numEquip_Agility.Location = new System.Drawing.Point(14, 113);
            this.numEquip_Agility.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.numEquip_Agility.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numEquip_Agility.Name = "numEquip_Agility";
            this.numEquip_Agility.Size = new System.Drawing.Size(105, 23);
            this.numEquip_Agility.TabIndex = 48;
            this.numEquip_Agility.ValueChanged += new System.EventHandler(this.numEquip_Agility_ValueChanged);
            // 
            // cmbEquipment_Type
            // 
            this.cmbEquipment_Type.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbEquipment_Type.FormattingEnabled = true;
            this.cmbEquipment_Type.Items.AddRange(new object[] {
            "Weapon",
            "Armor",
            "Helmet",
            "Shield",
            "Amulet"});
            this.cmbEquipment_Type.Location = new System.Drawing.Point(14, 21);
            this.cmbEquipment_Type.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbEquipment_Type.Name = "cmbEquipment_Type";
            this.cmbEquipment_Type.Size = new System.Drawing.Size(332, 24);
            this.cmbEquipment_Type.TabIndex = 22;
            this.cmbEquipment_Type.SelectedIndexChanged += new System.EventHandler(this.cmbEquipment_Type_SelectedIndexChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label10.Location = new System.Drawing.Point(124, 95);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 15);
            this.label10.TabIndex = 51;
            this.label10.Text = "Vitality:";
            // 
            // lblWeapon_Damage
            // 
            this.lblWeapon_Damage.AutoSize = true;
            this.lblWeapon_Damage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.lblWeapon_Damage.Location = new System.Drawing.Point(238, 95);
            this.lblWeapon_Damage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblWeapon_Damage.Name = "lblWeapon_Damage";
            this.lblWeapon_Damage.Size = new System.Drawing.Size(80, 15);
            this.lblWeapon_Damage.TabIndex = 57;
            this.lblWeapon_Damage.Text = "Base damage:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label8.Location = new System.Drawing.Point(10, 92);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 15);
            this.label8.TabIndex = 47;
            this.label8.Text = "Agility:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label7.Location = new System.Drawing.Point(10, 47);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 15);
            this.label7.TabIndex = 41;
            this.label7.Text = "Strength:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label6.Location = new System.Drawing.Point(124, 47);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 15);
            this.label6.TabIndex = 42;
            this.label6.Text = "Resistance:";
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label13.Location = new System.Drawing.Point(12, 140);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(335, 18);
            this.label13.TabIndex = 56;
            this.label13.Text = "(Negative values are also valid)";
            this.label13.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label11.Location = new System.Drawing.Point(239, 47);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(71, 15);
            this.label11.TabIndex = 45;
            this.label11.Text = "Intelligence:";
            // 
            // List
            // 
            this.List.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.List.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.List.ForeColor = System.Drawing.Color.Gainsboro;
            this.List.HideSelection = false;
            this.List.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.List.Location = new System.Drawing.Point(13, 44);
            this.List.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(234, 502);
            this.List.TabIndex = 44;
            this.List.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.List_AfterSelect);
            // 
            // butRemove
            // 
            this.butRemove.Location = new System.Drawing.Point(133, 553);
            this.butRemove.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butRemove.Name = "butRemove";
            this.butRemove.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.butRemove.Size = new System.Drawing.Size(114, 29);
            this.butRemove.TabIndex = 43;
            this.butRemove.Text = "Remove";
            this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
            // 
            // butNew
            // 
            this.butNew.Location = new System.Drawing.Point(12, 553);
            this.butNew.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butNew.Name = "butNew";
            this.butNew.Padding = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.butNew.Size = new System.Drawing.Size(114, 29);
            this.butNew.TabIndex = 42;
            this.butNew.Text = "New";
            this.butNew.Click += new System.EventHandler(this.butNew_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            this.txtFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtFilter.Location = new System.Drawing.Point(13, 14);
            this.txtFilter.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(234, 20);
            this.txtFilter.TabIndex = 45;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // EditorItems
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 597);
            this.ControlBox = false;
            this.Controls.Add(this.grpEquipment);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.List);
            this.Controls.Add(this.butRemove);
            this.Controls.Add(this.butNew);
            this.Controls.Add(this.grpRequirements);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.grpGeneral);
            this.Controls.Add(this.grpPotion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "EditorItems";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Item Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Editor_Items_FormClosed);
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTexture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTexture)).EndInit();
            this.grpRequirements.ResumeLayout(false);
            this.grpRequirements.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numReq_Level)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPotion_Experience)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPotion_MP)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPotion_HP)).EndInit();
            this.grpPotion.ResumeLayout(false);
            this.grpPotion.PerformLayout();
            this.grpEquipment.ResumeLayout(false);
            this.grpEquipment.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWeapon_Damage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Strength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Vitality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Resistance)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Intelligence)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numEquip_Agility)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DarkGroupBox grpGeneral;
        public DarkTextBox txtName;
        private DarkLabel label3;
        private DarkButton butSave;
        private DarkButton butCancel;
        private DarkLabel label1;
        private DarkLabel label2;
        private DarkComboBox cmbType;
        private DarkGroupBox grpRequirements;
        private DarkLabel label5;
        private DarkComboBox cmbReq_Class;
        private DarkNumericUpDown numReq_Level;
        private DarkLabel label4;
        private DarkLabel label12;
        private DarkNumericUpDown numPotion_Experience;
        private DarkLabel label9;
        private DarkNumericUpDown numPotion_MP;
        private DarkNumericUpDown numPotion_HP;
        private DarkLabel lblMP;
        private DarkGroupBox grpPotion;
        private DarkLabel label14;
        public DarkTextBox txtDescription;
        private DarkLabel label15;
        private DarkCheckBox chkStackable;
        private DarkGroupBox grpEquipment;
        private DarkComboBox cmbEquipment_Type;
        private DarkLabel label20;
        private DarkLabel label19;
        private DarkComboBox cmbRarity;
        private DarkComboBox cmbBind;
        private System.Windows.Forms.TreeView List;
        private DarkButton butRemove;
        private DarkButton butNew;
        public DarkTextBox txtFilter;
        private DarkNumericUpDown numWeapon_Damage;
        private DarkNumericUpDown numEquip_Strength;
        private DarkLabel label10;
        public DarkLabel lblWeapon_Damage;
        private DarkLabel label8;
        private DarkNumericUpDown numEquip_Vitality;
        private DarkLabel label7;
        private DarkLabel label6;
        private DarkNumericUpDown numEquip_Resistance;
        private DarkLabel label13;
        private DarkLabel label11;
        private DarkNumericUpDown numEquip_Intelligence;
        private DarkNumericUpDown numEquip_Agility;
        public DarkNumericUpDown numTexture;
        private System.Windows.Forms.PictureBox picTexture;
    }
}