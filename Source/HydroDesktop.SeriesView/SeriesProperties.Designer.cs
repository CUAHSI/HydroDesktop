namespace SeriesView
{
    partial class SeriesProperties
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
            this.btnOK = new System.Windows.Forms.Button();
            this.tbProperties = new System.Windows.Forms.TabControl();
            this.tbSite = new System.Windows.Forms.TabPage();
            this.siteView1 = new HydroDesktop.ObjectModel.Controls.SiteView();
            this.tbVariable = new System.Windows.Forms.TabPage();
            this.variableView1 = new HydroDesktop.ObjectModel.Controls.VariableView();
            this.tbMethod = new System.Windows.Forms.TabPage();
            this.methodView1 = new HydroDesktop.ObjectModel.Controls.MethodView();
            this.tbSource = new System.Windows.Forms.TabPage();
            this.sourceView1 = new HydroDesktop.ObjectModel.Controls.SourceView();
            this.tbProperties.SuspendLayout();
            this.tbSite.SuspendLayout();
            this.tbVariable.SuspendLayout();
            this.tbMethod.SuspendLayout();
            this.tbSource.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(280, 399);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tbProperties
            // 
            this.tbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProperties.Controls.Add(this.tbSite);
            this.tbProperties.Controls.Add(this.tbVariable);
            this.tbProperties.Controls.Add(this.tbMethod);
            this.tbProperties.Controls.Add(this.tbSource);
            this.tbProperties.Location = new System.Drawing.Point(13, 13);
            this.tbProperties.Name = "tbProperties";
            this.tbProperties.SelectedIndex = 0;
            this.tbProperties.Size = new System.Drawing.Size(342, 376);
            this.tbProperties.TabIndex = 1;
            // 
            // tbSite
            // 
            this.tbSite.Controls.Add(this.siteView1);
            this.tbSite.Location = new System.Drawing.Point(4, 22);
            this.tbSite.Name = "tbSite";
            this.tbSite.Padding = new System.Windows.Forms.Padding(3);
            this.tbSite.Size = new System.Drawing.Size(334, 350);
            this.tbSite.TabIndex = 0;
            this.tbSite.Text = "Site";
            this.tbSite.UseVisualStyleBackColor = true;
            // 
            // siteView1
            // 
            this.siteView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.siteView1.Location = new System.Drawing.Point(3, 3);
            this.siteView1.Name = "siteView1";
            this.siteView1.ReadOnly = false;
            this.siteView1.Size = new System.Drawing.Size(328, 344);
            this.siteView1.TabIndex = 0;
            // 
            // tbVariable
            // 
            this.tbVariable.Controls.Add(this.variableView1);
            this.tbVariable.Location = new System.Drawing.Point(4, 22);
            this.tbVariable.Name = "tbVariable";
            this.tbVariable.Padding = new System.Windows.Forms.Padding(3);
            this.tbVariable.Size = new System.Drawing.Size(334, 350);
            this.tbVariable.TabIndex = 1;
            this.tbVariable.Text = "Variable";
            this.tbVariable.UseVisualStyleBackColor = true;
            // 
            // variableView1
            // 
            this.variableView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.variableView1.Location = new System.Drawing.Point(3, 3);
            this.variableView1.Name = "variableView1";
            this.variableView1.ReadOnly = false;
            this.variableView1.Size = new System.Drawing.Size(328, 344);
            this.variableView1.TabIndex = 0;
            // 
            // tbMethod
            // 
            this.tbMethod.Controls.Add(this.methodView1);
            this.tbMethod.Location = new System.Drawing.Point(4, 22);
            this.tbMethod.Name = "tbMethod";
            this.tbMethod.Padding = new System.Windows.Forms.Padding(3);
            this.tbMethod.Size = new System.Drawing.Size(334, 350);
            this.tbMethod.TabIndex = 2;
            this.tbMethod.Text = "Method";
            this.tbMethod.UseVisualStyleBackColor = true;
            // 
            // methodView1
            // 
            this.methodView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodView1.Location = new System.Drawing.Point(3, 3);
            this.methodView1.Name = "methodView1";
            this.methodView1.ReadOnly = false;
            this.methodView1.Size = new System.Drawing.Size(328, 344);
            this.methodView1.TabIndex = 0;
            // 
            // tbSource
            // 
            this.tbSource.Controls.Add(this.sourceView1);
            this.tbSource.Location = new System.Drawing.Point(4, 22);
            this.tbSource.Name = "tbSource";
            this.tbSource.Padding = new System.Windows.Forms.Padding(3);
            this.tbSource.Size = new System.Drawing.Size(334, 350);
            this.tbSource.TabIndex = 3;
            this.tbSource.Text = "Source";
            this.tbSource.UseVisualStyleBackColor = true;
            // 
            // sourceView1
            // 
            this.sourceView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceView1.Location = new System.Drawing.Point(3, 3);
            this.sourceView1.MinimumSize = new System.Drawing.Size(200, 355);
            this.sourceView1.Name = "sourceView1";
            this.sourceView1.ReadOnly = false;
            this.sourceView1.Size = new System.Drawing.Size(328, 355);
            this.sourceView1.TabIndex = 0;
            // 
            // SeriesProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 434);
            this.Controls.Add(this.tbProperties);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SeriesProperties";
            this.Text = "Series Properties";
            this.tbProperties.ResumeLayout(false);
            this.tbSite.ResumeLayout(false);
            this.tbVariable.ResumeLayout(false);
            this.tbMethod.ResumeLayout(false);
            this.tbSource.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TabControl tbProperties;
        private System.Windows.Forms.TabPage tbSite;
        private System.Windows.Forms.TabPage tbVariable;
        private HydroDesktop.ObjectModel.Controls.SiteView siteView1;
        private HydroDesktop.ObjectModel.Controls.VariableView variableView1;
        private System.Windows.Forms.TabPage tbMethod;
        private HydroDesktop.ObjectModel.Controls.MethodView methodView1;
        private System.Windows.Forms.TabPage tbSource;
        private HydroDesktop.ObjectModel.Controls.SourceView sourceView1;
    }
}