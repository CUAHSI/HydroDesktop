namespace Oatc.OpenMI.Gui.ConfigurationEditor
{
    partial class FactoriesDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FactoriesDialog));
            this.cbFactories = new System.Windows.Forms.ComboBox();
            this.btnAddFactory = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFactoryDetails = new System.Windows.Forms.Button();
            this.clbAdapters = new System.Windows.Forms.CheckedListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbFactories
            // 
            this.cbFactories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbFactories.FormattingEnabled = true;
            this.cbFactories.Location = new System.Drawing.Point(6, 19);
            this.cbFactories.Name = "cbFactories";
            this.cbFactories.Size = new System.Drawing.Size(385, 21);
            this.cbFactories.TabIndex = 0;
            // 
            // btnAddFactory
            // 
            this.btnAddFactory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddFactory.Location = new System.Drawing.Point(395, 18);
            this.btnAddFactory.Name = "btnAddFactory";
            this.btnAddFactory.Size = new System.Drawing.Size(29, 23);
            this.btnAddFactory.TabIndex = 2;
            this.btnAddFactory.Text = "...";
            this.btnAddFactory.UseVisualStyleBackColor = true;
            this.btnAddFactory.Click += new System.EventHandler(this.OnAddFactory);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnFactoryDetails);
            this.groupBox1.Controls.Add(this.cbFactories);
            this.groupBox1.Controls.Add(this.btnAddFactory);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(514, 51);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Factory";
            // 
            // btnFactoryDetails
            // 
            this.btnFactoryDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFactoryDetails.Location = new System.Drawing.Point(430, 18);
            this.btnFactoryDetails.Name = "btnFactoryDetails";
            this.btnFactoryDetails.Size = new System.Drawing.Size(75, 23);
            this.btnFactoryDetails.TabIndex = 5;
            this.btnFactoryDetails.Text = "Details ...";
            this.btnFactoryDetails.UseVisualStyleBackColor = true;
            this.btnFactoryDetails.Click += new System.EventHandler(this.OnFactoryDetails);
            // 
            // clbAdapters
            // 
            this.clbAdapters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.clbAdapters.FormattingEnabled = true;
            this.clbAdapters.Location = new System.Drawing.Point(6, 15);
            this.clbAdapters.Name = "clbAdapters";
            this.clbAdapters.Size = new System.Drawing.Size(502, 169);
            this.clbAdapters.TabIndex = 4;
            this.clbAdapters.SelectedIndexChanged += new System.EventHandler(this.OnSelectionChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(452, 264);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAdd.Location = new System.Drawing.Point(371, 264);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.clbAdapters);
            this.groupBox2.Location = new System.Drawing.Point(13, 69);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(514, 189);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Source adapters";
            // 
            // FactoriesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 299);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FactoriesDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Add additional sources";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbFactories;
        private System.Windows.Forms.Button btnAddFactory;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox clbAdapters;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnFactoryDetails;
    }
}