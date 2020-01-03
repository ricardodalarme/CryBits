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
            this.prgProperties = new System.Windows.Forms.PropertyGrid();
            this.picWindow = new System.Windows.Forms.PictureBox();
            this.butClear = new System.Windows.Forms.Button();
            this.butCancel = new System.Windows.Forms.Button();
            this.butSave = new System.Windows.Forms.Button();
            this.cmbTools = new System.Windows.Forms.ComboBox();
            this.List = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cmbWIndows = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.picWindow)).BeginInit();
            this.SuspendLayout();
            // 
            // prgProperties
            // 
            this.prgProperties.HelpVisible = false;
            this.prgProperties.Location = new System.Drawing.Point(823, 384);
            this.prgProperties.Name = "prgProperties";
            this.prgProperties.Size = new System.Drawing.Size(221, 233);
            this.prgProperties.TabIndex = 0;
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
            this.butClear.Location = new System.Drawing.Point(974, 350);
            this.butClear.Name = "butClear";
            this.butClear.Size = new System.Drawing.Size(70, 25);
            this.butClear.TabIndex = 25;
            this.butClear.Text = "Clear";
            this.butClear.UseVisualStyleBackColor = true;
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(937, 623);
            this.butCancel.Name = "butCancel";
            this.butCancel.Size = new System.Drawing.Size(107, 25);
            this.butCancel.TabIndex = 24;
            this.butCancel.Text = "Cancel";
            this.butCancel.UseVisualStyleBackColor = true;
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(824, 624);
            this.butSave.Name = "butSave";
            this.butSave.Size = new System.Drawing.Size(107, 25);
            this.butSave.TabIndex = 23;
            this.butSave.Text = "Save All";
            this.butSave.UseVisualStyleBackColor = true;
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // cmbTools
            // 
            this.cmbTools.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTools.Location = new System.Drawing.Point(824, 12);
            this.cmbTools.Name = "cmbTools";
            this.cmbTools.Size = new System.Drawing.Size(220, 21);
            this.cmbTools.TabIndex = 27;
            this.cmbTools.SelectedIndexChanged += new System.EventHandler(this.cmbTools_SelectedIndexChanged);
            // 
            // List
            // 
            this.List.FormattingEnabled = true;
            this.List.Location = new System.Drawing.Point(824, 41);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(220, 303);
            this.List.TabIndex = 26;
            this.List.SelectedIndexChanged += new System.EventHandler(this.List_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(824, 350);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 25);
            this.button1.TabIndex = 28;
            this.button1.Text = "New";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(899, 350);
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
            // Editor_Interface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1053, 661);
            this.ControlBox = false;
            this.Controls.Add(this.cmbWIndows);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cmbTools);
            this.Controls.Add(this.List);
            this.Controls.Add(this.butClear);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.picWindow);
            this.Controls.Add(this.prgProperties);
            this.MinimizeBox = false;
            this.Name = "Editor_Interface";
            this.Text = "Interface Editor";
            ((System.ComponentModel.ISupportInitialize)(this.picWindow)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PropertyGrid prgProperties;
    private System.Windows.Forms.Button butClear;
    private System.Windows.Forms.Button butCancel;
    private System.Windows.Forms.Button butSave;
    public System.Windows.Forms.ComboBox cmbTools;
    public System.Windows.Forms.ListBox List;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    public System.Windows.Forms.ComboBox cmbWIndows;
    public System.Windows.Forms.PictureBox picWindow;
}