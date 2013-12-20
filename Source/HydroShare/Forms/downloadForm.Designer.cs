namespace HydroShare
{
    partial class downloadForm
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
            this.cmb_FilterSearch = new System.Windows.Forms.ComboBox();
            this.lst_AvailableItems = new System.Windows.Forms.ListBox();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_GetData = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmb_FilterSearch
            // 
            this.cmb_FilterSearch.FormattingEnabled = true;
            this.cmb_FilterSearch.Location = new System.Drawing.Point(12, 12);
            this.cmb_FilterSearch.Name = "cmb_FilterSearch";
            this.cmb_FilterSearch.Size = new System.Drawing.Size(260, 21);
            this.cmb_FilterSearch.TabIndex = 0;
            this.cmb_FilterSearch.Text = "Filter Search...";
            this.cmb_FilterSearch.SelectedIndexChanged += new System.EventHandler(this.lst_AvailableItems_Click);
            // 
            // lst_AvailableItems
            // 
            this.lst_AvailableItems.FormattingEnabled = true;
            this.lst_AvailableItems.Location = new System.Drawing.Point(13, 40);
            this.lst_AvailableItems.Name = "lst_AvailableItems";
            this.lst_AvailableItems.Size = new System.Drawing.Size(259, 199);
            this.lst_AvailableItems.TabIndex = 1;
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Location = new System.Drawing.Point(12, 245);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(126, 23);
            this.btn_Cancel.TabIndex = 2;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_GetData
            // 
            this.btn_GetData.Location = new System.Drawing.Point(146, 245);
            this.btn_GetData.Name = "btn_GetData";
            this.btn_GetData.Size = new System.Drawing.Size(126, 23);
            this.btn_GetData.TabIndex = 3;
            this.btn_GetData.Text = "Download";
            this.btn_GetData.UseVisualStyleBackColor = true;
            // 
            // downloadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 277);
            this.Controls.Add(this.btn_GetData);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.lst_AvailableItems);
            this.Controls.Add(this.cmb_FilterSearch);
            this.Name = "downloadForm";
            this.Text = "Find Resources on HydroShare";
            this.Load += new System.EventHandler(this.downloadForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_FilterSearch;
        private System.Windows.Forms.ListBox lst_AvailableItems;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_GetData;
    }
}