using DarkUI.Controls;

namespace Editors
{
    partial class Editor_Tiles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor_Tiles));
            this.scrlTileX = new System.Windows.Forms.HScrollBar();
            this.scrlTileY = new System.Windows.Forms.VScrollBar();
            this.picTile = new System.Windows.Forms.PictureBox();
            this.grpAttributes = new DarkUI.Controls.DarkGroupBox();
            this.optBlock = new DarkUI.Controls.DarkRadioButton();
            this.butClear = new DarkUI.Controls.DarkButton();
            this.butCancel = new DarkUI.Controls.DarkButton();
            this.butSave = new DarkUI.Controls.DarkButton();
            this.grpTile = new DarkUI.Controls.DarkGroupBox();
            this.scrlTile = new System.Windows.Forms.HScrollBar();
            this.groupBox2 = new DarkUI.Controls.DarkGroupBox();
            this.optAttributes = new DarkUI.Controls.DarkRadioButton();
            this.optDirBlock = new DarkUI.Controls.DarkRadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.picTile)).BeginInit();
            this.grpAttributes.SuspendLayout();
            this.grpTile.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // scrlTileX
            // 
            this.scrlTileX.LargeChange = 1;
            this.scrlTileX.Location = new System.Drawing.Point(14, 453);
            this.scrlTileX.Name = "scrlTileX";
            this.scrlTileX.Size = new System.Drawing.Size(256, 19);
            this.scrlTileX.TabIndex = 69;
            // 
            // scrlTileY
            // 
            this.scrlTileY.Cursor = System.Windows.Forms.Cursors.Default;
            this.scrlTileY.LargeChange = 1;
            this.scrlTileY.Location = new System.Drawing.Point(270, 69);
            this.scrlTileY.Maximum = 255;
            this.scrlTileY.Name = "scrlTileY";
            this.scrlTileY.Size = new System.Drawing.Size(19, 384);
            this.scrlTileY.TabIndex = 70;
            // 
            // picTile
            // 
            this.picTile.BackColor = System.Drawing.Color.Black;
            this.picTile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picTile.Location = new System.Drawing.Point(14, 69);
            this.picTile.Name = "picTile";
            this.picTile.Size = new System.Drawing.Size(256, 384);
            this.picTile.TabIndex = 68;
            this.picTile.TabStop = false;
            this.picTile.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picTile_MouseDown);
            // 
            // grpAttributes
            // 
            this.grpAttributes.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpAttributes.Controls.Add(this.optBlock);
            this.grpAttributes.Location = new System.Drawing.Point(297, 141);
            this.grpAttributes.Name = "grpAttributes";
            this.grpAttributes.Size = new System.Drawing.Size(98, 264);
            this.grpAttributes.TabIndex = 71;
            this.grpAttributes.TabStop = false;
            this.grpAttributes.Text = "Attributes";
            // 
            // optBlock
            // 
            this.optBlock.AutoSize = true;
            this.optBlock.Location = new System.Drawing.Point(6, 19);
            this.optBlock.Name = "optBlock";
            this.optBlock.Size = new System.Drawing.Size(52, 17);
            this.optBlock.TabIndex = 75;
            this.optBlock.TabStop = true;
            this.optBlock.Text = "Block";
            this.optBlock.CheckedChanged += new System.EventHandler(this.optBlock_CheckedChanged);
            // 
            // butClear
            // 
            this.butClear.Location = new System.Drawing.Point(297, 431);
            this.butClear.Name = "butClear";
            this.butClear.Padding = new System.Windows.Forms.Padding(5);
            this.butClear.Size = new System.Drawing.Size(97, 21);
            this.butClear.TabIndex = 74;
            this.butClear.Text = "Clear";
            this.butClear.Click += new System.EventHandler(this.butClear_Click);
            // 
            // butCancel
            // 
            this.butCancel.Location = new System.Drawing.Point(297, 451);
            this.butCancel.Name = "butCancel";
            this.butCancel.Padding = new System.Windows.Forms.Padding(5);
            this.butCancel.Size = new System.Drawing.Size(97, 21);
            this.butCancel.TabIndex = 73;
            this.butCancel.Text = "Cancel";
            this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
            // 
            // butSave
            // 
            this.butSave.Location = new System.Drawing.Point(297, 411);
            this.butSave.Name = "butSave";
            this.butSave.Padding = new System.Windows.Forms.Padding(5);
            this.butSave.Size = new System.Drawing.Size(97, 21);
            this.butSave.TabIndex = 72;
            this.butSave.Text = "Save";
            this.butSave.Click += new System.EventHandler(this.butSave_Click);
            // 
            // grpTile
            // 
            this.grpTile.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.grpTile.Controls.Add(this.scrlTile);
            this.grpTile.Location = new System.Drawing.Point(12, 12);
            this.grpTile.Name = "grpTile";
            this.grpTile.Size = new System.Drawing.Size(382, 49);
            this.grpTile.TabIndex = 75;
            this.grpTile.TabStop = false;
            this.grpTile.Text = "Tile: 1";
            // 
            // scrlTile
            // 
            this.scrlTile.LargeChange = 1;
            this.scrlTile.Location = new System.Drawing.Point(9, 18);
            this.scrlTile.Minimum = 1;
            this.scrlTile.Name = "scrlTile";
            this.scrlTile.Size = new System.Drawing.Size(370, 19);
            this.scrlTile.TabIndex = 16;
            this.scrlTile.Value = 1;
            this.scrlTile.ValueChanged += new System.EventHandler(this.scrlTile_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(51)))));
            this.groupBox2.Controls.Add(this.optAttributes);
            this.groupBox2.Controls.Add(this.optDirBlock);
            this.groupBox2.Location = new System.Drawing.Point(297, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(98, 66);
            this.groupBox2.TabIndex = 76;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Set";
            // 
            // optAttributes
            // 
            this.optAttributes.AutoSize = true;
            this.optAttributes.Checked = true;
            this.optAttributes.Location = new System.Drawing.Point(6, 19);
            this.optAttributes.Name = "optAttributes";
            this.optAttributes.Size = new System.Drawing.Size(69, 17);
            this.optAttributes.TabIndex = 75;
            this.optAttributes.TabStop = true;
            this.optAttributes.Text = "Attributes";
            this.optAttributes.CheckedChanged += new System.EventHandler(this.optAttributes_CheckedChanged);
            // 
            // optDirBlock
            // 
            this.optDirBlock.BackColor = System.Drawing.Color.Transparent;
            this.optDirBlock.Location = new System.Drawing.Point(6, 42);
            this.optDirBlock.Name = "optDirBlock";
            this.optDirBlock.Size = new System.Drawing.Size(91, 17);
            this.optDirBlock.TabIndex = 76;
            this.optDirBlock.Text = "Dir. Block";
            this.optDirBlock.CheckedChanged += new System.EventHandler(this.optDirBlock_CheckedChanged);
            // 
            // Editor_Tiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 487);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.butCancel);
            this.Controls.Add(this.grpTile);
            this.Controls.Add(this.butClear);
            this.Controls.Add(this.butSave);
            this.Controls.Add(this.grpAttributes);
            this.Controls.Add(this.scrlTileX);
            this.Controls.Add(this.scrlTileY);
            this.Controls.Add(this.picTile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Editor_Tiles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tile Editor";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Editor_Tiles_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.picTile)).EndInit();
            this.grpAttributes.ResumeLayout(false);
            this.grpAttributes.PerformLayout();
            this.grpTile.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.HScrollBar scrlTileX;
        public System.Windows.Forms.VScrollBar scrlTileY;
        public System.Windows.Forms.PictureBox picTile;
        private DarkGroupBox grpAttributes;
        private DarkButton butClear;
        private DarkButton butCancel;
        private DarkButton butSave;
        private DarkRadioButton optBlock;
        private DarkGroupBox grpTile;
        public System.Windows.Forms.HScrollBar scrlTile;
        private DarkGroupBox groupBox2;
        public DarkRadioButton optDirBlock;
        public DarkRadioButton optAttributes;
    }
}