namespace DataImport.CommonPages
{
    partial class FieldPropertiesForm
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
            this.tbProperties = new System.Windows.Forms.TabControl();
            this.tbSite = new System.Windows.Forms.TabPage();
            this.chApplySiteToAllCoumns = new System.Windows.Forms.CheckBox();
            this.btnCreateNewSite = new System.Windows.Forms.Button();
            this.siteView1 = new DataImport.CommonPages.SiteView();
            this.cmbSites = new System.Windows.Forms.ComboBox();
            this.lblSelectSite = new System.Windows.Forms.Label();
            this.tbVariable = new System.Windows.Forms.TabPage();
            this.chApplyVariableToAllColumns = new System.Windows.Forms.CheckBox();
            this.btnCreateNewVariable = new System.Windows.Forms.Button();
            this.cmbVariables = new System.Windows.Forms.ComboBox();
            this.lblSelectVariable = new System.Windows.Forms.Label();
            this.variableView1 = new DataImport.CommonPages.VariableView();
            this.tpSource = new System.Windows.Forms.TabPage();
            this.sourceView1 = new DataImport.CommonPages.SourceView();
            this.tpMethod = new System.Windows.Forms.TabPage();
            this.methodView1 = new DataImport.CommonPages.MethodView();
            this.tpQualityControl = new System.Windows.Forms.TabPage();
            this.qualityControlLevelView1 = new DataImport.CommonPages.QualityControlLevelView();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.sitesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.variablesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.chApplySourceToAllColumns = new System.Windows.Forms.CheckBox();
            this.chApplyMethodToAllColumns = new System.Windows.Forms.CheckBox();
            this.chApplyQualityControlToAllColumns = new System.Windows.Forms.CheckBox();
            this.tbProperties.SuspendLayout();
            this.tbSite.SuspendLayout();
            this.tbVariable.SuspendLayout();
            this.tpSource.SuspendLayout();
            this.tpMethod.SuspendLayout();
            this.tpQualityControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sitesBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.variablesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tbProperties
            // 
            this.tbProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProperties.Controls.Add(this.tbSite);
            this.tbProperties.Controls.Add(this.tbVariable);
            this.tbProperties.Controls.Add(this.tpSource);
            this.tbProperties.Controls.Add(this.tpMethod);
            this.tbProperties.Controls.Add(this.tpQualityControl);
            this.tbProperties.Location = new System.Drawing.Point(12, 12);
            this.tbProperties.Name = "tbProperties";
            this.tbProperties.SelectedIndex = 0;
            this.tbProperties.Size = new System.Drawing.Size(449, 415);
            this.tbProperties.TabIndex = 0;
            // 
            // tbSite
            // 
            this.tbSite.Controls.Add(this.chApplySiteToAllCoumns);
            this.tbSite.Controls.Add(this.btnCreateNewSite);
            this.tbSite.Controls.Add(this.siteView1);
            this.tbSite.Controls.Add(this.cmbSites);
            this.tbSite.Controls.Add(this.lblSelectSite);
            this.tbSite.Location = new System.Drawing.Point(4, 22);
            this.tbSite.Name = "tbSite";
            this.tbSite.Padding = new System.Windows.Forms.Padding(3);
            this.tbSite.Size = new System.Drawing.Size(441, 389);
            this.tbSite.TabIndex = 0;
            this.tbSite.Text = "Site";
            this.tbSite.UseVisualStyleBackColor = true;
            // 
            // chApplySiteToAllCoumns
            // 
            this.chApplySiteToAllCoumns.AutoSize = true;
            this.chApplySiteToAllCoumns.Location = new System.Drawing.Point(10, 231);
            this.chApplySiteToAllCoumns.Name = "chApplySiteToAllCoumns";
            this.chApplySiteToAllCoumns.Size = new System.Drawing.Size(157, 17);
            this.chApplySiteToAllCoumns.TabIndex = 4;
            this.chApplySiteToAllCoumns.Text = "Apply this site to all columns";
            this.chApplySiteToAllCoumns.UseVisualStyleBackColor = true;
            // 
            // btnCreateNewSite
            // 
            this.btnCreateNewSite.Location = new System.Drawing.Point(223, 10);
            this.btnCreateNewSite.Name = "btnCreateNewSite";
            this.btnCreateNewSite.Size = new System.Drawing.Size(90, 23);
            this.btnCreateNewSite.TabIndex = 3;
            this.btnCreateNewSite.Text = "Create new...";
            this.btnCreateNewSite.UseVisualStyleBackColor = true;
            this.btnCreateNewSite.Click += new System.EventHandler(this.btnCreateNewSite_Click);
            // 
            // siteView1
            // 
            this.siteView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.siteView1.Location = new System.Drawing.Point(10, 37);
            this.siteView1.Name = "siteView1";
            this.siteView1.ReadOnly = false;
            this.siteView1.Size = new System.Drawing.Size(414, 176);
            this.siteView1.TabIndex = 2;
            // 
            // cmbSites
            // 
            this.cmbSites.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSites.FormattingEnabled = true;
            this.cmbSites.Location = new System.Drawing.Point(80, 12);
            this.cmbSites.Name = "cmbSites";
            this.cmbSites.Size = new System.Drawing.Size(121, 21);
            this.cmbSites.TabIndex = 1;
            // 
            // lblSelectSite
            // 
            this.lblSelectSite.AutoSize = true;
            this.lblSelectSite.Location = new System.Drawing.Point(7, 15);
            this.lblSelectSite.Name = "lblSelectSite";
            this.lblSelectSite.Size = new System.Drawing.Size(56, 13);
            this.lblSelectSite.TabIndex = 0;
            this.lblSelectSite.Text = "Select site";
            // 
            // tbVariable
            // 
            this.tbVariable.Controls.Add(this.chApplyVariableToAllColumns);
            this.tbVariable.Controls.Add(this.btnCreateNewVariable);
            this.tbVariable.Controls.Add(this.cmbVariables);
            this.tbVariable.Controls.Add(this.lblSelectVariable);
            this.tbVariable.Controls.Add(this.variableView1);
            this.tbVariable.Location = new System.Drawing.Point(4, 22);
            this.tbVariable.Name = "tbVariable";
            this.tbVariable.Padding = new System.Windows.Forms.Padding(3);
            this.tbVariable.Size = new System.Drawing.Size(441, 389);
            this.tbVariable.TabIndex = 1;
            this.tbVariable.Text = "Variable";
            this.tbVariable.UseVisualStyleBackColor = true;
            // 
            // chApplyVariableToAllColumns
            // 
            this.chApplyVariableToAllColumns.AutoSize = true;
            this.chApplyVariableToAllColumns.Location = new System.Drawing.Point(10, 321);
            this.chApplyVariableToAllColumns.Name = "chApplyVariableToAllColumns";
            this.chApplyVariableToAllColumns.Size = new System.Drawing.Size(178, 17);
            this.chApplyVariableToAllColumns.TabIndex = 6;
            this.chApplyVariableToAllColumns.Text = "Apply this variable to all columns";
            this.chApplyVariableToAllColumns.UseVisualStyleBackColor = true;
            // 
            // btnCreateNewVariable
            // 
            this.btnCreateNewVariable.Location = new System.Drawing.Point(242, 10);
            this.btnCreateNewVariable.Name = "btnCreateNewVariable";
            this.btnCreateNewVariable.Size = new System.Drawing.Size(90, 23);
            this.btnCreateNewVariable.TabIndex = 5;
            this.btnCreateNewVariable.Text = "Create new...";
            this.btnCreateNewVariable.UseVisualStyleBackColor = true;
            this.btnCreateNewVariable.Click += new System.EventHandler(this.btnCreateNewVariable_Click);
            // 
            // cmbVariables
            // 
            this.cmbVariables.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVariables.FormattingEnabled = true;
            this.cmbVariables.Location = new System.Drawing.Point(95, 12);
            this.cmbVariables.Name = "cmbVariables";
            this.cmbVariables.Size = new System.Drawing.Size(121, 21);
            this.cmbVariables.TabIndex = 3;
            // 
            // lblSelectVariable
            // 
            this.lblSelectVariable.AutoSize = true;
            this.lblSelectVariable.Location = new System.Drawing.Point(7, 15);
            this.lblSelectVariable.Name = "lblSelectVariable";
            this.lblSelectVariable.Size = new System.Drawing.Size(77, 13);
            this.lblSelectVariable.TabIndex = 2;
            this.lblSelectVariable.Text = "Select variable";
            // 
            // variableView1
            // 
            this.variableView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.variableView1.Location = new System.Drawing.Point(10, 39);
            this.variableView1.Name = "variableView1";
            this.variableView1.ReadOnly = false;
            this.variableView1.Size = new System.Drawing.Size(415, 276);
            this.variableView1.TabIndex = 4;
            // 
            // tpSource
            // 
            this.tpSource.Controls.Add(this.chApplySourceToAllColumns);
            this.tpSource.Controls.Add(this.sourceView1);
            this.tpSource.Location = new System.Drawing.Point(4, 22);
            this.tpSource.Name = "tpSource";
            this.tpSource.Padding = new System.Windows.Forms.Padding(3);
            this.tpSource.Size = new System.Drawing.Size(441, 389);
            this.tpSource.TabIndex = 2;
            this.tpSource.Text = "Source";
            this.tpSource.UseVisualStyleBackColor = true;
            // 
            // sourceView1
            // 
            this.sourceView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceView1.Location = new System.Drawing.Point(6, 6);
            this.sourceView1.MinimumSize = new System.Drawing.Size(200, 355);
            this.sourceView1.Name = "sourceView1";
            this.sourceView1.Size = new System.Drawing.Size(429, 355);
            this.sourceView1.TabIndex = 0;
            // 
            // tpMethod
            // 
            this.tpMethod.Controls.Add(this.chApplyMethodToAllColumns);
            this.tpMethod.Controls.Add(this.methodView1);
            this.tpMethod.Location = new System.Drawing.Point(4, 22);
            this.tpMethod.Name = "tpMethod";
            this.tpMethod.Padding = new System.Windows.Forms.Padding(3);
            this.tpMethod.Size = new System.Drawing.Size(441, 389);
            this.tpMethod.TabIndex = 3;
            this.tpMethod.Text = "Method";
            this.tpMethod.UseVisualStyleBackColor = true;
            // 
            // methodView1
            // 
            this.methodView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.methodView1.Location = new System.Drawing.Point(6, 6);
            this.methodView1.Name = "methodView1";
            this.methodView1.Size = new System.Drawing.Size(429, 260);
            this.methodView1.TabIndex = 0;
            // 
            // tpQualityControl
            // 
            this.tpQualityControl.Controls.Add(this.chApplyQualityControlToAllColumns);
            this.tpQualityControl.Controls.Add(this.qualityControlLevelView1);
            this.tpQualityControl.Location = new System.Drawing.Point(4, 22);
            this.tpQualityControl.Name = "tpQualityControl";
            this.tpQualityControl.Padding = new System.Windows.Forms.Padding(3);
            this.tpQualityControl.Size = new System.Drawing.Size(441, 389);
            this.tpQualityControl.TabIndex = 4;
            this.tpQualityControl.Text = "Quality Control";
            this.tpQualityControl.UseVisualStyleBackColor = true;
            // 
            // qualityControlLevelView1
            // 
            this.qualityControlLevelView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qualityControlLevelView1.Location = new System.Drawing.Point(6, 6);
            this.qualityControlLevelView1.Name = "qualityControlLevelView1";
            this.qualityControlLevelView1.Size = new System.Drawing.Size(429, 108);
            this.qualityControlLevelView1.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(305, 437);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(386, 437);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chApplySourceToAllColumns
            // 
            this.chApplySourceToAllColumns.AutoSize = true;
            this.chApplySourceToAllColumns.Location = new System.Drawing.Point(10, 366);
            this.chApplySourceToAllColumns.Name = "chApplySourceToAllColumns";
            this.chApplySourceToAllColumns.Size = new System.Drawing.Size(173, 17);
            this.chApplySourceToAllColumns.TabIndex = 7;
            this.chApplySourceToAllColumns.Text = "Apply this source to all columns";
            this.chApplySourceToAllColumns.UseVisualStyleBackColor = true;
            // 
            // chApplyMethodToAllColumns
            // 
            this.chApplyMethodToAllColumns.AutoSize = true;
            this.chApplyMethodToAllColumns.Location = new System.Drawing.Point(10, 281);
            this.chApplyMethodToAllColumns.Name = "chApplyMethodToAllColumns";
            this.chApplyMethodToAllColumns.Size = new System.Drawing.Size(176, 17);
            this.chApplyMethodToAllColumns.TabIndex = 8;
            this.chApplyMethodToAllColumns.Text = "Apply this method to all columns";
            this.chApplyMethodToAllColumns.UseVisualStyleBackColor = true;
            // 
            // chApplyQualityControlToAllColumns
            // 
            this.chApplyQualityControlToAllColumns.AutoSize = true;
            this.chApplyQualityControlToAllColumns.Location = new System.Drawing.Point(10, 120);
            this.chApplyQualityControlToAllColumns.Name = "chApplyQualityControlToAllColumns";
            this.chApplyQualityControlToAllColumns.Size = new System.Drawing.Size(206, 17);
            this.chApplyQualityControlToAllColumns.TabIndex = 9;
            this.chApplyQualityControlToAllColumns.Text = "Apply this quality control to all columns";
            this.chApplyQualityControlToAllColumns.UseVisualStyleBackColor = true;
            // 
            // FieldPropertiesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(473, 477);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbProperties);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FieldPropertiesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Field properties";
            this.tbProperties.ResumeLayout(false);
            this.tbSite.ResumeLayout(false);
            this.tbSite.PerformLayout();
            this.tbVariable.ResumeLayout(false);
            this.tbVariable.PerformLayout();
            this.tpSource.ResumeLayout(false);
            this.tpSource.PerformLayout();
            this.tpMethod.ResumeLayout(false);
            this.tpMethod.PerformLayout();
            this.tpQualityControl.ResumeLayout(false);
            this.tpQualityControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sitesBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.variablesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tbProperties;
        private System.Windows.Forms.TabPage tbSite;
        private System.Windows.Forms.TabPage tbVariable;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbSites;
        private System.Windows.Forms.Label lblSelectSite;
        private System.Windows.Forms.ComboBox cmbVariables;
        private System.Windows.Forms.Label lblSelectVariable;
        private SiteView siteView1;
        private VariableView variableView1;
        private System.Windows.Forms.Button btnCreateNewSite;
        private System.Windows.Forms.Button btnCreateNewVariable;
        private System.Windows.Forms.BindingSource sitesBindingSource;
        private System.Windows.Forms.BindingSource variablesBindingSource;
        private System.Windows.Forms.CheckBox chApplySiteToAllCoumns;
        private System.Windows.Forms.CheckBox chApplyVariableToAllColumns;
        private System.Windows.Forms.TabPage tpSource;
        private System.Windows.Forms.TabPage tpMethod;
        private System.Windows.Forms.TabPage tpQualityControl;
        private SourceView sourceView1;
        private MethodView methodView1;
        private QualityControlLevelView qualityControlLevelView1;
        private System.Windows.Forms.CheckBox chApplySourceToAllColumns;
        private System.Windows.Forms.CheckBox chApplyMethodToAllColumns;
        private System.Windows.Forms.CheckBox chApplyQualityControlToAllColumns;
    }
}