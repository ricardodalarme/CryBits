using DarkUI.Controls;

namespace CryBits.Editors.Forms
{
    partial class EditorShops
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorShops));
            this.butSave = new DarkUI.Controls.DarkButton();
            this.butCancel = new DarkUI.Controls.DarkButton();
            this.txtName = new DarkUI.Controls.DarkTextBox();
            this.label3 = new DarkUI.Controls.DarkLabel();
            this.lstSold = new System.Windows.Forms.ListBox();
            this.grpGeneral = new DarkUI.Controls.DarkGroupBox();
            this.cmbCurrency = new DarkUI.Controls.DarkComboBox();
            this.label5 = new DarkUI.Controls.DarkLabel();
            this.grpSold = new DarkUI.Controls.DarkGroupBox();
            this.butSold_Remove = new DarkUI.Controls.DarkButton();
            this.butSold_Add = new DarkUI.Controls.DarkButton();
            this.grpBought = new DarkUI.Controls.DarkGroupBox();
            this.butBought_Remove = new DarkUI.Controls.DarkButton();
            this.butBought_Add = new DarkUI.Controls.DarkButton();
            this.lstBought = new System.Windows.Forms.ListBox();
            this.grpAddItem = new DarkUI.Controls.DarkGroupBox();
            this.numAmount = new DarkUI.Controls.DarkNumericUpDown();
            this.numPrice = new DarkUI.Controls.DarkNumericUpDown();
            this.cmbItems = new DarkUI.Controls.DarkComboBox();
            this.butConfirm = new DarkUI.Controls.DarkButton();
            this.label4 = new DarkUI.Controls.DarkLabel();
            this.label2 = new DarkUI.Controls.DarkLabel();
            this.label1 = new DarkUI.Controls.DarkLabel();
            this.butNew = new DarkUI.Controls.DarkButton();
            this.butRemove = new DarkUI.Controls.DarkButton();
            this.List = new System.Windows.Forms.TreeView();
            this.txtFilter = new DarkUI.Controls.DarkTextBox();
            this.grpGeneral.SuspendLayout();
            this.grpSold.SuspendLayout();
            this.grpBought.SuspendLayout();
            this.grpAddItem.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(219, 412);
            this.butSave.Name = "butSave";
            this.butSave.Padding = new System.Windows.Forms.Padding(5);
            this.butSave.Size = new System.Drawing.Size(250, 25);
            this.butSave.TabIndex = 16;
            this.butSave.Text = "Save All";
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(475, 412);
            this.butCancel.Name = "butCancel";
            this.butCancel.Padding = new System.Windows.Forms.Padding(5);
            this.butCancel.Size = new System.Drawing.Size(250, 25);
            this.butCancel.TabIndex = 17;
            this.butCancel.Text = "Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtName.Location = new System.Drawing.Point(6, 36);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(242, 20);
            this.txtName.TabIndex = 20;
            this.txtName.TextChanged += new System.EventHandler(this.txtName_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label3.Location = new System.Drawing.Point(3, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Name:";
            // 
            // lstSold
            // 
            this.lstSold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstSold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSold.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstSold.FormattingEnabled = true;
            this.lstSold.Location = new System.Drawing.Point(6, 19);
            this.lstSold.Name = "lstSold";
            this.lstSold.Size = new System.Drawing.Size(238, 262);
            this.lstSold.TabIndex = 22;
            // 
            // grpGeneral
            // 
            this.grpGeneral.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpGeneral.Controls.Add(this.cmbCurrency);
            this.grpGeneral.Controls.Add(this.label5);
            this.grpGeneral.Controls.Add(this.txtName);
            this.grpGeneral.Controls.Add(this.label3);
            this.grpGeneral.Location = new System.Drawing.Point(219, 12);
            this.grpGeneral.Name = "grpGeneral";
            this.grpGeneral.Size = new System.Drawing.Size(506, 68);
            this.grpGeneral.TabIndex = 23;
            this.grpGeneral.TabStop = false;
            this.grpGeneral.Text = "General";
            // 
            // cmbCurrency
            // 
            this.cmbCurrency.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbCurrency.FormattingEnabled = true;
            this.cmbCurrency.Location = new System.Drawing.Point(258, 36);
            this.cmbCurrency.Name = "cmbCurrency";
            this.cmbCurrency.Size = new System.Drawing.Size(242, 21);
            this.cmbCurrency.TabIndex = 22;
            this.cmbCurrency.SelectedIndexChanged += new System.EventHandler(this.cmbCurrency_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label5.Location = new System.Drawing.Point(255, 20);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "Default currency:";
            // 
            // grpSold
            // 
            this.grpSold.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpSold.Controls.Add(this.butSold_Remove);
            this.grpSold.Controls.Add(this.butSold_Add);
            this.grpSold.Controls.Add(this.lstSold);
            this.grpSold.Location = new System.Drawing.Point(219, 86);
            this.grpSold.Name = "grpSold";
            this.grpSold.Size = new System.Drawing.Size(250, 321);
            this.grpSold.TabIndex = 24;
            this.grpSold.TabStop = false;
            this.grpSold.Text = "Items Sold";
            // 
            // butSold_Remove
            // 
            this.butSold_Remove.Location = new System.Drawing.Point(126, 289);
            this.butSold_Remove.Name = "butSold_Remove";
            this.butSold_Remove.Padding = new System.Windows.Forms.Padding(5);
            this.butSold_Remove.Size = new System.Drawing.Size(118, 23);
            this.butSold_Remove.TabIndex = 24;
            this.butSold_Remove.Text = "Remove";
            this.butSold_Remove.Click += new System.EventHandler(this.butSold_Remove_Click);
            // 
            // butSold_Add
            // 
            this.butSold_Add.Location = new System.Drawing.Point(6, 289);
            this.butSold_Add.Name = "butSold_Add";
            this.butSold_Add.Padding = new System.Windows.Forms.Padding(5);
            this.butSold_Add.Size = new System.Drawing.Size(118, 23);
            this.butSold_Add.TabIndex = 23;
            this.butSold_Add.Text = "Add";
            this.butSold_Add.Click += new System.EventHandler(this.butSold_Add_Click);
            // 
            // grpBought
            // 
            this.grpBought.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpBought.Controls.Add(this.butBought_Remove);
            this.grpBought.Controls.Add(this.butBought_Add);
            this.grpBought.Controls.Add(this.lstBought);
            this.grpBought.Location = new System.Drawing.Point(475, 87);
            this.grpBought.Name = "grpBought";
            this.grpBought.Size = new System.Drawing.Size(250, 320);
            this.grpBought.TabIndex = 25;
            this.grpBought.TabStop = false;
            this.grpBought.Text = "Items Bought";
            // 
            // butBought_Remove
            // 
            this.butBought_Remove.Location = new System.Drawing.Point(126, 288);
            this.butBought_Remove.Name = "butBought_Remove";
            this.butBought_Remove.Padding = new System.Windows.Forms.Padding(5);
            this.butBought_Remove.Size = new System.Drawing.Size(118, 23);
            this.butBought_Remove.TabIndex = 26;
            this.butBought_Remove.Text = "Remove";
            this.butBought_Remove.Click += new System.EventHandler(this.butBought_Remove_Click);
            // 
            // butBought_Add
            // 
            this.butBought_Add.Location = new System.Drawing.Point(6, 288);
            this.butBought_Add.Name = "butBought_Add";
            this.butBought_Add.Padding = new System.Windows.Forms.Padding(5);
            this.butBought_Add.Size = new System.Drawing.Size(118, 23);
            this.butBought_Add.TabIndex = 25;
            this.butBought_Add.Text = "Add";
            this.butBought_Add.Click += new System.EventHandler(this.butBought_Add_Click);
            // 
            // lstBought
            // 
            this.lstBought.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.lstBought.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstBought.ForeColor = System.Drawing.Color.Gainsboro;
            this.lstBought.FormattingEnabled = true;
            this.lstBought.Location = new System.Drawing.Point(6, 19);
            this.lstBought.Name = "lstBought";
            this.lstBought.Size = new System.Drawing.Size(238, 262);
            this.lstBought.TabIndex = 22;
            // 
            // grpAddItem
            // 
            this.grpAddItem.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpAddItem.Controls.Add(this.numAmount);
            this.grpAddItem.Controls.Add(this.numPrice);
            this.grpAddItem.Controls.Add(this.cmbItems);
            this.grpAddItem.Controls.Add(this.butConfirm);
            this.grpAddItem.Controls.Add(this.label4);
            this.grpAddItem.Controls.Add(this.label2);
            this.grpAddItem.Controls.Add(this.label1);
            this.grpAddItem.Location = new System.Drawing.Point(219, 86);
            this.grpAddItem.Name = "grpAddItem";
            this.grpAddItem.Size = new System.Drawing.Size(506, 320);
            this.grpAddItem.TabIndex = 26;
            this.grpAddItem.TabStop = false;
            this.grpAddItem.Text = "Add Item";
            this.grpAddItem.Visible = false;
            // 
            // numAmount
            // 
            this.numAmount.Location = new System.Drawing.Point(170, 133);
            this.numAmount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numAmount.Name = "numAmount";
            this.numAmount.Size = new System.Drawing.Size(166, 20);
            this.numAmount.TabIndex = 22;
            this.numAmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numPrice
            // 
            this.numPrice.Location = new System.Drawing.Point(170, 172);
            this.numPrice.Name = "numPrice";
            this.numPrice.Size = new System.Drawing.Size(166, 20);
            this.numPrice.TabIndex = 21;
            // 
            // cmbItems
            // 
            this.cmbItems.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbItems.FormattingEnabled = true;
            this.cmbItems.Location = new System.Drawing.Point(170, 93);
            this.cmbItems.Name = "cmbItems";
            this.cmbItems.Size = new System.Drawing.Size(166, 21);
            this.cmbItems.TabIndex = 20;
            // 
            // butConfirm
            // 
            this.butConfirm.Location = new System.Drawing.Point(170, 198);
            this.butConfirm.Name = "butConfirm";
            this.butConfirm.Padding = new System.Windows.Forms.Padding(5);
            this.butConfirm.Size = new System.Drawing.Size(166, 20);
            this.butConfirm.TabIndex = 19;
            this.butConfirm.Text = "Confirm";
            this.butConfirm.Click += new System.EventHandler(this.butConfirm_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label4.Location = new System.Drawing.Point(167, 156);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(34, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Price:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label2.Location = new System.Drawing.Point(167, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Amount:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label1.Location = new System.Drawing.Point(167, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Item:";
            // 
            // butNew
            // 
            this.butNew.Location = new System.Drawing.Point(12, 412);
            this.butNew.Name = "butNew";
            this.butNew.Padding = new System.Windows.Forms.Padding(5);
            this.butNew.Size = new System.Drawing.Size(98, 25);
            this.butNew.TabIndex = 27;
            this.butNew.Text = "New";
            this.butNew.Click += new System.EventHandler(this.butNew_Click);
            // 
            // butRemove
            // 
            this.butRemove.Location = new System.Drawing.Point(116, 412);
            this.butRemove.Name = "butRemove";
            this.butRemove.Padding = new System.Windows.Forms.Padding(5);
            this.butRemove.Size = new System.Drawing.Size(98, 25);
            this.butRemove.TabIndex = 28;
            this.butRemove.Text = "Remove";
            this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
            // 
            // List
            // 
            this.List.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(65)))));
            this.List.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.List.ForeColor = System.Drawing.Color.Gainsboro;
            this.List.HideSelection = false;
            this.List.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(150)))), ((int)(((byte)(150)))));
            this.List.Location = new System.Drawing.Point(12, 38);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(201, 369);
            this.List.TabIndex = 23;
            this.List.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.List_AfterSelect);
            // 
            // txtFilter
            // 
            this.txtFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(73)))), ((int)(((byte)(74)))));
            this.txtFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFilter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtFilter.Location = new System.Drawing.Point(12, 12);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(201, 20);
            this.txtFilter.TabIndex = 43;
            this.txtFilter.TextChanged += new System.EventHandler(this.txtFilter_TextChanged);
            // 
            // Editor_Shops
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 447);
            this.ControlBox = false;
            this.Controls.Add(this.grpAddItem);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.List);
            this.Controls.Add(this.butRemove);
            this.Controls.Add(this.butNew);
            this.Controls.Add(this.grpBought);
            this.Controls.Add(this.grpSold);
            this.Controls.Add(this.grpGeneral);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "EditorShops";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shop Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Editor_Shops_FormClosed);
            this.grpGeneral.ResumeLayout(false);
            this.grpGeneral.PerformLayout();
            this.grpSold.ResumeLayout(false);
            this.grpBought.ResumeLayout(false);
            this.grpAddItem.ResumeLayout(false);
            this.grpAddItem.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DarkButton butSave;
        private DarkButton butCancel;
        public DarkTextBox txtName;
        private DarkLabel label3;
        private System.Windows.Forms.ListBox lstSold;
        private DarkGroupBox grpGeneral;
        private DarkGroupBox grpSold;
        private DarkGroupBox grpBought;
        private System.Windows.Forms.ListBox lstBought;
        private DarkButton butSold_Remove;
        private DarkButton butSold_Add;
        private DarkButton butBought_Remove;
        private DarkButton butBought_Add;
        private DarkGroupBox grpAddItem;
        private DarkNumericUpDown numAmount;
        private DarkNumericUpDown numPrice;
        private DarkComboBox cmbItems;
        private DarkButton butConfirm;
        private DarkLabel label4;
        private DarkLabel label2;
        private DarkLabel label1;
        private DarkLabel label5;
        private DarkComboBox cmbCurrency;
        private DarkButton butNew;
        private DarkButton butRemove;
        private System.Windows.Forms.TreeView List;
        public DarkTextBox txtFilter;
    }
}