namespace HydroDesktop.Plugins.HydroR.Controls
{
    partial class REditor
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
            
            this.transp = new HydroR.cHighlight();           

            this.Controls.Add(this.transp);
            this.VScroll += new System.EventHandler(this.Scroll);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rMouseUp);
            this.HScroll += new System.EventHandler(this.Scroll);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.rKeyUp);
            this.TextChanged += new System.EventHandler(this.rTextChanged);

            // 
            // transp
            // 
            this.transp.BackColor = System.Drawing.Color.Yellow;
            this.transp.Location = new System.Drawing.Point(0, 0);
            this.transp.Name = "transp";
            this.transp.Opacity = 20;
            this.transp.Size = new System.Drawing.Size(300, 17);
            this.transp.TabIndex = 0;

        }

        #endregion

        //public System.Windows.Forms.RichTextBox textBox;
        private cHighlight transp;
    }
}
