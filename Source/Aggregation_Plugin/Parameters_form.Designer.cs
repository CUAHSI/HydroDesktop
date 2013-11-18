namespace Aggregation_Plugin
{
    partial class Parameters_form
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
            this.OK_click = new System.Windows.Forms.Button();
            this.Cancel_click = new System.Windows.Forms.Button();
            this.Polygon_cbx = new System.Windows.Forms.ComboBox();
            this.Points_cbx = new System.Windows.Forms.ComboBox();
            this.Polygon_Label = new System.Windows.Forms.Label();
            this.Point_Label = new System.Windows.Forms.Label();
            this.Variable_Label = new System.Windows.Forms.Label();
            this.Output_txt = new System.Windows.Forms.TextBox();
            this.Variable_cbx = new System.Windows.Forms.ComboBox();
            this.Output_Label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // OK_click
            // 
            this.OK_click.Location = new System.Drawing.Point(141, 116);
            this.OK_click.Name = "OK_click";
            this.OK_click.Size = new System.Drawing.Size(75, 23);
            this.OK_click.TabIndex = 0;
            this.OK_click.Text = "OK";
            this.OK_click.UseVisualStyleBackColor = true;
            // 
            // Cancel_click
            // 
            this.Cancel_click.Location = new System.Drawing.Point(60, 116);
            this.Cancel_click.Name = "Cancel_click";
            this.Cancel_click.Size = new System.Drawing.Size(75, 23);
            this.Cancel_click.TabIndex = 1;
            this.Cancel_click.Text = "Cancel";
            this.Cancel_click.UseVisualStyleBackColor = true;
            // 
            // Polygon_cbx
            // 
            this.Polygon_cbx.FormattingEnabled = true;
            this.Polygon_cbx.Location = new System.Drawing.Point(95, 9);
            this.Polygon_cbx.Name = "Polygon_cbx";
            this.Polygon_cbx.Size = new System.Drawing.Size(121, 21);
            this.Polygon_cbx.TabIndex = 2;
            // 
            // Points_cbx
            // 
            this.Points_cbx.FormattingEnabled = true;
            this.Points_cbx.Location = new System.Drawing.Point(95, 36);
            this.Points_cbx.Name = "Points_cbx";
            this.Points_cbx.Size = new System.Drawing.Size(121, 21);
            this.Points_cbx.TabIndex = 3;
            // 
            // Polygon_Label
            // 
            this.Polygon_Label.AutoSize = true;
            this.Polygon_Label.Location = new System.Drawing.Point(12, 9);
            this.Polygon_Label.Name = "Polygon_Label";
            this.Polygon_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Polygon_Label.Size = new System.Drawing.Size(77, 13);
            this.Polygon_Label.TabIndex = 4;
            this.Polygon_Label.Text = "Select polygon";
            // 
            // Point_Label
            // 
            this.Point_Label.AutoSize = true;
            this.Point_Label.Location = new System.Drawing.Point(2, 36);
            this.Point_Label.Name = "Point_Label";
            this.Point_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Point_Label.Size = new System.Drawing.Size(87, 13);
            this.Point_Label.TabIndex = 5;
            this.Point_Label.Text = "Select point data";
            // 
            // Variable_Label
            // 
            this.Variable_Label.AutoSize = true;
            this.Variable_Label.Location = new System.Drawing.Point(12, 63);
            this.Variable_Label.Name = "Variable_Label";
            this.Variable_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Variable_Label.Size = new System.Drawing.Size(77, 13);
            this.Variable_Label.TabIndex = 6;
            this.Variable_Label.Text = "Select variable";
            // 
            // Output_txt
            // 
            this.Output_txt.Location = new System.Drawing.Point(95, 90);
            this.Output_txt.Name = "Output_txt";
            this.Output_txt.Size = new System.Drawing.Size(121, 20);
            this.Output_txt.TabIndex = 7;
            // 
            // Variable_cbx
            // 
            this.Variable_cbx.FormattingEnabled = true;
            this.Variable_cbx.Location = new System.Drawing.Point(95, 63);
            this.Variable_cbx.Name = "Variable_cbx";
            this.Variable_cbx.Size = new System.Drawing.Size(121, 21);
            this.Variable_cbx.TabIndex = 8;
            // 
            // Output_Label
            // 
            this.Output_Label.AutoSize = true;
            this.Output_Label.Location = new System.Drawing.Point(21, 90);
            this.Output_Label.Name = "Output_Label";
            this.Output_Label.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Output_Label.Size = new System.Drawing.Size(68, 13);
            this.Output_Label.TabIndex = 9;
            this.Output_Label.Text = "Output name";
            // 
            // Parameters_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(226, 147);
            this.Controls.Add(this.Output_Label);
            this.Controls.Add(this.Variable_cbx);
            this.Controls.Add(this.Output_txt);
            this.Controls.Add(this.Variable_Label);
            this.Controls.Add(this.Point_Label);
            this.Controls.Add(this.Polygon_Label);
            this.Controls.Add(this.Points_cbx);
            this.Controls.Add(this.Polygon_cbx);
            this.Controls.Add(this.Cancel_click);
            this.Controls.Add(this.OK_click);
            this.Name = "Parameters_form";
            this.Text = "Aggregation tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button OK_click;
        private System.Windows.Forms.Button Cancel_click;
        private System.Windows.Forms.ComboBox Polygon_cbx;
        private System.Windows.Forms.ComboBox Points_cbx;
        private System.Windows.Forms.Label Polygon_Label;
        private System.Windows.Forms.Label Point_Label;
        private System.Windows.Forms.Label Variable_Label;
        private System.Windows.Forms.TextBox Output_txt;
        private System.Windows.Forms.ComboBox Variable_cbx;
        private System.Windows.Forms.Label Output_Label;
    }
}