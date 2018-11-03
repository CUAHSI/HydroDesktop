namespace HydroDesktop.Plugins.TableView
{
    partial class DeleteThemeForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeleteThemeForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.bgwMain = new System.ComponentModel.BackgroundWorker();
            this.gbxProgress = new System.Windows.Forms.GroupBox();
            this.btnPgsCancel = new System.Windows.Forms.Button();
            this.pgsBar = new System.Windows.Forms.ProgressBar();
            this.gbxDelete = new System.Windows.Forms.GroupBox();
            this.checkListThemes = new System.Windows.Forms.CheckedListBox();
            this.gbxProgress.SuspendLayout();
            this.gbxDelete.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(83, 12);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(109, 27);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "Remove Data Sites";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(206, 12);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(68, 27);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(191, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select the Data sites layer(s) to remove";
            // 
            // bgwMain
            // 
            this.bgwMain.WorkerReportsProgress = true;
            this.bgwMain.WorkerSupportsCancellation = true;
            // 
            // gbxProgress
            // 
            this.gbxProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxProgress.Controls.Add(this.btnPgsCancel);
            this.gbxProgress.Controls.Add(this.pgsBar);
            this.gbxProgress.Location = new System.Drawing.Point(6, 172);
            this.gbxProgress.Name = "gbxProgress";
            this.gbxProgress.Size = new System.Drawing.Size(279, 45);
            this.gbxProgress.TabIndex = 4;
            this.gbxProgress.TabStop = false;
            this.gbxProgress.Text = "Processing..";
            this.gbxProgress.Visible = false;
            // 
            // btnPgsCancel
            // 
            this.btnPgsCancel.Location = new System.Drawing.Point(198, 16);
            this.btnPgsCancel.Name = "btnPgsCancel";
            this.btnPgsCancel.Size = new System.Drawing.Size(75, 23);
            this.btnPgsCancel.TabIndex = 1;
            this.btnPgsCancel.Text = "Cancel";
            this.btnPgsCancel.UseVisualStyleBackColor = true;
            this.btnPgsCancel.Click += new System.EventHandler(this.btnPgsCancel_Click_1);
            // 
            // pgsBar
            // 
            this.pgsBar.Location = new System.Drawing.Point(6, 16);
            this.pgsBar.Name = "pgsBar";
            this.pgsBar.Size = new System.Drawing.Size(170, 23);
            this.pgsBar.TabIndex = 0;
            // 
            // gbxDelete
            // 
            this.gbxDelete.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxDelete.Controls.Add(this.btnOK);
            this.gbxDelete.Controls.Add(this.btnCancel);
            this.gbxDelete.Location = new System.Drawing.Point(6, 172);
            this.gbxDelete.Name = "gbxDelete";
            this.gbxDelete.Size = new System.Drawing.Size(279, 45);
            this.gbxDelete.TabIndex = 2;
            this.gbxDelete.TabStop = false;
            // 
            // checkListThemes
            // 
            this.checkListThemes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkListThemes.CheckOnClick = true;
            this.checkListThemes.FormattingEnabled = true;
            this.checkListThemes.Location = new System.Drawing.Point(6, 33);
            this.checkListThemes.Name = "checkListThemes";
            this.checkListThemes.Size = new System.Drawing.Size(279, 139);
            this.checkListThemes.TabIndex = 5;
            this.checkListThemes.SelectedIndexChanged += new System.EventHandler(this.checkListThemes_SelectedIndexChanged);
            // 
            // DeleteThemeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 224);
            this.Controls.Add(this.checkListThemes);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbxDelete);
            this.Controls.Add(this.gbxProgress);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeleteThemeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Remove Data Sites from Database";
            this.Load += new System.EventHandler(this.DeleteThemeForm_Load);
            this.gbxProgress.ResumeLayout(false);
            this.gbxDelete.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker bgwMain;
        private System.Windows.Forms.GroupBox gbxProgress;
        private System.Windows.Forms.ProgressBar pgsBar;
        private System.Windows.Forms.Button btnPgsCancel;
        private System.Windows.Forms.GroupBox gbxDelete;
        private System.Windows.Forms.CheckedListBox checkListThemes;
    }
}