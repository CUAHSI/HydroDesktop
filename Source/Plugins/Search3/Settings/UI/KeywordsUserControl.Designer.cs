
namespace HydroDesktop.Plugins.Search.Settings.UI
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
            this.lblSelectedKeywords = new System.Windows.Forms.Label();
            this.lbSelectedKeywords = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.treeviewOntology = new System.Windows.Forms.TreeView();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tboTypeKeyword
            // 
            this.tboTypeKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tboTypeKeyword.ForeColor = System.Drawing.Color.Gray;
            this.tboTypeKeyword.Location = new System.Drawing.Point(14, 10);
            this.tboTypeKeyword.Name = "tboTypeKeyword";
            this.tboTypeKeyword.Size = new System.Drawing.Size(269, 20);
            this.tboTypeKeyword.TabIndex = 52;
            // 
            // lblKeywords
            // 
            this.lblKeywords.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblKeywords.Location = new System.Drawing.Point(0, 0);
            this.lblKeywords.Name = "lblKeywords";
            this.lblKeywords.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblKeywords.Size = new System.Drawing.Size(103, 40);
            this.lblKeywords.TabIndex = 53;
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
            this.btnRemoveKeyword.Click += new System.EventHandler(this.btnRemoveKeyword_Click);
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
            this.lbSelectedKeywords.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
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
            // treeviewOntology
            // 
            this.treeviewOntology.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeviewOntology.BackColor = System.Drawing.SystemColors.Window;
            this.treeviewOntology.HideSelection = false;
            this.treeviewOntology.Indent = 19;
            this.treeviewOntology.Location = new System.Drawing.Point(3, 92);
            this.treeviewOntology.Name = "treeviewOntology";
            this.treeviewOntology.Size = new System.Drawing.Size(289, 179);
            this.treeviewOntology.TabIndex = 39;
            // 
            // radioButton1
            // 
            this.radioButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(123, 277);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(207, 17);
            this.radioButton1.TabIndex = 67;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Find sites with ANY selected keywords";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(123, 299);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(204, 17);
            this.radioButton2.TabIndex = 68;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Find sites with ALL selected keywords";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // KeywordsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.treeviewOntology);
            this.Controls.Add(this.btnRemoveKeyword);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnAddKeyword);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblKeywordRelation);
            this.Name = "KeywordsUserControl";
            this.Size = new System.Drawing.Size(295, 407);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tboTypeKeyword;
        private System.Windows.Forms.Label lblKeywords;
        private System.Windows.Forms.Label lblKeywordRelation;
        private System.Windows.Forms.Button btnAddKeyword;
        private System.Windows.Forms.Button btnRemoveKeyword;
        private System.Windows.Forms.Label lblSelectedKeywords;
        private System.Windows.Forms.ListBox lbSelectedKeywords;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TreeView treeviewOntology;
        internal System.Windows.Forms.RadioButton radioButton1;
        internal System.Windows.Forms.RadioButton radioButton2;
    }
}
