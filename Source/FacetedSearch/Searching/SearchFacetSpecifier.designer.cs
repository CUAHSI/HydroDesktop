namespace FacetedSearch
{
    partial class SearchFacetSpecifier
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.SearchSQLBtn = new System.Windows.Forms.Button();
            this.NextSQLBtn = new System.Windows.Forms.Button();
            this.SearchBtn = new System.Windows.Forms.Button();
            this.NextBtn = new System.Windows.Forms.Button();
            this.FacetTextBox = new System.Windows.Forms.TextBox();
            this.FacetTree = new System.Windows.Forms.TreeView();
            this.DeleteFacetBtn = new System.Windows.Forms.Button();
            this.SearchPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ButtonsPanel.SuspendLayout();
            this.SearchPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Controls.Add(this.SearchSQLBtn);
            this.ButtonsPanel.Controls.Add(this.NextSQLBtn);
            this.ButtonsPanel.Controls.Add(this.SearchBtn);
            this.ButtonsPanel.Controls.Add(this.NextBtn);
            this.ButtonsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonsPanel.Location = new System.Drawing.Point(0, 217);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(250, 32);
            this.ButtonsPanel.TabIndex = 0;
            // 
            // SearchSQLBtn
            // 
            this.SearchSQLBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.SearchSQLBtn.Location = new System.Drawing.Point(184, 0);
            this.SearchSQLBtn.Margin = new System.Windows.Forms.Padding(5);
            this.SearchSQLBtn.Name = "SearchSQLBtn";
            this.SearchSQLBtn.Size = new System.Drawing.Size(65, 32);
            this.SearchSQLBtn.TabIndex = 3;
            this.SearchSQLBtn.Text = "Srch SQL";
            this.SearchSQLBtn.UseVisualStyleBackColor = true;
            this.SearchSQLBtn.Click += new System.EventHandler(this.SearchSQLBtn_Click);
            // 
            // NextSQLBtn
            // 
            this.NextSQLBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.NextSQLBtn.Location = new System.Drawing.Point(119, 0);
            this.NextSQLBtn.Margin = new System.Windows.Forms.Padding(5);
            this.NextSQLBtn.Name = "NextSQLBtn";
            this.NextSQLBtn.Size = new System.Drawing.Size(65, 32);
            this.NextSQLBtn.TabIndex = 2;
            this.NextSQLBtn.Text = "Next SQL";
            this.NextSQLBtn.UseVisualStyleBackColor = true;
            this.NextSQLBtn.Click += new System.EventHandler(this.NextSQLBtn_Click);
            // 
            // SearchBtn
            // 
            this.SearchBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.SearchBtn.Location = new System.Drawing.Point(61, 0);
            this.SearchBtn.Margin = new System.Windows.Forms.Padding(5);
            this.SearchBtn.Name = "SearchBtn";
            this.SearchBtn.Size = new System.Drawing.Size(58, 32);
            this.SearchBtn.TabIndex = 1;
            this.SearchBtn.Text = "Search";
            this.SearchBtn.UseVisualStyleBackColor = true;
            this.SearchBtn.Click += new System.EventHandler(this.SearchBtn_Click);
            // 
            // NextBtn
            // 
            this.NextBtn.Dock = System.Windows.Forms.DockStyle.Left;
            this.NextBtn.Location = new System.Drawing.Point(0, 0);
            this.NextBtn.Margin = new System.Windows.Forms.Padding(5);
            this.NextBtn.Name = "NextBtn";
            this.NextBtn.Size = new System.Drawing.Size(61, 32);
            this.NextBtn.TabIndex = 0;
            this.NextBtn.Text = "Next";
            this.NextBtn.UseVisualStyleBackColor = true;
            this.NextBtn.Click += new System.EventHandler(this.NextBtn_Click);
            // 
            // FacetTextBox
            // 
            this.FacetTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FacetTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FacetTextBox.Location = new System.Drawing.Point(0, 0);
            this.FacetTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.FacetTextBox.Name = "FacetTextBox";
            this.FacetTextBox.Size = new System.Drawing.Size(219, 24);
            this.FacetTextBox.TabIndex = 0;
            this.FacetTextBox.TextChanged += new System.EventHandler(this.FacetTextBox_TextChanged);
            // 
            // FacetTree
            // 
            this.FacetTree.CheckBoxes = true;
            this.FacetTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FacetTree.Location = new System.Drawing.Point(0, 0);
            this.FacetTree.Name = "FacetTree";
            this.FacetTree.ShowNodeToolTips = true;
            this.FacetTree.Size = new System.Drawing.Size(250, 188);
            this.FacetTree.TabIndex = 0;
            this.FacetTree.BeforeCheck += new System.Windows.Forms.TreeViewCancelEventHandler(this.FacetTree_BeforeCheck);
            this.FacetTree.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.FacetTree_AfterCheck);
            // 
            // DeleteFacetBtn
            // 
            this.DeleteFacetBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this.DeleteFacetBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteFacetBtn.Location = new System.Drawing.Point(219, 0);
            this.DeleteFacetBtn.Name = "DeleteFacetBtn";
            this.DeleteFacetBtn.Size = new System.Drawing.Size(31, 29);
            this.DeleteFacetBtn.TabIndex = 0;
            this.DeleteFacetBtn.Text = "X";
            this.DeleteFacetBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.DeleteFacetBtn.UseVisualStyleBackColor = true;
            this.DeleteFacetBtn.Click += new System.EventHandler(this.DeleteFacetBtn_Click);
            // 
            // SearchPanel
            // 
            this.SearchPanel.Controls.Add(this.FacetTextBox);
            this.SearchPanel.Controls.Add(this.DeleteFacetBtn);
            this.SearchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SearchPanel.Location = new System.Drawing.Point(0, 0);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(250, 29);
            this.SearchPanel.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.FacetTree);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 29);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(250, 188);
            this.panel1.TabIndex = 2;
            // 
            // SearchFacetSpecifier
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.SearchPanel);
            this.Controls.Add(this.ButtonsPanel);
            this.MinimumSize = new System.Drawing.Size(250, 250);
            this.Name = "SearchFacetSpecifier";
            this.Size = new System.Drawing.Size(250, 249);
            this.ButtonsPanel.ResumeLayout(false);
            this.SearchPanel.ResumeLayout(false);
            this.SearchPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel ButtonsPanel;
        private System.Windows.Forms.Button SearchSQLBtn;
        private System.Windows.Forms.Button NextSQLBtn;
        private System.Windows.Forms.Button SearchBtn;
        private System.Windows.Forms.Button NextBtn;
        private System.Windows.Forms.TextBox FacetTextBox;
        private System.Windows.Forms.TreeView FacetTree;
        private System.Windows.Forms.Button DeleteFacetBtn;
        private System.Windows.Forms.Panel SearchPanel;
        private System.Windows.Forms.Panel panel1;

    }
}
