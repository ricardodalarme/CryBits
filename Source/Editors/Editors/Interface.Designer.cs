partial class Editor_Interface
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor_Interface));
            this.picWindow = new System.Windows.Forms.PictureBox();
            this.butNew = new System.Windows.Forms.Button();
            this.butRemove = new System.Windows.Forms.Button();
            this.cmbWindows = new System.Windows.Forms.ComboBox();
            this.grpOrder = new System.Windows.Forms.GroupBox();
            this.butOrder_Down = new System.Windows.Forms.Button();
            this.butOrder_Up = new System.Windows.Forms.Button();
            this.butOrder_Unpin = new System.Windows.Forms.Button();
            this.butOrder_Pin = new System.Windows.Forms.Button();
            this.treOrder = new System.Windows.Forms.TreeView();
            this.grpProperties = new System.Windows.Forms.GroupBox();
            this.prgProperties = new System.Windows.Forms.PropertyGrid();
            this.butCancel = new System.Windows.Forms.Button();
            this.butSaveAll = new System.Windows.Forms.Button();
            this.grpNew = new System.Windows.Forms.GroupBox();
            this.butConfirm = new System.Windows.Forms.Button();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picWindow)).BeginInit();
            this.grpOrder.SuspendLayout();
            this.grpProperties.SuspendLayout();
            this.grpNew.SuspendLayout();
            this.SuspendLayout();
            // 
            // picWindow
            // 
            this.picWindow.BackColor = System.Drawing.Color.Black;
            this.picWindow.Location = new System.Drawing.Point(12, 40);
            this.picWindow.Name = "picWindow";
            this.picWindow.Size = new System.Drawing.Size(800, 608);
            this.picWindow.TabIndex = 1;
            this.picWindow.TabStop = false;
            // 
            // butNew
            // 
            this.butNew.Location = new System.Drawing.Point(11, 19);
            this.butNew.Name = "butNew";
            this.butNew.Size = new System.Drawing.Size(109, 25);
            this.butNew.TabIndex = 28;
            this.butNew.Text = "New";
            this.butNew.UseVisualStyleBackColor = true;
            this.butNew.Click += new System.EventHandler(this.butNew_Click);
            // 
            // butRemove
            // 
            this.butRemove.Location = new System.Drawing.Point(124, 19);
            this.butRemove.Name = "butRemove";
            this.butRemove.Size = new System.Drawing.Size(109, 25);
            this.butRemove.TabIndex = 29;
            this.butRemove.Text = "Remove";
            this.butRemove.UseVisualStyleBackColor = true;
            this.butRemove.Click += new System.EventHandler(this.butRemove_Click);
            // 
            // cmbWindows
            // 
            this.cmbWindows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWindows.Location = new System.Drawing.Point(12, 12);
            this.cmbWindows.Name = "cmbWindows";
            this.cmbWindows.Size = new System.Drawing.Size(800, 21);
            this.cmbWindows.TabIndex = 30;
            this.cmbWindows.SelectedIndexChanged += new System.EventHandler(this.cmbWindows_SelectedIndexChanged);
            // 
            // grpOrder
            // 
            this.grpOrder.Controls.Add(this.butOrder_Down);
            this.grpOrder.Controls.Add(this.butOrder_Up);
            this.grpOrder.Controls.Add(this.butRemove);
            this.grpOrder.Controls.Add(this.butOrder_Unpin);
            this.grpOrder.Controls.Add(this.butNew);
            this.grpOrder.Controls.Add(this.butOrder_Pin);
            this.grpOrder.Controls.Add(this.treOrder);
            this.grpOrder.Location = new System.Drawing.Point(823, 12);
            this.grpOrder.Name = "grpOrder";
            this.grpOrder.Size = new System.Drawing.Size(243, 382);
            this.grpOrder.TabIndex = 33;
            this.grpOrder.TabStop = false;
            this.grpOrder.Text = "Order";
            // 
            // butOrder_Down
            // 
            this.butOrder_Down.Location = new System.Drawing.Point(183, 350);
            this.butOrder_Down.Name = "butOrder_Down";
            this.butOrder_Down.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Down.TabIndex = 30;
            this.butOrder_Down.Text = "Down";
            this.butOrder_Down.UseVisualStyleBackColor = true;
            this.butOrder_Down.Click += new System.EventHandler(this.butOrder_Down_Click);
            // 
            // butOrder_Up
            // 
            this.butOrder_Up.Location = new System.Drawing.Point(125, 350);
            this.butOrder_Up.Name = "butOrder_Up";
            this.butOrder_Up.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Up.TabIndex = 29;
            this.butOrder_Up.Text = "Up";
            this.butOrder_Up.UseVisualStyleBackColor = true;
            this.butOrder_Up.Click += new System.EventHandler(this.butOrder_Up_Click);
            // 
            // butOrder_Unpin
            // 
            this.butOrder_Unpin.Location = new System.Drawing.Point(68, 350);
            this.butOrder_Unpin.Name = "butOrder_Unpin";
            this.butOrder_Unpin.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Unpin.TabIndex = 28;
            this.butOrder_Unpin.Text = "Unpin";
            this.butOrder_Unpin.UseVisualStyleBackColor = true;
            this.butOrder_Unpin.Click += new System.EventHandler(this.butOrder_Unpin_Click);
            // 
            // butOrder_Pin
            // 
            this.butOrder_Pin.Location = new System.Drawing.Point(12, 350);
            this.butOrder_Pin.Name = "butOrder_Pin";
            this.butOrder_Pin.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Pin.TabIndex = 27;
            this.butOrder_Pin.Text = "Pin";
            this.butOrder_Pin.UseVisualStyleBackColor = true;
            this.butOrder_Pin.Click += new System.EventHandler(this.butOrder_Pin_Click);
            // 
            // treOrder
            // 
            this.treOrder.Location = new System.Drawing.Point(12, 50);
            this.treOrder.Name = "treOrder";
            this.treOrder.Size = new System.Drawing.Size(221, 294);
            this.treOrder.TabIndex = 0;
            this.treOrder.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treOrder_AfterSelect);
            // 
            // grpProperties
            // 
            this.grpProperties.Controls.Add(this.prgProperties);
            this.grpProperties.Location = new System.Drawing.Point(823, 400);
            this.grpProperties.Name = "grpProperties";
            this.grpProperties.Size = new System.Drawing.Size(243, 215);
            this.grpProperties.TabIndex = 34;
            this.grpProperties.TabStop = false;
            this.grpProperties.Text = "Properties";
            // 
            // prgProperties
            // 
            this.prgProperties.HelpVisible = false;
            this.prgProperties.Location = new System.Drawing.Point(11, 19);
            this.prgProperties.Name = "prgProperties";
            this.prgProperties.Size = new System.Drawing.Size(222, 182);
            this.prgProperties.TabIndex = 25;
            this.prgProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.prgProperties_PropertyValueChanged);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(948, 621);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(118, 25);
            this.butCancel.TabIndex = 39;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSaveAll
            // 
            this.butSaveAll.Location = new System.Drawing.Point(822, 621);
            this.butSaveAll.Name = "butSaveAll";
            this.butSaveAll.Size = new System.Drawing.Size(118, 25);
            this.butSaveAll.TabIndex = 38;
            this.butSaveAll.Text = "Save All";
            this.butSaveAll.UseVisualStyleBackColor = true;
            this.butSaveAll.Click += new System.EventHandler(this.butSaveAll_Click);
            // 
            // grpNew
            // 
            this.grpNew.Controls.Add(this.butConfirm);
            this.grpNew.Controls.Add(this.cmbType);
            this.grpNew.Controls.Add(this.label1);
            this.grpNew.Location = new System.Drawing.Point(822, 12);
            this.grpNew.Name = "grpNew";
            this.grpNew.Size = new System.Drawing.Size(244, 603);
            this.grpNew.TabIndex = 31;
            this.grpNew.TabStop = false;
            this.grpNew.Text = "New";
            this.grpNew.Visible = false;
            // 
            // butConfirm
            // 
            this.butConfirm.Location = new System.Drawing.Point(175, 296);
            this.butConfirm.Name = "butConfirm";
            this.butConfirm.Size = new System.Drawing.Size(59, 23);
            this.butConfirm.TabIndex = 32;
            this.butConfirm.Text = "Confirm";
            this.butConfirm.UseVisualStyleBackColor = true;
            this.butConfirm.Click += new System.EventHandler(this.butConfirm_Click);
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.Location = new System.Drawing.Point(14, 298);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(155, 21);
            this.cmbType.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 282);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Type:";
            // 
            // Editor_Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 661);
            this.ControlBox = false;
            this.Controls.Add(this.grpOrder);
            this.Controls.Add(this.grpProperties);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSaveAll);
            this.Controls.Add(this.cmbWindows);
            this.Controls.Add(this.picWindow);
            this.Controls.Add(this.grpNew);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimizeBox = false;
            this.Name = "Editor_Interface";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Interface Editor";
            ((System.ComponentModel.ISupportInitialize)(this.picWindow)).EndInit();
            this.grpOrder.ResumeLayout(false);
            this.grpProperties.ResumeLayout(false);
            this.grpNew.ResumeLayout(false);
            this.grpNew.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button butNew;
    private System.Windows.Forms.Button butRemove;
    public System.Windows.Forms.ComboBox cmbWindows;
    public System.Windows.Forms.PictureBox picWindow;
    private System.Windows.Forms.GroupBox grpOrder;
    private System.Windows.Forms.GroupBox grpProperties;
    private System.Windows.Forms.PropertyGrid prgProperties;
    public System.Windows.Forms.TreeView treOrder;
    private System.Windows.Forms.Button butOrder_Down;
    private System.Windows.Forms.Button butOrder_Up;
    private System.Windows.Forms.Button butOrder_Unpin;
    private System.Windows.Forms.Button butOrder_Pin;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.Button butSaveAll;
    private System.Windows.Forms.GroupBox grpNew;
    private System.Windows.Forms.Button butConfirm;
    public System.Windows.Forms.ComboBox cmbType;
    private System.Windows.Forms.Label label1;
}