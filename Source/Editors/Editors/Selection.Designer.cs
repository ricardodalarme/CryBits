partial class Selection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Selection));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDirectory_Client = new System.Windows.Forms.TextBox();
            this.butDirectory_Client = new System.Windows.Forms.Button();
            this.Directory_Client = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.butCharacter = new System.Windows.Forms.Button();
            this.butItems = new System.Windows.Forms.Button();
            this.butNPCs = new System.Windows.Forms.Button();
            this.butTiles = new System.Windows.Forms.Button();
            this.butData = new System.Windows.Forms.Button();
            this.butInterface = new System.Windows.Forms.Button();
            this.butMaps = new System.Windows.Forms.Button();
            this.butClasses = new System.Windows.Forms.Button();
            this.Directory_Server = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtDirectory_Client);
            this.groupBox1.Controls.Add(this.butDirectory_Client);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(306, 45);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Directories";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Client";
            // 
            // txtDirectory_Client
            // 
            this.txtDirectory_Client.Location = new System.Drawing.Point(61, 19);
            this.txtDirectory_Client.Name = "txtDirectory_Client";
            this.txtDirectory_Client.ReadOnly = true;
            this.txtDirectory_Client.Size = new System.Drawing.Size(204, 20);
            this.txtDirectory_Client.TabIndex = 3;
            // 
            // butDirectory_Client
            // 
            this.butDirectory_Client.Location = new System.Drawing.Point(271, 18);
            this.butDirectory_Client.Name = "butDirectory_Client";
            this.butDirectory_Client.Size = new System.Drawing.Size(29, 21);
            this.butDirectory_Client.TabIndex = 2;
            this.butDirectory_Client.Text = "...";
            this.butDirectory_Client.UseVisualStyleBackColor = true;
            this.butDirectory_Client.Click += new System.EventHandler(this.butDirectory_Client_Click);
            // 
            // Directory_Client
            // 
            this.Directory_Client.Description = "Select the client directory";
            this.Directory_Client.Tag = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.butCharacter);
            this.groupBox2.Controls.Add(this.butItems);
            this.groupBox2.Controls.Add(this.butNPCs);
            this.groupBox2.Controls.Add(this.butTiles);
            this.groupBox2.Controls.Add(this.butData);
            this.groupBox2.Controls.Add(this.butInterface);
            this.groupBox2.Controls.Add(this.butMaps);
            this.groupBox2.Controls.Add(this.butClasses);
            this.groupBox2.Location = new System.Drawing.Point(12, 63);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(306, 121);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Editors";
            // 
            // butCharacter
            // 
            this.butCharacter.Location = new System.Drawing.Point(206, 19);
            this.butCharacter.Name = "butCharacter";
            this.butCharacter.Size = new System.Drawing.Size(94, 26);
            this.butCharacter.TabIndex = 13;
            this.butCharacter.Text = "Sprites";
            this.butCharacter.UseVisualStyleBackColor = true;
            this.butCharacter.Click += new System.EventHandler(this.butCharacter_Click);
            // 
            // butItems
            // 
            this.butItems.Location = new System.Drawing.Point(106, 83);
            this.butItems.Name = "butItems";
            this.butItems.Size = new System.Drawing.Size(94, 26);
            this.butItems.TabIndex = 12;
            this.butItems.Text = "Items";
            this.butItems.UseVisualStyleBackColor = true;
            this.butItems.Click += new System.EventHandler(this.butItems_Click);
            // 
            // butNPCs
            // 
            this.butNPCs.Location = new System.Drawing.Point(106, 51);
            this.butNPCs.Name = "butNPCs";
            this.butNPCs.Size = new System.Drawing.Size(94, 26);
            this.butNPCs.TabIndex = 11;
            this.butNPCs.Text = "NPCs";
            this.butNPCs.UseVisualStyleBackColor = true;
            this.butNPCs.Click += new System.EventHandler(this.butNPCs_Click);
            // 
            // butTiles
            // 
            this.butTiles.Location = new System.Drawing.Point(206, 51);
            this.butTiles.Name = "butTiles";
            this.butTiles.Size = new System.Drawing.Size(94, 26);
            this.butTiles.TabIndex = 10;
            this.butTiles.Text = "Tiles";
            this.butTiles.UseVisualStyleBackColor = true;
            this.butTiles.Click += new System.EventHandler(this.butTiles_Click);
            // 
            // butData
            // 
            this.butData.Location = new System.Drawing.Point(6, 19);
            this.butData.Name = "butData";
            this.butData.Size = new System.Drawing.Size(94, 26);
            this.butData.TabIndex = 9;
            this.butData.Text = "Data";
            this.butData.UseVisualStyleBackColor = true;
            this.butData.Click += new System.EventHandler(this.butData_Click);
            // 
            // butInterface
            // 
            this.butInterface.Location = new System.Drawing.Point(106, 19);
            this.butInterface.Name = "butInterface";
            this.butInterface.Size = new System.Drawing.Size(94, 26);
            this.butInterface.TabIndex = 8;
            this.butInterface.Text = "Interface";
            this.butInterface.UseVisualStyleBackColor = true;
            this.butInterface.Click += new System.EventHandler(this.butInterface_Click);
            // 
            // butMaps
            // 
            this.butMaps.Location = new System.Drawing.Point(6, 83);
            this.butMaps.Name = "butMaps";
            this.butMaps.Size = new System.Drawing.Size(94, 26);
            this.butMaps.TabIndex = 7;
            this.butMaps.Text = "Maps";
            this.butMaps.UseVisualStyleBackColor = true;
            this.butMaps.Click += new System.EventHandler(this.butMaps_Click);
            // 
            // butClasses
            // 
            this.butClasses.Location = new System.Drawing.Point(6, 51);
            this.butClasses.Name = "butClasses";
            this.butClasses.Size = new System.Drawing.Size(94, 26);
            this.butClasses.TabIndex = 6;
            this.butClasses.Text = "Classes";
            this.butClasses.UseVisualStyleBackColor = true;
            this.butClasses.Click += new System.EventHandler(this.butClasses_Click);
            // 
            // Directory_Server
            // 
            this.Directory_Server.Description = "Select the server directory";
            this.Directory_Server.Tag = "";
            // 
            // Selection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(330, 196);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Selection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Editors";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Selection_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Button butDirectory_Client;
    public System.Windows.Forms.FolderBrowserDialog Directory_Client;
    private System.Windows.Forms.GroupBox groupBox2;
    public System.Windows.Forms.FolderBrowserDialog Directory_Server;
    public System.Windows.Forms.TextBox txtDirectory_Client;
    private System.Windows.Forms.Button butClasses;
    private System.Windows.Forms.Button butMaps;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button butInterface;
    private System.Windows.Forms.Button butData;
    private System.Windows.Forms.Button butTiles;
    private System.Windows.Forms.Button butNPCs;
    private System.Windows.Forms.Button butItems;
    private System.Windows.Forms.Button butCharacter;
}