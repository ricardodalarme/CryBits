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
            this.picWindow = new System.Windows.Forms.PictureBox();
            this.butClear = new System.Windows.Forms.Button();
            this.cmbTools = new System.Windows.Forms.ComboBox();
            this.List = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cmbWIndows = new System.Windows.Forms.ComboBox();
            this.grpList = new System.Windows.Forms.GroupBox();
            this.grpOrder = new System.Windows.Forms.GroupBox();
            this.treOrder = new System.Windows.Forms.TreeView();
            this.grpProperties = new System.Windows.Forms.GroupBox();
            this.butCancel = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.prgProperties = new System.Windows.Forms.PropertyGrid();
            this.optList = new System.Windows.Forms.RadioButton();
            this.optOrder = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.butOrder_Pin = new System.Windows.Forms.Button();
            this.butOrder_Unpin = new System.Windows.Forms.Button();
            this.butOrder_Up = new System.Windows.Forms.Button();
            this.butOrder_Down = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.picWindow)).BeginInit();
            this.grpList.SuspendLayout();
            this.grpOrder.SuspendLayout();
            this.grpProperties.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            // butClear
            // 
            this.butClear.Location = new System.Drawing.Point(161, 270);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(70, 25);
            this.butClear.TabIndex = 25;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // cmbTools
            // 
            this.cmbTools.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTools.Location = new System.Drawing.Point(11, 23);
            this.cmbTools.Name = "cmbTools";
            this.cmbTools.Size = new System.Drawing.Size(220, 21);
            this.cmbTools.TabIndex = 27;
            this.cmbTools.SelectedIndexChanged += new System.EventHandler(this.cmbTools_SelectedIndexChanged);
            // 
            // List
            // 
            this.List.FormattingEnabled = true;
            this.List.Location = new System.Drawing.Point(11, 52);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(220, 212);
            this.List.TabIndex = 26;
            this.List.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 270);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 25);
            this.button1.TabIndex = 28;
            this.button1.Text = "New";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(87, 270);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 25);
            this.button2.TabIndex = 29;
            this.button2.Text = "Remove";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // cmbWIndows
            // 
            this.cmbWIndows.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWIndows.Location = new System.Drawing.Point(12, 12);
            this.cmbWIndows.Name = "cmbWIndows";
            this.cmbWIndows.Size = new System.Drawing.Size(800, 21);
            this.cmbWIndows.TabIndex = 30;
            // 
            // grpList
            // 
            this.grpList.Controls.Add(this.List);
            this.grpList.Controls.Add(this.butClear);
            this.grpList.Controls.Add(this.cmbTools);
            this.grpList.Controls.Add(this.button2);
            this.grpList.Controls.Add(this.button1);
            this.grpList.Location = new System.Drawing.Point(823, 61);
            this.grpList.Name = "grpList";
            this.grpList.Size = new System.Drawing.Size(243, 308);
            this.grpList.TabIndex = 32;
            this.grpList.TabStop = false;
            this.grpList.Text = "List";
            // 
            // grpOrder
            // 
            this.grpOrder.Controls.Add(this.butOrder_Down);
            this.grpOrder.Controls.Add(this.butOrder_Up);
            this.grpOrder.Controls.Add(this.butOrder_Unpin);
            this.grpOrder.Controls.Add(this.butOrder_Pin);
            this.grpOrder.Controls.Add(this.treOrder);
            this.grpOrder.Location = new System.Drawing.Point(823, 61);
            this.grpOrder.Name = "grpOrder";
            this.grpOrder.Size = new System.Drawing.Size(243, 308);
            this.grpOrder.TabIndex = 33;
            this.grpOrder.TabStop = false;
            this.grpOrder.Text = "Order";
            this.grpOrder.Visible = false;
            // 
            // treOrder
            // 
            this.treOrder.Location = new System.Drawing.Point(10, 19);
            this.treOrder.Name = "treOrder";
            this.treOrder.Size = new System.Drawing.Size(221, 252);
            this.treOrder.TabIndex = 0;
            // 
            // grpProperties
            // 
            this.grpProperties.Controls.Add(this.butCancel);
            this.grpProperties.Controls.Add(this.butSave);
            this.grpProperties.Controls.Add(this.prgProperties);
            this.grpProperties.Location = new System.Drawing.Point(823, 375);
            this.grpProperties.Name = "grpProperties";
            this.grpProperties.Size = new System.Drawing.Size(243, 274);
            this.grpProperties.TabIndex = 34;
            this.grpProperties.TabStop = false;
            this.grpProperties.Text = "Properties";
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(124, 237);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(107, 25);
            this.butCancel.TabIndex = 27;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(11, 237);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(107, 25);
            this.butSave.TabIndex = 26;
            this.butSave.Text = "Save All";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSaveAll_Click);
            // 
            // prgProperties
            // 
            this.prgProperties.HelpVisible = false;
            this.prgProperties.Location = new System.Drawing.Point(10, 19);
            this.prgProperties.Name = "prgProperties";
            this.prgProperties.Size = new System.Drawing.Size(221, 212);
            this.prgProperties.TabIndex = 25;
            // 
            // optList
            // 
            this.optList.AutoSize = true;
            this.optList.Checked = true;
            this.optList.Location = new System.Drawing.Point(24, 19);
            this.optList.Name = "optList";
            this.optList.Size = new System.Drawing.Size(41, 17);
            this.optList.TabIndex = 35;
            this.optList.TabStop = true;
            this.optList.Text = "List";
            this.optList.UseVisualStyleBackColor = true;
            this.optList.CheckedChanged += new System.EventHandler(this.optList_CheckedChanged);
            // 
            // optOrder
            // 
            this.optOrder.AutoSize = true;
            this.optOrder.Location = new System.Drawing.Point(155, 19);
            this.optOrder.Name = "optOrder";
            this.optOrder.Size = new System.Drawing.Size(51, 17);
            this.optOrder.TabIndex = 36;
            this.optOrder.Text = "Order";
            this.optOrder.UseVisualStyleBackColor = true;
            this.optOrder.CheckedChanged += new System.EventHandler(this.optOrder_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.optList);
            this.groupBox1.Controls.Add(this.optOrder);
            this.groupBox1.Location = new System.Drawing.Point(823, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(243, 51);
            this.groupBox1.TabIndex = 37;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Menu";
            // 
            // butOrder_Pin
            // 
            this.butOrder_Pin.Location = new System.Drawing.Point(11, 277);
            this.butOrder_Pin.Name = "butOrder_Pin";
            this.butOrder_Pin.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Pin.TabIndex = 27;
            this.butOrder_Pin.Text = "Pin";
            this.butOrder_Pin.UseVisualStyleBackColor = true;
            this.butOrder_Pin.Click += new System.EventHandler(this.butOrder_Pin_Click);
            // 
            // butOrder_Unpin
            // 
            this.butOrder_Unpin.Location = new System.Drawing.Point(68, 277);
            this.butOrder_Unpin.Name = "butOrder_Unpin";
            this.butOrder_Unpin.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Unpin.TabIndex = 28;
            this.butOrder_Unpin.Text = "Unpin";
            this.butOrder_Unpin.UseVisualStyleBackColor = true;
            this.butOrder_Unpin.Click += new System.EventHandler(this.butOrder_Unpin_Click);
            // 
            // butOrder_Up
            // 
            this.butOrder_Up.Location = new System.Drawing.Point(125, 277);
            this.butOrder_Up.Name = "butOrder_Up";
            this.butOrder_Up.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Up.TabIndex = 29;
            this.butOrder_Up.Text = "Up";
            this.butOrder_Up.UseVisualStyleBackColor = true;
            this.butOrder_Up.Click += new System.EventHandler(this.butOrder_Up_Click);
            // 
            // butOrder_Down
            // 
            this.butOrder_Down.Location = new System.Drawing.Point(181, 277);
            this.butOrder_Down.Name = "butOrder_Down";
            this.butOrder_Down.Size = new System.Drawing.Size(50, 25);
            this.butOrder_Down.TabIndex = 30;
            this.butOrder_Down.Text = "Down";
            this.butOrder_Down.UseVisualStyleBackColor = true;
            this.butOrder_Down.Click += new System.EventHandler(this.butOrder_Down_Click);
            // 
            // Editor_Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1078, 661);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpProperties);
            this.Controls.Add(this.grpOrder);
            this.Controls.Add(this.grpList);
            this.Controls.Add(this.cmbWIndows);
            this.Controls.Add(this.picWindow);
            this.MinimizeBox = false;
            this.Name = "Editor_Interface";
            this.Text = "Interface Editor";
            ((System.ComponentModel.ISupportInitialize)(this.picWindow)).EndInit();
            this.grpList.ResumeLayout(false);
            this.grpOrder.ResumeLayout(false);
            this.grpProperties.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button butClear;
    public System.Windows.Forms.ComboBox cmbTools;
    public System.Windows.Forms.ListBox List;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    public System.Windows.Forms.ComboBox cmbWIndows;
    public System.Windows.Forms.PictureBox picWindow;
    private System.Windows.Forms.GroupBox grpList;
    private System.Windows.Forms.GroupBox grpOrder;
    private System.Windows.Forms.GroupBox grpProperties;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.Button butSave;
    private System.Windows.Forms.PropertyGrid prgProperties;
    private System.Windows.Forms.RadioButton optList;
    private System.Windows.Forms.RadioButton optOrder;
    private System.Windows.Forms.GroupBox groupBox1;
    public System.Windows.Forms.TreeView treOrder;
    private System.Windows.Forms.Button butOrder_Down;
    private System.Windows.Forms.Button butOrder_Up;
    private System.Windows.Forms.Button butOrder_Unpin;
    private System.Windows.Forms.Button butOrder_Pin;
}