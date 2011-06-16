namespace HydroDesktop.Main
{
    partial class EditPluginsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditPluginsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTurnAllOff = new System.Windows.Forms.Button();
            this.btnTurnAllOn = new System.Windows.Forms.Button();
            this.listPlugins = new System.Windows.Forms.CheckedListBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.rtbPluginInfo = new System.Windows.Forms.RichTextBox();
            this.cmdOK = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnTurnAllOff);
            this.groupBox1.Controls.Add(this.btnTurnAllOn);
            this.groupBox1.Controls.Add(this.listPlugins);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(258, 256);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Loaded Plug-ins";
            // 
            // btnTurnAllOff
            // 
            this.btnTurnAllOff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTurnAllOff.Location = new System.Drawing.Point(174, 224);
            this.btnTurnAllOff.Name = "btnTurnAllOff";
            this.btnTurnAllOff.Size = new System.Drawing.Size(71, 23);
            this.btnTurnAllOff.TabIndex = 2;
            this.btnTurnAllOff.Text = "Turn All O&ff";
            this.btnTurnAllOff.UseVisualStyleBackColor = true;
            this.btnTurnAllOff.Click += new System.EventHandler(this.btnTurnAllOff_Click);
            // 
            // btnTurnAllOn
            // 
            this.btnTurnAllOn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTurnAllOn.Location = new System.Drawing.Point(93, 224);
            this.btnTurnAllOn.Name = "btnTurnAllOn";
            this.btnTurnAllOn.Size = new System.Drawing.Size(71, 23);
            this.btnTurnAllOn.TabIndex = 1;
            this.btnTurnAllOn.Text = "Turn All &On";
            this.btnTurnAllOn.UseVisualStyleBackColor = true;
            this.btnTurnAllOn.Click += new System.EventHandler(this.btnTurnAllOn_Click);
            // 
            // listPlugins
            // 
            this.listPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listPlugins.CheckOnClick = true;
            this.listPlugins.FormattingEnabled = true;
            this.listPlugins.Location = new System.Drawing.Point(8, 16);
            this.listPlugins.Name = "listPlugins";
            this.listPlugins.Size = new System.Drawing.Size(242, 199);
            this.listPlugins.TabIndex = 0;
            this.listPlugins.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listPlugins_ItemCheck);
            // 
            // Label3
            // 
            this.Label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label3.Location = new System.Drawing.Point(13, 268);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(109, 18);
            this.Label3.TabIndex = 3;
            this.Label3.Text = "Plug-in Details:";
            // 
            // rtbPluginInfo
            // 
            this.rtbPluginInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbPluginInfo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rtbPluginInfo.Location = new System.Drawing.Point(16, 285);
            this.rtbPluginInfo.Name = "rtbPluginInfo";
            this.rtbPluginInfo.Size = new System.Drawing.Size(242, 102);
            this.rtbPluginInfo.TabIndex = 4;
            this.rtbPluginInfo.Text = "";
            // 
            // cmdOK
            // 
            this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOK.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOK.Location = new System.Drawing.Point(100, 391);
            this.cmdOK.Name = "cmdOK";
            this.cmdOK.Size = new System.Drawing.Size(75, 23);
            this.cmdOK.TabIndex = 7;
            this.cmdOK.Text = "&OK";
            this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdCancel.Location = new System.Drawing.Point(180, 391);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 6;
            this.cmdCancel.Text = "&Cancel";
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // EditPluginsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(274, 419);
            this.Controls.Add(this.cmdOK);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.rtbPluginInfo);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(280, 352);
            this.Name = "EditPluginsForm";
            this.Text = "EditPluginsForm";
            this.Load += new System.EventHandler(this.EditPluginsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox listPlugins;
        private System.Windows.Forms.Button btnTurnAllOff;
        private System.Windows.Forms.Button btnTurnAllOn;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.RichTextBox rtbPluginInfo;
        internal System.Windows.Forms.Button cmdOK;
        internal System.Windows.Forms.Button cmdCancel;
    }
}