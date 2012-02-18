namespace DataImport.CommonPages
{
    partial class DetailsForm
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.tcDetails = new System.Windows.Forms.TabControl();
            this.tpSource = new System.Windows.Forms.TabPage();
            this.sourceView = new DataImport.CommonPages.SourceView();
            this.tpMethod = new System.Windows.Forms.TabPage();
            this.methodView = new DataImport.CommonPages.MethodView();
            this.tbQualityControl = new System.Windows.Forms.TabPage();
            this.qualityControlLevelView = new DataImport.CommonPages.QualityControlLevelView();
            this.tcDetails.SuspendLayout();
            this.tpSource.SuspendLayout();
            this.tpMethod.SuspendLayout();
            this.tbQualityControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(284, 412);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(366, 411);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tcDetails
            // 
            this.tcDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcDetails.Controls.Add(this.tpSource);
            this.tcDetails.Controls.Add(this.tpMethod);
            this.tcDetails.Controls.Add(this.tbQualityControl);
            this.tcDetails.Location = new System.Drawing.Point(13, 12);
            this.tcDetails.Name = "tcDetails";
            this.tcDetails.SelectedIndex = 0;
            this.tcDetails.Size = new System.Drawing.Size(427, 382);
            this.tcDetails.TabIndex = 2;
            // 
            // tpSource
            // 
            this.tpSource.Controls.Add(this.sourceView);
            this.tpSource.Location = new System.Drawing.Point(4, 22);
            this.tpSource.Name = "tpSource";
            this.tpSource.Padding = new System.Windows.Forms.Padding(3);
            this.tpSource.Size = new System.Drawing.Size(419, 356);
            this.tpSource.TabIndex = 0;
            this.tpSource.Text = "Source";
            this.tpSource.UseVisualStyleBackColor = true;
            // 
            // sourceView
            // 
            this.sourceView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceView.Location = new System.Drawing.Point(3, 3);
            this.sourceView.MinimumSize = new System.Drawing.Size(200, 355);
            this.sourceView.Name = "sourceView";
            this.sourceView.Size = new System.Drawing.Size(413, 355);
            this.sourceView.TabIndex = 0;
            // 
            // tpMethod
            // 
            this.tpMethod.Controls.Add(this.methodView);
            this.tpMethod.Location = new System.Drawing.Point(4, 22);
            this.tpMethod.Name = "tpMethod";
            this.tpMethod.Padding = new System.Windows.Forms.Padding(3);
            this.tpMethod.Size = new System.Drawing.Size(419, 356);
            this.tpMethod.TabIndex = 1;
            this.tpMethod.Text = "Method";
            this.tpMethod.UseVisualStyleBackColor = true;
            // 
            // methodView
            // 
            this.methodView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.methodView.Location = new System.Drawing.Point(3, 3);
            this.methodView.Name = "methodView";
            this.methodView.Size = new System.Drawing.Size(413, 350);
            this.methodView.TabIndex = 0;
            // 
            // tbQualityControl
            // 
            this.tbQualityControl.Controls.Add(this.qualityControlLevelView);
            this.tbQualityControl.Location = new System.Drawing.Point(4, 22);
            this.tbQualityControl.Name = "tbQualityControl";
            this.tbQualityControl.Padding = new System.Windows.Forms.Padding(3);
            this.tbQualityControl.Size = new System.Drawing.Size(419, 356);
            this.tbQualityControl.TabIndex = 2;
            this.tbQualityControl.Text = "Quality Control";
            this.tbQualityControl.UseVisualStyleBackColor = true;
            // 
            // qualityControlLevelView
            // 
            this.qualityControlLevelView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.qualityControlLevelView.Location = new System.Drawing.Point(3, 3);
            this.qualityControlLevelView.Name = "qualityControlLevelView";
            this.qualityControlLevelView.Size = new System.Drawing.Size(413, 350);
            this.qualityControlLevelView.TabIndex = 0;
            // 
            // DetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(452, 447);
            this.Controls.Add(this.tcDetails);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetailsForm";
            this.Text = "Details";
            this.tcDetails.ResumeLayout(false);
            this.tpSource.ResumeLayout(false);
            this.tpMethod.ResumeLayout(false);
            this.tbQualityControl.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabControl tcDetails;
        private System.Windows.Forms.TabPage tpSource;
        private System.Windows.Forms.TabPage tpMethod;
        private MethodView methodView;
        private SourceView sourceView;
        private System.Windows.Forms.TabPage tbQualityControl;
        private QualityControlLevelView qualityControlLevelView;
    }
}