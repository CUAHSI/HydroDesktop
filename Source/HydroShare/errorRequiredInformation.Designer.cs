namespace HydroShare
{
    partial class errorRequiredInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(errorRequiredInformation));
            this.label1 = new System.Windows.Forms.Label();
            this.errorMessage = new System.Windows.Forms.PictureBox();
            this.okBT = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errorMessage)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 78);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Error. Please fill out all required text boxes (*).\r\n";
            // 
            // errorMessage
            // 
            this.errorMessage.ErrorImage = ((System.Drawing.Image)(resources.GetObject("errorMessage.ErrorImage")));
            this.errorMessage.Image = global::HydroShare.Properties.Resources.errorImage1;
            this.errorMessage.InitialImage = ((System.Drawing.Image)(resources.GetObject("errorMessage.InitialImage")));
            this.errorMessage.Location = new System.Drawing.Point(91, 12);
            this.errorMessage.Name = "errorMessage";
            this.errorMessage.Size = new System.Drawing.Size(50, 50);
            this.errorMessage.TabIndex = 1;
            this.errorMessage.TabStop = false;
            this.errorMessage.Click += new System.EventHandler(this.errorMessage_Click);
            // 
            // okBT
            // 
            this.okBT.Location = new System.Drawing.Point(75, 107);
            this.okBT.Name = "okBT";
            this.okBT.Size = new System.Drawing.Size(83, 24);
            this.okBT.TabIndex = 2;
            this.okBT.Text = "Ok";
            this.okBT.UseVisualStyleBackColor = true;
            this.okBT.Click += new System.EventHandler(this.okBT_Click);
            // 
            // errorRequiredInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(243, 152);
            this.Controls.Add(this.okBT);
            this.Controls.Add(this.errorMessage);
            this.Controls.Add(this.label1);
            this.Name = "errorRequiredInformation";
            this.Text = "Error";
            ((System.ComponentModel.ISupportInitialize)(this.errorMessage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox errorMessage;
        private System.Windows.Forms.Button okBT;
    }
}