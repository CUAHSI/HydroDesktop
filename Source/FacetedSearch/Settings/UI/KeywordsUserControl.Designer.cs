namespace FacetedSearch.Settings.UI
{
    partial class KeywordsUserControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeywordsUserControl));
            this.tboTypeKeyword = new System.Windows.Forms.TextBox();
            this.lblKeywords = new System.Windows.Forms.Label();
            this.lblKeywordRelation = new System.Windows.Forms.Label();
            this.btnAddKeyword = new System.Windows.Forms.Button();
            this.btnRemoveKeyword = new System.Windows.Forms.Button();
            this.groupboxKeywordDisplay = new System.Windows.Forms.GroupBox();
            this.rbBoth = new System.Windows.Forms.RadioButton();
            this.rbTree = new System.Windows.Forms.RadioButton();
            this.rbList = new System.Windows.Forms.RadioButton();
            this.spcKey = new System.Windows.Forms.SplitContainer();
            this.lbKeywords = new System.Windows.Forms.ListBox();
            this.treeviewOntology = new System.Windows.Forms.TreeView();
            this.lblSelectedKeywords = new System.Windows.Forms.Label();
            this.lbSelectedKeywords = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupboxKeywordDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcKey)).BeginInit();
            this.spcKey.Panel1.SuspendLayout();
            this.spcKey.Panel2.SuspendLayout();
            this.spcKey.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tboTypeKeyword
            // 
            this.tboTypeKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tboTypeKeyword.Location = new System.Drawing.Point(109, 10);
            this.tboTypeKeyword.Name = "tboTypeKeyword";
            this.tboTypeKeyword.Size = new System.Drawing.Size(174, 20);
            this.tboTypeKeyword.TabIndex = 52;
            this.tboTypeKeyword.TextChanged += new System.EventHandler(this.tboTypeKeyword_TextChanged);
            // 
            // lblKeywords
            // 
            this.lblKeywords.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblKeywords.Location = new System.Drawing.Point(0, 0);
            this.lblKeywords.Name = "lblKeywords";
            this.lblKeywords.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblKeywords.Size = new System.Drawing.Size(103, 40);
            this.lblKeywords.TabIndex = 53;
            this.lblKeywords.Text = "Keywords: Type-in first few letters";
            // 
            // lblKeywordRelation
            // 
            this.lblKeywordRelation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblKeywordRelation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblKeywordRelation.Location = new System.Drawing.Point(14, 44);
            this.lblKeywordRelation.Name = "lblKeywordRelation";
            this.lblKeywordRelation.Size = new System.Drawing.Size(269, 30);
            this.lblKeywordRelation.TabIndex = 54;
            // 
            // btnAddKeyword
            // 
            this.btnAddKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddKeyword.Image = ((System.Drawing.Image)(resources.GetObject("btnAddKeyword.Image")));
            this.btnAddKeyword.Location = new System.Drawing.Point(14, 277);
            this.btnAddKeyword.Name = "btnAddKeyword";
            this.btnAddKeyword.Size = new System.Drawing.Size(25, 22);
            this.btnAddKeyword.TabIndex = 59;
            this.btnAddKeyword.UseVisualStyleBackColor = true;
            this.btnAddKeyword.Click += new System.EventHandler(this.btnAddKeyword_Click);
            // 
            // btnRemoveKeyword
            // 
            this.btnRemoveKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveKeyword.Image = ((System.Drawing.Image)(resources.GetObject("btnRemoveKeyword.Image")));
            this.btnRemoveKeyword.Location = new System.Drawing.Point(45, 277);
            this.btnRemoveKeyword.Name = "btnRemoveKeyword";
            this.btnRemoveKeyword.Size = new System.Drawing.Size(25, 22);
            this.btnRemoveKeyword.TabIndex = 60;
            this.btnRemoveKeyword.UseVisualStyleBackColor = true;
            this.btnRemoveKeyword.Click += new System.EventHandler(this.button15_Click);
            // 
            // groupboxKeywordDisplay
            // 
            this.groupboxKeywordDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupboxKeywordDisplay.Controls.Add(this.rbBoth);
            this.groupboxKeywordDisplay.Controls.Add(this.rbTree);
            this.groupboxKeywordDisplay.Controls.Add(this.rbList);
            this.groupboxKeywordDisplay.Location = new System.Drawing.Point(125, 263);
            this.groupboxKeywordDisplay.Name = "groupboxKeywordDisplay";
            this.groupboxKeywordDisplay.Size = new System.Drawing.Size(158, 40);
            this.groupboxKeywordDisplay.TabIndex = 61;
            this.groupboxKeywordDisplay.TabStop = false;
            this.groupboxKeywordDisplay.Text = "Keywords Display";
            // 
            // rbBoth
            // 
            this.rbBoth.AutoSize = true;
            this.rbBoth.Checked = true;
            this.rbBoth.Location = new System.Drawing.Point(97, 17);
            this.rbBoth.Name = "rbBoth";
            this.rbBoth.Size = new System.Drawing.Size(47, 17);
            this.rbBoth.TabIndex = 2;
            this.rbBoth.TabStop = true;
            this.rbBoth.Text = "Both";
            this.rbBoth.UseVisualStyleBackColor = true;
            // 
            // rbTree
            // 
            this.rbTree.AutoSize = true;
            this.rbTree.Location = new System.Drawing.Point(48, 17);
            this.rbTree.Name = "rbTree";
            this.rbTree.Size = new System.Drawing.Size(47, 17);
            this.rbTree.TabIndex = 1;
            this.rbTree.Text = "Tree";
            this.rbTree.UseVisualStyleBackColor = true;
            // 
            // rbList
            // 
            this.rbList.AutoSize = true;
            this.rbList.Location = new System.Drawing.Point(4, 17);
            this.rbList.Name = "rbList";
            this.rbList.Size = new System.Drawing.Size(41, 17);
            this.rbList.TabIndex = 0;
            this.rbList.Text = "List";
            this.rbList.UseVisualStyleBackColor = true;
            // 
            // spcKey
            // 
            this.spcKey.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.spcKey.Location = new System.Drawing.Point(3, 80);
            this.spcKey.Name = "spcKey";
            // 
            // spcKey.Panel1
            // 
            this.spcKey.Panel1.Controls.Add(this.lbKeywords);
            // 
            // spcKey.Panel2
            // 
            this.spcKey.Panel2.Controls.Add(this.treeviewOntology);
            this.spcKey.Size = new System.Drawing.Size(292, 179);
            this.spcKey.SplitterDistance = 95;
            this.spcKey.TabIndex = 62;
            // 
            // lbKeywords
            // 
            this.lbKeywords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbKeywords.FormattingEnabled = true;
            this.lbKeywords.HorizontalScrollbar = true;
            this.lbKeywords.Location = new System.Drawing.Point(0, 0);
            this.lbKeywords.Name = "lbKeywords";
            this.lbKeywords.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbKeywords.Size = new System.Drawing.Size(95, 179);
            this.lbKeywords.Sorted = true;
            this.lbKeywords.TabIndex = 42;
            this.lbKeywords.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbKeywords_MouseUp);
            // 
            // treeviewOntology
            // 
            this.treeviewOntology.BackColor = System.Drawing.SystemColors.Window;
            this.treeviewOntology.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeviewOntology.HideSelection = false;
            this.treeviewOntology.Indent = 19;
            this.treeviewOntology.Location = new System.Drawing.Point(0, 0);
            this.treeviewOntology.Name = "treeviewOntology";
            this.treeviewOntology.Size = new System.Drawing.Size(193, 179);
            this.treeviewOntology.TabIndex = 39;
            this.treeviewOntology.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvOntology_AfterSelect);
            // 
            // lblSelectedKeywords
            // 
            this.lblSelectedKeywords.AutoSize = true;
            this.lblSelectedKeywords.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSelectedKeywords.Location = new System.Drawing.Point(0, 0);
            this.lblSelectedKeywords.Name = "lblSelectedKeywords";
            this.lblSelectedKeywords.Size = new System.Drawing.Size(98, 13);
            this.lblSelectedKeywords.TabIndex = 64;
            this.lblSelectedKeywords.Text = "Selected Keywords";
            // 
            // lbSelectedKeywords
            // 
            this.lbSelectedKeywords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbSelectedKeywords.ForeColor = System.Drawing.Color.Gray;
            this.lbSelectedKeywords.FormattingEnabled = true;
            this.lbSelectedKeywords.Location = new System.Drawing.Point(0, 13);
            this.lbSelectedKeywords.Name = "lbSelectedKeywords";
            this.lbSelectedKeywords.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.lbSelectedKeywords.Size = new System.Drawing.Size(295, 85);
            this.lbSelectedKeywords.TabIndex = 63;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lbSelectedKeywords);
            this.panel1.Controls.Add(this.lblSelectedKeywords);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 309);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(295, 98);
            this.panel1.TabIndex = 65;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tboTypeKeyword);
            this.panel2.Controls.Add(this.lblKeywords);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(295, 40);
            this.panel2.TabIndex = 66;
            // 
            // KeywordsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnRemoveKeyword);
            this.Controls.Add(this.groupboxKeywordDisplay);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnAddKeyword);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.spcKey);
            this.Controls.Add(this.lblKeywordRelation);
            this.Name = "KeywordsUserControl";
            this.Size = new System.Drawing.Size(295, 407);
            this.groupboxKeywordDisplay.ResumeLayout(false);
            this.groupboxKeywordDisplay.PerformLayout();
            this.spcKey.Panel1.ResumeLayout(false);
            this.spcKey.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcKey)).EndInit();
            this.spcKey.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox tboTypeKeyword;
        private System.Windows.Forms.Label lblKeywords;
        private System.Windows.Forms.Label lblKeywordRelation;
        private System.Windows.Forms.Button btnAddKeyword;
        private System.Windows.Forms.Button btnRemoveKeyword;
        private System.Windows.Forms.GroupBox groupboxKeywordDisplay;
        private System.Windows.Forms.RadioButton rbBoth;
        private System.Windows.Forms.RadioButton rbTree;
        private System.Windows.Forms.RadioButton rbList;
        private System.Windows.Forms.SplitContainer spcKey;
        private System.Windows.Forms.ListBox lbKeywords;
        private System.Windows.Forms.TreeView treeviewOntology;
        private System.Windows.Forms.Label lblSelectedKeywords;
        private System.Windows.Forms.ListBox lbSelectedKeywords;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}
