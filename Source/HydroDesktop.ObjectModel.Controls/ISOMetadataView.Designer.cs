namespace HydroDesktop.ObjectModel.Controls
{
    partial class ISOMetadataView
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
            this.components = new System.ComponentModel.Container();
            this.lblTopicCategory = new System.Windows.Forms.Label();
            this.tbTopicCategory = new System.Windows.Forms.TextBox();
            this.tbTitle = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.tbAbstract = new System.Windows.Forms.TextBox();
            this.lblAbstract = new System.Windows.Forms.Label();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.tbProfileVersion = new System.Windows.Forms.TextBox();
            this.lblProfileVersion = new System.Windows.Forms.Label();
            this.tblMetadataLink = new System.Windows.Forms.TextBox();
            this.lblMetadataLink = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTopicCategory
            // 
            this.lblTopicCategory.AutoSize = true;
            this.lblTopicCategory.Location = new System.Drawing.Point(14, 11);
            this.lblTopicCategory.Name = "lblTopicCategory";
            this.lblTopicCategory.Size = new System.Drawing.Size(79, 13);
            this.lblTopicCategory.TabIndex = 0;
            this.lblTopicCategory.Text = "Topic Category";
            // 
            // tbTopicCategory
            // 
            this.tbTopicCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTopicCategory.Location = new System.Drawing.Point(96, 8);
            this.tbTopicCategory.Name = "tbTopicCategory";
            this.tbTopicCategory.Size = new System.Drawing.Size(166, 20);
            this.tbTopicCategory.TabIndex = 0;
            // 
            // tbTitle
            // 
            this.tbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbTitle.Location = new System.Drawing.Point(96, 34);
            this.tbTitle.Name = "tbTitle";
            this.tbTitle.Size = new System.Drawing.Size(166, 20);
            this.tbTitle.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(14, 37);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(27, 13);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Title";
            // 
            // tbAbstract
            // 
            this.tbAbstract.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbAbstract.Location = new System.Drawing.Point(96, 60);
            this.tbAbstract.Name = "tbAbstract";
            this.tbAbstract.Size = new System.Drawing.Size(166, 20);
            this.tbAbstract.TabIndex = 2;
            // 
            // lblAbstract
            // 
            this.lblAbstract.AutoSize = true;
            this.lblAbstract.Location = new System.Drawing.Point(14, 63);
            this.lblAbstract.Name = "lblAbstract";
            this.lblAbstract.Size = new System.Drawing.Size(46, 13);
            this.lblAbstract.TabIndex = 4;
            this.lblAbstract.Text = "Abstract";
            // 
            // tbProfileVersion
            // 
            this.tbProfileVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProfileVersion.Location = new System.Drawing.Point(96, 86);
            this.tbProfileVersion.Name = "tbProfileVersion";
            this.tbProfileVersion.Size = new System.Drawing.Size(166, 20);
            this.tbProfileVersion.TabIndex = 3;
            // 
            // lblProfileVersion
            // 
            this.lblProfileVersion.AutoSize = true;
            this.lblProfileVersion.Location = new System.Drawing.Point(14, 89);
            this.lblProfileVersion.Name = "lblProfileVersion";
            this.lblProfileVersion.Size = new System.Drawing.Size(74, 13);
            this.lblProfileVersion.TabIndex = 6;
            this.lblProfileVersion.Text = "Profile Version";
            // 
            // tblMetadataLink
            // 
            this.tblMetadataLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblMetadataLink.Location = new System.Drawing.Point(96, 112);
            this.tblMetadataLink.Name = "tblMetadataLink";
            this.tblMetadataLink.Size = new System.Drawing.Size(166, 20);
            this.tblMetadataLink.TabIndex = 4;
            // 
            // lblMetadataLink
            // 
            this.lblMetadataLink.AutoSize = true;
            this.lblMetadataLink.Location = new System.Drawing.Point(14, 115);
            this.lblMetadataLink.Name = "lblMetadataLink";
            this.lblMetadataLink.Size = new System.Drawing.Size(75, 13);
            this.lblMetadataLink.TabIndex = 8;
            this.lblMetadataLink.Text = "Metadata Link";
            // 
            // ISOMetadataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblMetadataLink);
            this.Controls.Add(this.lblMetadataLink);
            this.Controls.Add(this.tbProfileVersion);
            this.Controls.Add(this.lblProfileVersion);
            this.Controls.Add(this.tbAbstract);
            this.Controls.Add(this.lblAbstract);
            this.Controls.Add(this.tbTitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.tbTopicCategory);
            this.Controls.Add(this.lblTopicCategory);
            this.Name = "ISOMetadataView";
            this.Size = new System.Drawing.Size(275, 143);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTopicCategory;
        private System.Windows.Forms.TextBox tbTopicCategory;
        private System.Windows.Forms.TextBox tbTitle;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox tbAbstract;
        private System.Windows.Forms.Label lblAbstract;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TextBox tbProfileVersion;
        private System.Windows.Forms.Label lblProfileVersion;
        private System.Windows.Forms.TextBox tblMetadataLink;
        private System.Windows.Forms.Label lblMetadataLink;
    }
}
