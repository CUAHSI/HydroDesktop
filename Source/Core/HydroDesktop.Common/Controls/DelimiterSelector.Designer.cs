namespace HydroDesktop.Common.Controls
{
    partial class DelimiterSelector
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
            this.gbxDelimiters = new System.Windows.Forms.GroupBox();
            this.rdoComma = new System.Windows.Forms.RadioButton();
            this.rdoTab = new System.Windows.Forms.RadioButton();
            this.rdoSpace = new System.Windows.Forms.RadioButton();
            this.rdoPipe = new System.Windows.Forms.RadioButton();
            this.rdoSemicolon = new System.Windows.Forms.RadioButton();
            this.rdoOthers = new System.Windows.Forms.RadioButton();
            this.tbOther = new System.Windows.Forms.TextBox();
            this.gbxDelimiters.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbxDelimiters
            // 
            this.gbxDelimiters.Controls.Add(this.rdoComma);
            this.gbxDelimiters.Controls.Add(this.rdoTab);
            this.gbxDelimiters.Controls.Add(this.rdoSpace);
            this.gbxDelimiters.Controls.Add(this.rdoPipe);
            this.gbxDelimiters.Controls.Add(this.rdoSemicolon);
            this.gbxDelimiters.Controls.Add(this.rdoOthers);
            this.gbxDelimiters.Controls.Add(this.tbOther);
            this.gbxDelimiters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gbxDelimiters.Location = new System.Drawing.Point(0, 0);
            this.gbxDelimiters.Name = "gbxDelimiters";
            this.gbxDelimiters.Size = new System.Drawing.Size(313, 78);
            this.gbxDelimiters.TabIndex = 11;
            this.gbxDelimiters.TabStop = false;
            this.gbxDelimiters.Text = "Select a Delimiter";
            // 
            // rdoComma
            // 
            this.rdoComma.AutoSize = true;
            this.rdoComma.Location = new System.Drawing.Point(11, 23);
            this.rdoComma.Name = "rdoComma";
            this.rdoComma.Size = new System.Drawing.Size(90, 17);
            this.rdoComma.TabIndex = 0;
            this.rdoComma.Text = "&Comma (CSV)";
            this.rdoComma.UseVisualStyleBackColor = true;
            // 
            // rdoTab
            // 
            this.rdoTab.AutoSize = true;
            this.rdoTab.Location = new System.Drawing.Point(116, 23);
            this.rdoTab.Name = "rdoTab";
            this.rdoTab.Size = new System.Drawing.Size(44, 17);
            this.rdoTab.TabIndex = 1;
            this.rdoTab.Text = "&Tab";
            this.rdoTab.UseVisualStyleBackColor = true;
            // 
            // rdoSpace
            // 
            this.rdoSpace.AutoSize = true;
            this.rdoSpace.Location = new System.Drawing.Point(203, 23);
            this.rdoSpace.Name = "rdoSpace";
            this.rdoSpace.Size = new System.Drawing.Size(56, 17);
            this.rdoSpace.TabIndex = 2;
            this.rdoSpace.Text = "&Space";
            this.rdoSpace.UseVisualStyleBackColor = true;
            // 
            // rdoPipe
            // 
            this.rdoPipe.AutoSize = true;
            this.rdoPipe.Location = new System.Drawing.Point(11, 41);
            this.rdoPipe.Name = "rdoPipe";
            this.rdoPipe.Size = new System.Drawing.Size(46, 17);
            this.rdoPipe.TabIndex = 3;
            this.rdoPipe.Text = "&Pipe";
            this.rdoPipe.UseVisualStyleBackColor = true;
            // 
            // rdoSemicolon
            // 
            this.rdoSemicolon.AutoSize = true;
            this.rdoSemicolon.Location = new System.Drawing.Point(116, 41);
            this.rdoSemicolon.Name = "rdoSemicolon";
            this.rdoSemicolon.Size = new System.Drawing.Size(74, 17);
            this.rdoSemicolon.TabIndex = 4;
            this.rdoSemicolon.Text = "Se&micolon";
            this.rdoSemicolon.UseVisualStyleBackColor = true;
            // 
            // rdoOthers
            // 
            this.rdoOthers.AutoSize = true;
            this.rdoOthers.Location = new System.Drawing.Point(203, 41);
            this.rdoOthers.Name = "rdoOthers";
            this.rdoOthers.Size = new System.Drawing.Size(54, 17);
            this.rdoOthers.TabIndex = 5;
            this.rdoOthers.Text = "&Other:";
            this.rdoOthers.UseVisualStyleBackColor = true;
            // 
            // tbOther
            // 
            this.tbOther.Location = new System.Drawing.Point(269, 38);
            this.tbOther.MaxLength = 1;
            this.tbOther.Name = "tbOther";
            this.tbOther.Size = new System.Drawing.Size(27, 20);
            this.tbOther.TabIndex = 6;
            this.tbOther.TextChanged += new System.EventHandler(this.tbOther_TextChanged);
            // 
            // DelimiterSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbxDelimiters);
            this.Name = "DelimiterSelector";
            this.Size = new System.Drawing.Size(313, 78);
            this.gbxDelimiters.ResumeLayout(false);
            this.gbxDelimiters.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbxDelimiters;
        private System.Windows.Forms.RadioButton rdoComma;
        private System.Windows.Forms.RadioButton rdoTab;
        private System.Windows.Forms.RadioButton rdoSpace;
        private System.Windows.Forms.RadioButton rdoPipe;
        private System.Windows.Forms.RadioButton rdoSemicolon;
        private System.Windows.Forms.RadioButton rdoOthers;
        private System.Windows.Forms.TextBox tbOther;
    }
}
