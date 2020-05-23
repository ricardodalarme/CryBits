partial class Preview
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preview));
            this.List = new System.Windows.Forms.ListBox();
            this.butSelecionar = new System.Windows.Forms.Button();
            this.chkTransparent = new System.Windows.Forms.CheckBox();
            this.scrlImageY = new System.Windows.Forms.VScrollBar();
            this.scrlImageX = new System.Windows.Forms.HScrollBar();
            this.picImage = new System.Windows.Forms.PictureBox();
            this.tmrRender = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
            this.SuspendLayout();
            // 
            // List
            // 
            this.List.FormattingEnabled = true;
            this.List.Location = new System.Drawing.Point(12, 12);
            this.List.Name = "List";
            this.List.Size = new System.Drawing.Size(202, 342);
            this.List.TabIndex = 5;
            this.List.SelectedIndexChanged += new System.EventHandler(this.lstList_SelectedIndexChanged);
            // 
            // butSelecionar
            // 
            this.butSelecionar.Location = new System.Drawing.Point(12, 355);
            this.butSelecionar.Name = "butSelecionar";
            this.butSelecionar.Size = new System.Drawing.Size(202, 25);
            this.butSelecionar.TabIndex = 21;
            this.butSelecionar.Text = "Select";
            this.butSelecionar.UseVisualStyleBackColor = true;
            this.butSelecionar.Click += new System.EventHandler(this.butSelect_Click);
            // 
            // chkTransparent
            // 
            this.chkTransparent.AutoSize = true;
            this.chkTransparent.Checked = true;
            this.chkTransparent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTransparent.Location = new System.Drawing.Point(220, 12);
            this.chkTransparent.Name = "chkTransparent";
            this.chkTransparent.Size = new System.Drawing.Size(149, 17);
            this.chkTransparent.TabIndex = 22;
            this.chkTransparent.Text = "Transparent background?";
            this.chkTransparent.UseVisualStyleBackColor = true;
            // 
            // scrlImageY
            // 
            this.scrlImageY.Location = new System.Drawing.Point(546, 32);
            this.scrlImageY.Name = "scrlImageY";
            this.scrlImageY.Size = new System.Drawing.Size(19, 328);
            this.scrlImageY.TabIndex = 29;
            // 
            // scrlImageX
            // 
            this.scrlImageX.Location = new System.Drawing.Point(220, 361);
            this.scrlImageX.Name = "scrlImageX";
            this.scrlImageX.Size = new System.Drawing.Size(325, 19);
            this.scrlImageX.TabIndex = 28;
            // 
            // picImage
            // 
            this.picImage.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.picImage.Location = new System.Drawing.Point(220, 35);
            this.picImage.Name = "picImage";
            this.picImage.Size = new System.Drawing.Size(325, 325);
            this.picImage.TabIndex = 27;
            this.picImage.TabStop = false;
            // 
            // tmrRender
            // 
            this.tmrRender.Enabled = true;
            this.tmrRender.Interval = 1;
            this.tmrRender.Tick += new System.EventHandler(this.tmpRender_Tick);
            // 
            // Preview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 386);
            this.Controls.Add(this.scrlImageY);
            this.Controls.Add(this.scrlImageX);
            this.Controls.Add(this.picImage);
            this.Controls.Add(this.chkTransparent);
            this.Controls.Add(this.butSelecionar);
            this.Controls.Add(this.List);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Preview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preview";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreView_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    public System.Windows.Forms.ListBox List;
    private System.Windows.Forms.Button butSelecionar;
    public System.Windows.Forms.VScrollBar scrlImageY;
    public System.Windows.Forms.HScrollBar scrlImageX;
    public System.Windows.Forms.PictureBox picImage;
    public System.Windows.Forms.CheckBox chkTransparent;
    private System.Windows.Forms.Timer tmrRender;
}