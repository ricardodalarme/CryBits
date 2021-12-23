using DarkUI.Controls;

namespace CryBits.Editors.Forms
{
    partial class EditorItemType
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorItemType));
            this.butSelect = new DarkUI.Controls.DarkButton();
            this.cmbType = new DarkUI.Controls.DarkComboBox();
            this.label2 = new DarkUI.Controls.DarkLabel();
            this.SuspendLayout();
            // 
            // butSelect
            // 
            this.butSelect.Location = new System.Drawing.Point(13, 70);
            this.butSelect.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.butSelect.Name = "butSelect";
            this.butSelect.Padding = new System.Windows.Forms.Padding(6);
            this.butSelect.Size = new System.Drawing.Size(335, 29);
            this.butSelect.TabIndex = 19;
            this.butSelect.Text = "Select";
            this.butSelect.Click += new System.EventHandler(this.butSelect_Click);
            // 
            // cmbType
            // 
            this.cmbType.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(13, 28);
            this.cmbType.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(335, 24);
            this.cmbType.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.label2.Location = new System.Drawing.Point(9, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 15);
            this.label2.TabIndex = 22;
            this.label2.Text = "Type:";
            // 
            // EditorItemType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 110);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.butSelect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorItemType";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select item type";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DarkButton butSelect;
        private DarkComboBox cmbType;
        private DarkLabel label2;
    }
}